using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LenovoController.Providers
{
    public static class DriverProvider
    {
        private static SafeFileHandle _energyDriver;

        public static SafeFileHandle EnergyDriver
        {
            get
            {
                if (_energyDriver == null)
                {
                    var fileHandle = Native.CreateFileW("\\\\.\\EnergyDrv", 0xC0000000,
                        3u, IntPtr.Zero, 3u, 0x80, IntPtr.Zero);
                    if (fileHandle == new IntPtr(-1))
                        throw new Exception("fileHandle is 0");
                    _energyDriver = new SafeFileHandle(fileHandle, true);
                }

                return _energyDriver;
            }
        }

        public static int SendCode(SafeFileHandle handle, uint controlCode, byte inBuffer, out uint outBuffer)
        {
            if (!Native.DeviceIoControl(handle, controlCode, ref inBuffer, sizeof(byte),
                out outBuffer, sizeof(uint), out var bytesReturned, IntPtr.Zero)
            )
                throw new Exception("DeviceIoControl returned 0, last error: " + Marshal.GetLastWin32Error());
            return bytesReturned;
        }
    }
}