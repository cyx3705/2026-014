using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using b_Code_SE.Interop;
using b_Code_SE.Services;

namespace b_Code_SE;

public partial class MainWindow : Window
{
    private readonly SolidEdgeMonitorService _monitor = new();
    private readonly SeWindowEmbedder _embedder;
    private readonly ObservableCollection<LogItemViewModel> _logs = [];

    public MainWindow()
    {
        InitializeComponent();
        _embedder = new SeWindowEmbedder(this);
        LogList.ItemsSource = _logs;

        _monitor.LogReceived += OnLogReceived;
        _monitor.StatusChanged += OnStatusChanged;
    }

    private void BtnConnect_Click(object sender, RoutedEventArgs e)
        => RunSafe(() => _monitor.ConnectToRunningInstance());

    private void BtnStart_Click(object sender, RoutedEventArgs e)
        => RunSafe(() => _monitor.StartAndConnect());

    private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        => RunSafe(() =>
        {
            _embedder.Detach();
            _monitor.Disconnect();
        });

    private void BtnEmbed_Click(object sender, RoutedEventArgs e)
        => RunSafe(() =>
        {
            if (_monitor.Application == null)
            {
                throw new InvalidOperationException("请先连接 Solid Edge。");
            }

            _embedder.Embed(_monitor.Application);
            UpdateButtons();
        });

    private void BtnDetach_Click(object sender, RoutedEventArgs e)
        => RunSafe(() =>
        {
            _embedder.Detach();
            UpdateButtons();
        });

    private void BtnClear_Click(object sender, RoutedEventArgs e)
        => _logs.Clear();

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _embedder.Dispose();
        _monitor.Dispose();
    }

    private void RunSafe(Action action)
    {
        try
        {
            action();
            UpdateButtons();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "操作失败", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OnLogReceived(SeLogEntry entry)
    {
        Dispatcher.Invoke(() =>
        {
            _logs.Add(new LogItemViewModel(entry));
            if (ChkAutoScroll.IsChecked == true && _logs.Count > 0)
            {
                LogList.ScrollIntoView(_logs[^1]);
            }
        });
    }

    private void OnStatusChanged(string status)
    {
        Dispatcher.Invoke(() =>
        {
            StatusText.Text = status;
            StatusDot.Fill = _monitor.IsConnected
                ? new SolidColorBrush(Color.FromRgb(0x44, 0xAA, 0x44))
                : new SolidColorBrush(Color.FromRgb(0xCC, 0x44, 0x44));
            UpdateButtons();
        });
    }

    private void UpdateButtons()
    {
        bool connected = _monitor.IsConnected;
        BtnConnect.IsEnabled = !connected;
        BtnStart.IsEnabled = !connected;
        BtnDisconnect.IsEnabled = connected;
        BtnEmbed.IsEnabled = connected && !_embedder.IsEmbedded;
        BtnDetach.IsEnabled = connected && _embedder.IsEmbedded;
    }

    private sealed class LogItemViewModel
    {
        public LogItemViewModel(SeLogEntry entry)
        {
            TimeText = entry.Time.ToString("HH:mm:ss.fff");
            Category = entry.Category;
            Message = entry.Message;
        }

        public string TimeText { get; }
        public string Category { get; }
        public string Message { get; }
    }
}