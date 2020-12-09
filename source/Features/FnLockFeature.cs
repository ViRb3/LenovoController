using System;
using LenovoController.Providers;

namespace LenovoController.Features
{
    public enum FnLockState
    {
        Off,
        On,
        Status
    }

    public class FnLockFeature : IFeature<FnLockState>
    {
        public FnLockState GetState()
        {
            DriverProvider.SendCode(DriverProvider.EnergyDriver, 0X831020E8,
                ToInternal(FnLockState.Status)[0], out var result);
            return FromInternal(result);
        }

        public void SetState(FnLockState state)
        {
            var codes = ToInternal(state);
            foreach (var code in codes)
                DriverProvider.SendCode(DriverProvider.EnergyDriver, 0X831020E8,
                    code, out _);
        }

        private byte[] ToInternal(FnLockState state)
        {
            switch (state)
            {
                case FnLockState.Off:
                    return new byte[] {0xF};
                case FnLockState.On:
                    return new byte[] {0xA, 0xE};
                case FnLockState.Status:
                    return new byte[] {0x2};
                default:
                    throw new Exception("Invalid state");
            }
        }

        private static FnLockState FromInternal(uint state)
        {
            var bytes = BitConverter.GetBytes(state);
            Array.Reverse(bytes, 0, bytes.Length);
            state = BitConverter.ToUInt32(bytes, 0);

            if (Util.GetNthBit(state, 18))
                return FnLockState.On;
            return FnLockState.Off;
        }
    }
}