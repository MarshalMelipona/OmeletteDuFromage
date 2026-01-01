namespace Content.Omu.Server.ArcDemon;

[RegisterComponent]
public sealed partial class ArcDemonComponent : Component
{
    [DataField]
    public int StartingMaxBatteryCharge = 3000;
}
