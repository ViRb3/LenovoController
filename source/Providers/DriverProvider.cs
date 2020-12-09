using System;
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
    }
}