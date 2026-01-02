using Content.Omu.Server.VoltLeech.CreatureBatteryDrinker;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._EinsteinEngines.Power.Components;
using Content.Shared.Whitelist;

namespace Content.Omu.Server.VoltLeech;

public sealed partial class VoltLeechSystem : EntitySystem
{
    [Dependency] private readonly BatterySystem _battery = null!;
    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoltLeechComponent, MapInitEvent>(OnInitialize);
    }

    private void OnInitialize(Entity<VoltLeechComponent> arcDemon, ref MapInitEvent args)
    {
        var batteryDrinker = EnsureComp<BatteryDrinkerComponent>(arcDemon);
        batteryDrinker.Blacklist = new EntityWhitelist
        {
            Components = ["PowerCellSlot"],
        };

        EnsureComp<BatteryComponent>(arcDemon);
        _battery.SetMaxCharge(arcDemon, arcDemon.Comp.StartingMaxBatteryCharge);

        EnsureComp<CreatureBatteryDrinkerComponent>(arcDemon);
    }
}
