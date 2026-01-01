using Content.Omu.Server.ArcDemon.CreatureBatteryDrinker;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._EinsteinEngines.Power.Components;

namespace Content.Omu.Server.ArcDemon;

public sealed partial class ArcDemonSystem : EntitySystem
{
    [Dependency] private readonly BatterySystem _battery = null!;
    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArcDemonComponent, MapInitEvent>(OnInitialize);
    }

    private void OnInitialize(Entity<ArcDemonComponent> arcDemon, ref MapInitEvent args)
    {
        EnsureComp<BatteryDrinkerComponent>(arcDemon);
        EnsureComp<BatteryComponent>(arcDemon);
        EnsureComp<CreatureBatteryDrinkerComponent>(arcDemon);

        _battery.SetMaxCharge(arcDemon, arcDemon.Comp.StartingMaxBatteryCharge);
    }
}
