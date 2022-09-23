namespace LenovoController.Features
{
    public enum ExtremeCoolingState
    {
        Off,
        On
    }

    public class ExtremeCoolingFeature : AbstractWmiFeature<ExtremeCoolingState>
    {
        public ExtremeCoolingFeature() : base("GetFanCoolingStatus", "SetFanCooling", 0)
        {
        }
    }
}