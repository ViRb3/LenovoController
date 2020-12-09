using System;
using LenovoController.Providers;

namespace LenovoController.Features
{
    public enum BatteryState
    {
        Conservation,
        Normal,
        RapidCharge,
        Status
    }

    public class BatteryFeature : IFeature<BatteryState>
    {
        private BatteryState _lastState;

        public BatteryState GetState()
        {
            DriverProvider.SendCode(DriverProvider.EnergyDriver, 0x831020F8,
                ToInternal(BatteryState.Status)[0], out var result);
            var state = FromInternal(result);
            _lastState = state;
            return state;
        }

        public void SetState(BatteryState state)
        {
            foreach (var code in ToInternal(state))
                DriverProvider.SendCode(DriverProvider.EnergyDriver, 0x831020F8,
                    code, out _);
            _lastState = state;
        }

        private byte[] ToInternal(BatteryState state)
        {
            switch (state)
            {
                case BatteryState.Conservation:
                    if (_lastState == BatteryState.RapidCharge)
                        return new byte[] {0x8, 0x3};
                    else
                        return new byte[] {0x3};
                case BatteryState.Normal:
                    if (_lastState == BatteryState.Conservation)
                        return new byte[] {0x5};
                    else
                        return new byte[] {0x8};
                case BatteryState.RapidCharge:
                    if (_lastState == BatteryState.Conservation)
                        return new byte[] {0x5, 0x7};
                    else
                        return new byte[] {0x7};
                case BatteryState.Status:
                    return new byte[] {0xFF};
                default:
                    throw new Exception("Invalid state");
            }
        }

        private static BatteryState FromInternal(uint state)
        {
            var bytes = BitConverter.GetBytes(state);
            Array.Reverse(bytes, 0, bytes.Length);
            state = BitConverter.ToUInt32(bytes, 0);

            if (Util.GetNthBit(state, 17)) // is charging?
            {
                if (Util.GetNthBit(state, 26))
                {
                    return BatteryState.RapidCharge;
                }

                return BatteryState.Normal;
            }

            if (Util.GetNthBit(state, 29))
                return BatteryState.Conservation;

            throw new Exception("Unknown battery state: " + state);
        }
    }
}