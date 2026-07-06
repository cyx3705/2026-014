using System.Runtime.InteropServices;

namespace b_Code_SE.Interop;

/// <summary>
/// .NET Core 不包含 Marshal.GetActiveObject，通过 oleaut32 实现同等功能。
/// </summary>
internal static class ComHelper
{
    public static object GetActiveObject(string progId)
    {
        Type? type = Type.GetTypeFromProgID(progId)
            ?? throw new COMException($"未找到 ProgID: {progId}");

        Guid clsid = type.GUID;
        GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
        return obj;
    }

    [DllImport("oleaut32.dll", PreserveSig = false)]
    private static extern void GetActiveObject(ref Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
}