using Content.Omu.Server.VoltLeech.CreatureBatteryDrinker;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._EinsteinEngines.Power.Components;
using Content.Shared.Alert;
using Content.Shared.Whitelist;

namespace Content.Omu.Server.VoltLeech;

public sealed partial class VoltLeechSystem : EntitySystem
{
    [Dependency] private readonly BatterySystem _battery = null!;
    [Dependency] private readonly AlertsSystem _alerts = null!;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoltLeechComponent, MapInitEvent>(OnInitialize);
        SubscribeLocalEvent<VoltLeechComponent, ChargeChangedEvent>(OnChargeChanged);
    }

    private void OnInitialize(Entity<VoltLeechComponent> voltLeech, ref MapInitEvent args)
    {
        var batteryDrinker = EnsureComp<BatteryDrinkerComponent>(voltLeech);
        batteryDrinker.Blacklist = new EntityWhitelist
        {
            Components = ["PowerCellSlot"],
        };

        batteryDrinker.DrinkMultiplier = 0.1f;

        EnsureComp<BatteryComponent>(voltLeech);
        _battery.SetMaxCharge(voltLeech, voltLeech.Comp.StartingMaxBatteryCharge);

        EnsureComp<CreatureBatteryDrinkerComponent>(voltLeech);
        UpdateBatteryAlert(voltLeech);
    }

    private void OnChargeChanged(Entity<VoltLeechComponent> voltLeech, ref ChargeChangedEvent args)
    {
        UpdateBatteryAlert(voltLeech);
    }

    private void UpdateBatteryAlert(Entity<VoltLeechComponent> voltLeech, BatteryComponent? battery = null)
    {
        if (!Resolve(voltLeech, ref battery))
            return;

        var chargePercent = (short) MathF.Round(battery.CurrentCharge / battery.MaxCharge * 10f);

        _alerts.ShowAlert(voltLeech, voltLeech.Comp.BatteryAlert, chargePercent);
    }
}
