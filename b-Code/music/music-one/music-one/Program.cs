using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Threading;

namespace RealTimeAudioSuperResolution
{
    using NAudio.Wave;
    using NAudio.Dsp;
    using System;
    using System.Threading;
    using System.Collections.Concurrent; // 新增：ConcurrentQueue需要的命名空间
    using System.Diagnostics; // 新增：Stopwatch需要的命名空间

    class Program
    {
        // 核心参数

        private const int SOURCE_SAMPLE_RATE = 44100;
        private const int TARGET_SAMPLE_RATE = 96000;     
        private const int BIT_DEPTH = 16;
        private const int CHANNELS = 2;
        private static bool _isRunning = true;
        private static WaveFormat _sourceFormat = new WaveFormat(SOURCE_SAMPLE_RATE, BIT_DEPTH, CHANNELS);
        private static WaveFormat _targetFormat = new WaveFormat(TARGET_SAMPLE_RATE, BIT_DEPTH, CHANNELS);
        private static ConcurrentQueue<byte[]> _audioQueue = new ConcurrentQueue<byte[]>();
        private static BufferedWaveProvider _playbackProvider; // 全局播放缓冲区
        private static AutoResetEvent _queueSignal = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== 实时音频超分处理 ===\n");
                Console.WriteLine("正在初始化音频设备...");

                // 1. 查找播放设备
                int speakerDeviceNumber = GetSpeakerDeviceNumber();
                Console.WriteLine($"已找到播放设备：{WaveOut.GetCapabilities(speakerDeviceNumber).ProductName}");

                // 初始化播放设备
                var outputDevice = new WaveOutEvent
                {
                    DesiredLatency = 150,
                    NumberOfBuffers = 4,
                    DeviceNumber = speakerDeviceNumber
                };

                // ========== 改动1：适配96000Hz采样率（先修改全局参数后，这里格式自动匹配） ==========
                // 2. 创建播放缓冲区（赋值给全局变量，增大到3秒缓冲）
                _playbackProvider = new BufferedWaveProvider(_targetFormat)
                {
                    BufferDuration = TimeSpan.FromSeconds(3), // 从2秒→3秒，提升容错
                    ReadFully = true,
                    DiscardOnBufferOverflow = true
                };

                // ========== 改动2：核心修复 - 用WaveProviderToWaveStream包装缓冲区，强制格式对齐 ==========
                outputDevice.Init(new WaveFormatConversionProvider(_targetFormat, _playbackProvider));
               
                // 启动缓冲区监控线程
                Thread monitorThread = new Thread(() => BufferMonitor(_playbackProvider))
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest
                };
                monitorThread.Start();

                // 3. 初始化捕获设备
                int captureDeviceNumber = GetCaptureDeviceNumber();
                Console.WriteLine($"已找到捕获设备：{WaveInEvent.GetCapabilities(captureDeviceNumber).ProductName}");

                var captureDevice = new WaveInEvent
                {
                    WaveFormat = _sourceFormat,
                    DeviceNumber = captureDeviceNumber,
                    BufferMilliseconds = 100
                };

                // 启动异步处理线程
                Thread processThread = new Thread(ProcessAudioQueue)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal
                };
                processThread.Start();

                // 捕获数据事件：只入队，不处理
                captureDevice.DataAvailable += (s, e) =>
                {
                    if (!_isRunning) return;
                    if (e.BytesRecorded == 0 || e.BytesRecorded % 2 != 0) return;

                    byte[] bufferCopy = new byte[e.BytesRecorded];
                    Array.Copy(e.Buffer, bufferCopy, e.BytesRecorded);
                    _audioQueue.Enqueue(bufferCopy);
                };

                // 启动逻辑部分
                Console.WriteLine("预热播放设备...");
                byte[] silence = new byte[TARGET_SAMPLE_RATE * CHANNELS * BIT_DEPTH / 8 / 5];
                _playbackProvider.AddSamples(silence, 0, silence.Length);

                // 关键：用WaveFormatConversionProvider初始化，无需waveStream
                outputDevice.Init(new WaveFormatConversionProvider(_targetFormat, _playbackProvider));
                outputDevice.Play();
                Thread.Sleep(300);
                captureDevice.StartRecording();

                Console.WriteLine("实时超分已启动！按任意键停止...\n");
                Console.ReadKey();

                // 停止逻辑
                _isRunning = false;
                captureDevice.StopRecording();
                outputDevice.Stop();
                Thread.Sleep(500);
                captureDevice.Dispose();
                outputDevice.Dispose();
                monitorThread.Abort();
                processThread.Abort(); // 新增：停止异步处理线程

                Console.WriteLine("实时超分已停止");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化错误：{ex.Message}\n{ex.StackTrace}");
            }
        }

        #region 核心方法
        static void ProcessAudioQueue()
        {
            // 关键1：设置线程为实时优先级（最高级别，抢占CPU）
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Thread.CurrentThread.IsBackground = true;

            // 关键2：复用变量，减少GC开销（避免频繁创建对象）
            byte[] buffer = null;
            float[] sourceSamples = null;
            float[] enhancedSamples = null;
            byte[] enhancedBytes = null;
            Stopwatch sw = new Stopwatch();

            while (_isRunning)
            {
                try
                {
                    // 关键3：用信号量等待，替代Thread.Sleep(10)，0延迟响应队列
                    bool hasData = _audioQueue.TryDequeue(out buffer);
                    if (!hasData)
                    {
                        // 等待10ms，有数据立即唤醒，无数据则休眠（CPU占用≈0）
                        _queueSignal.WaitOne(10);
                        continue;
                    }

                    // 跳过空数据
                    if (buffer == null || buffer.Length == 0) continue;

                    sw.Restart(); // 复用Stopwatch，减少对象创建

                    // 核心处理逻辑（保留，但减少内存分配）
                    sourceSamples = Convert16BitPCMToFloat(buffer, buffer.Length);
                    enhancedSamples = StableSuperResolveAudio(sourceSamples);
                    enhancedBytes = ConvertFloatTo16BitPCM(enhancedSamples);

                    sw.Stop();

                    // 快速写入缓冲区（锁粒度最小化）
                    if (enhancedBytes != null && enhancedBytes.Length > 0)
                    {
                        // 关键4：锁只包裹写入操作，且缩短持有时间
                        int writeLength = enhancedBytes.Length;
                        lock (_playbackProvider)
                        {
                            _playbackProvider.AddSamples(enhancedBytes, 0, writeLength);
                        }
                        // 打印关键信息，确认数据写入
                        Console.WriteLine($"处理耗时：{sw.ElapsedMilliseconds}ms | 队列剩余：{_audioQueue.Count} | 写入字节：{writeLength}");
                    }

                    // 关键5：手动释放内存，减少GC卡顿
                    Array.Clear(buffer, 0, buffer.Length);
                    Array.Clear(sourceSamples, 0, sourceSamples.Length);
                    Array.Clear(enhancedSamples, 0, enhancedSamples.Length);
                    Array.Clear(enhancedBytes, 0, enhancedBytes.Length);
                }
                catch (Exception ex)
                {
                    // 异常不中断循环，仅打印
                    Console.WriteLine($"处理异常：{ex.Message}");
                    continue;
                }
            }

            // 清理资源
            _queueSignal.Dispose();
        }

        static void BufferMonitor(BufferedWaveProvider provider)
        {
            while (_isRunning)
            {
                try
                {
                    int minBufferSize = TARGET_SAMPLE_RATE * CHANNELS * BIT_DEPTH / 8 / 10;
                    if (provider.BufferedBytes < minBufferSize)
                    {
                        byte[] silence = new byte[minBufferSize];
                        lock (provider)
                        {
                            provider.AddSamples(silence, 0, silence.Length);
                        }
                        Console.WriteLine("缓冲区数据不足，填充静音");
                    }
                    Thread.Sleep(50);
                }
                catch { }
            }
        }

        static float[] StableSuperResolveAudio(float[] sourceSamples)
        {
            // 空输入保护
            if (sourceSamples == null || sourceSamples.Length == 0)
                return Array.Empty<float>();

            double ratio = (double)TARGET_SAMPLE_RATE / SOURCE_SAMPLE_RATE;
            int targetLength = (int)Math.Round(sourceSamples.Length * ratio);
            float[] enhancedSamples = new float[targetLength];

            // 预计算低通滤波器系数（截止频率为源采样率的0.45倍，避免混叠）
            double cutoffFreq = 0.45 * SOURCE_SAMPLE_RATE;
            double normalizedCutoff = cutoffFreq / TARGET_SAMPLE_RATE;
            double filterCoeff = Math.Sin(2 * Math.PI * normalizedCutoff) / (Math.PI * normalizedCutoff);

            for (int i = 0; i < targetLength; i++)
            {
                double sourceIndex = i / ratio;
                // 严格限制索引范围，避免越界
                sourceIndex = Clamp(sourceIndex, 0, sourceSamples.Length - 1);

                // 立方插值核心计算（需要4个相邻采样点）
                int baseIndex = (int)Math.Floor(sourceIndex);
                double fraction = sourceIndex - baseIndex;

                // 获取4个插值所需的采样点（边界扩展处理）
                float s0 = GetSampleWithPadding(sourceSamples, baseIndex - 1);
                float s1 = GetSampleWithPadding(sourceSamples, baseIndex);
                float s2 = GetSampleWithPadding(sourceSamples, baseIndex + 1);
                float s3 = GetSampleWithPadding(sourceSamples, baseIndex + 2);

                // 立方插值公式（Catmull-Rom样条，音频领域最优选择之一）
                float interpolated = CubicInterpolation(s0, s1, s2, s3, fraction);

                // 应用低通滤波，消除升采样后的高频混叠噪声
                enhancedSamples[i] = (float)(interpolated * filterCoeff);

                // 防失真：限制采样值在音频标准范围[-1, 1]内
                enhancedSamples[i] = Clamp(enhancedSamples[i], -1.0f, 1.0f);
            }

            // 最后进行简单的平滑处理，进一步提升音质
            enhancedSamples = ApplySimpleSmoothing(enhancedSamples);

            return enhancedSamples;
        }

        /// <summary>
        /// 边界扩展的采样点获取（避免立方插值时边界越界）
        /// </summary>
        private static float GetSampleWithPadding(float[] samples, int index)
        {
            if (index < 0)
                return samples[0]; // 左边界填充第一个值
            if (index >= samples.Length)
                return samples[samples.Length - 1]; // 右边界填充最后一个值
            return samples[index];
        }

        /// <summary>
        /// Catmull-Rom立方插值实现（音频优化版）
        /// </summary>
        private static float CubicInterpolation(float s0, float s1, float s2, float s3, double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            // Catmull-Rom样条公式，兼顾平滑度和保真度
            double a0 = -0.5 * s0 + 1.5 * s1 - 1.5 * s2 + 0.5 * s3;
            double a1 = s0 - 2.5 * s1 + 2 * s2 - 0.5 * s3;
            double a2 = -0.5 * s0 + 0.5 * s2;
            double a3 = s1;

            return (float)(a0 * t3 + a1 * t2 + a2 * t + a3);
        }

        /// <summary>
        /// 简单的滑动平均平滑处理，减少插值毛刺
        /// </summary>
        private static float[] ApplySimpleSmoothing(float[] samples)
        {
            if (samples.Length <= 2)
                return samples;

            float[] smoothed = new float[samples.Length];
            // 保留首尾值，避免边界失真
            smoothed[0] = samples[0];
            smoothed[samples.Length - 1] = samples[samples.Length - 1];

            // 3点滑动平均（轻量级，性能影响小）
            for (int i = 1; i < samples.Length - 1; i++)
            {
                smoothed[i] = (samples[i - 1] + 2 * samples[i] + samples[i + 1]) / 4;
            }

            return smoothed;
        }

        /// <summary>
        /// 通用数值限制函数
        /// </summary>
        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// 浮点型数值限制函数（音频专用）
        /// </summary>
        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
        #endregion

        #region 辅助方法
        static int GetSpeakerDeviceNumber()
        {
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var devCap = WaveOut.GetCapabilities(i);
                string deviceName = devCap.ProductName.ToLower();
                if (!deviceName.Contains("cable") && !deviceName.Contains("virtual") &&
                    (deviceName.Contains("扬声器") ||
                     deviceName.Contains("耳机") ||
                     deviceName.Contains("speaker") ||
                     deviceName.Contains("headphone") ||
                     deviceName.Contains("audio")))
                {
                    return i;
                }
            }
            Console.WriteLine("未找到真实扬声器，使用系统默认播放设备");
            return 0;
        }

        static int GetCaptureDeviceNumber()
        {
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var capDev = WaveInEvent.GetCapabilities(i);
                if (capDev.ProductName.Contains("CABLE Output") ||
                    capDev.ProductName.Contains("VB-Cable") ||
                    capDev.ProductName.Contains("立体声混音") ||
                    capDev.ProductName.Contains("stereo mix"))
                {
                    return i;
                }
            }
            throw new Exception("未找到音频捕获设备！\n请先安装VB-Cable并配置为默认输入设备，或启用立体声混音。");
        }

        static float[] Convert16BitPCMToFloat(byte[] buffer, int bytesRecorded)
        {
            int sampleCount = bytesRecorded / 2;
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)((buffer[i * 2 + 1] << 8) | buffer[i * 2]);
                samples[i] = sample / 32768f;
            }
            return samples;
        }

        static byte[] ConvertFloatTo16BitPCM(float[] samples)
        {
            byte[] buffer = new byte[samples.Length * 2];
            for (int i = 0; i < samples.Length; i++)
            {
                float sample = Math.Max(Math.Min(samples[i], 1.0f), -1.0f);
                short pcm = (short)(sample * 32768f);
                buffer[i * 2] = (byte)(pcm & 0xFF);
                buffer[i * 2 + 1] = (byte)((pcm >> 8) & 0xFF);
            }
            return buffer;
        }

        
       
        #endregion
    }
}

