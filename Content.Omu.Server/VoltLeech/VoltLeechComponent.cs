using Content.Shared.Alert;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server.VoltLeech;

[RegisterComponent]
public sealed partial class VoltLeechComponent : Component
{
    [DataField]
    public int StartingMaxBatteryCharge = 3000;

    [DataField]
    public ProtoId<AlertPrototype> BatteryAlert = "BorgBattery";
}
