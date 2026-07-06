using System.Runtime.InteropServices;

namespace b_Code_SE.Interop;

/// <summary>
/// 处理 Solid Edge COM 互操作时的 RPC_E_CALL_REJECTED / RPC_E_SERVERCALL_RETRYLATER 错误。
/// 参考 sesdk.chm → OleMessageFilterUsage.html
/// </summary>
internal sealed class OleMessageFilter : IOleMessageFilterNative
{
    public static void Register()
    {
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            throw new COMException("当前线程必须为 STA 才能注册 OleMessageFilter。");
        }

        IOleMessageFilterNative newFilter = new OleMessageFilter();
        CoRegisterMessageFilter(newFilter, out _);
    }

    public static void Revoke()
    {
        CoRegisterMessageFilter(null, out _);
    }

    int IOleMessageFilterNative.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
        => (int)ServerCall.ServerCallIsHandled;

    int IOleMessageFilterNative.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        => dwRejectType == (int)ServerCall.ServerCallRetryLater ? 99 : -1;

    int IOleMessageFilterNative.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        => (int)PendingMsg.PendingMsgWaitDefProcess;

    [DllImport("Ole32.dll")]
    private static extern int CoRegisterMessageFilter(IOleMessageFilterNative? newFilter, out IOleMessageFilterNative oldFilter);

    private enum ServerCall
    {
        ServerCallIsHandled = 0,
        ServerCallRejected = 1,
        ServerCallRetryLater = 2
    }

    private enum PendingMsg
    {
        PendingMsgCancelCall = 0,
        PendingMsgWaitNoProcess = 1,
        PendingMsgWaitDefProcess = 2
    }
}

[ComImport]
[Guid("00000016-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleMessageFilterNative
{
    [PreserveSig]
    int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

    [PreserveSig]
    int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

    [PreserveSig]
    int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
}