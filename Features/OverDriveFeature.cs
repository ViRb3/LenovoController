namespace LenovoController.Features
{
    public enum OverDriveState
    {
        Off,
        On
    }

    public class OverDriveFeature : AbstractWmiFeature<OverDriveState>
    {
        public OverDriveFeature() : base("ODStatus", 0)
        {
        }
    }
}