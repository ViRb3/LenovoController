namespace LenovoController.Features
{
    public enum FanProfileState
    {
        Quiet,
        Balance,
        Performance
    }

    public class FanProfileFeature : AbstractWmiFeature<FanProfileState>
    {
        public FanProfileFeature() : base("SmartFanMode", 1)
        {
        }
    }
}