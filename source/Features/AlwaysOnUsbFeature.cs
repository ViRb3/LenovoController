using System;
using LenovoController.Providers;

namespace LenovoController.Features
{
    public enum AlwaysOnUsbState
    {
        Off,
        OnWhenSleeping,
        OnAlways,
        Status
    }

    public class AlwaysOnUsbFeature : IFeature<AlwaysOnUsbState>
    {
        public AlwaysOnUsbState GetState()
        {
            DriverProvider.SendCode(DriverProvider.EnergyDriver, 0X831020E8,
                ToInternal(AlwaysOnUsbState.Status)[0], out var result);
            return FromInternal(result);
        }

        public void SetState(AlwaysOnUsbState state)
        {
            var codes = ToInternal(state);
            foreach (var code in codes)
                DriverProvider.SendCode(DriverProvider.EnergyDriver, 0X831020E8,
                    code, out _);
        }

        private byte[] ToInternal(AlwaysOnUsbState state)
        {
            switch (state)
            {
                case AlwaysOnUsbState.Off:
                    return new byte[] {0xB, 0x12};
                case AlwaysOnUsbState.OnWhenSleeping:
                    return new byte[] {0xA, 0x12};
                case AlwaysOnUsbState.OnAlways:
                    return new byte[] {0xA, 0x13};
                case AlwaysOnUsbState.Status:
                    return new byte[] {0x2};
                default:
                    throw new Exception("Invalid state");
            }
        }

        private static AlwaysOnUsbState FromInternal(uint state)
        {
            var bytes = BitConverter.GetBytes(state);
            Array.Reverse(bytes, 0, bytes.Length);
            state = BitConverter.ToUInt32(bytes, 0);

            if (Util.GetNthBit(state, 31)) // is on?
            {
                if (Util.GetNthBit(state, 23))
                    return AlwaysOnUsbState.OnAlways;
                return AlwaysOnUsbState.OnWhenSleeping;
            }

            return AlwaysOnUsbState.Off;
        }
    }
}