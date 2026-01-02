namespace Content.Omu.Server.VoltLeech;

[RegisterComponent]
public sealed partial class VoltLeechComponent : Component
{
    [DataField]
    public int StartingMaxBatteryCharge = 3000;
}
