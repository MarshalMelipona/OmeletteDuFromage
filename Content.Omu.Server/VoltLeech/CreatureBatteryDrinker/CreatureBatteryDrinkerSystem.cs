using Content.Omu.Shared.VoltLeech;
using Content.Server.Emp;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._EinsteinEngines.Silicon.Components;
using Content.Shared._Shitmed.Targeting;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Utility;

namespace Content.Omu.Server.VoltLeech.CreatureBatteryDrinker;

public sealed partial class CreatureBatteryDrinkerSystem : EntitySystem
{
    [Dependency] private readonly MobStateSystem _mobState = null!;
    [Dependency] private readonly EmpSystem _emp = null!;
    [Dependency] private readonly DamageableSystem _damage = null!;
    [Dependency] private readonly SharedMindSystem _mind = null!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = null!;
    [Dependency] private readonly BatterySystem _battery = null!;
    [Dependency] private readonly SharedPopupSystem _popup = null!;
    [Dependency] private readonly SharedAudioSystem _audio = null!;
    [Dependency] private readonly SharedStaminaSystem _stamina = null!;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CreatureBatteryDrinkerComponent, GetVerbsEvent<InnateVerb>>(OnGetVerbs);
        SubscribeLocalEvent<CreatureBatteryDrinkerComponent, CreatureBatteryDrinkerDoAfterEvent>(OnDoAfter);
    }

    private void OnGetVerbs(Entity<CreatureBatteryDrinkerComponent> drinker, ref GetVerbsEvent<InnateVerb> args)
    {
        var target = args.Target;

        if (!args.CanAccess
            || !args.CanInteract
            || !CanDrinkEntity(target)
            || target == drinker.Owner)
            return;

        InnateVerb verb = new()
        {
            Act = () => DrinkCreature(drinker, target),
            Text = Loc.GetString("creature-battery-drinker-verb-drink"),
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png")),
            Priority = 2,
        };

        args.Verbs.Add(verb);
    }

    private void DrinkCreature(Entity<CreatureBatteryDrinkerComponent> drinker, EntityUid target)
    {
        if (!CanDrinkEntity(target))
            return;

        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            drinker,
            drinker.Comp.DrinkSpeed,
            new CreatureBatteryDrinkerDoAfterEvent(),
            drinker,
            target)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BlockDuplicate = true,
            BreakOnHandChange = true,
            RequireCanInteract = true,
            NeedHand = true,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);

        var popup = Loc.GetString("creature-battery-drinker-start-drink", ("target", target), ("drinker", drinker));
        _popup.PopupEntity(popup, target, PopupType.MediumCaution);
    }

    private void OnDoAfter(Entity<CreatureBatteryDrinkerComponent> drinker, ref CreatureBatteryDrinkerDoAfterEvent args)
    {
        if (args.Cancelled
            || args.Handled
            || args.Target is not { } target
            || !CanDrinkEntity(target))
            return;

        args.Repeat = !_battery.IsFull(drinker);

        if (!args.Repeat)
        {
            var popup = Loc.GetString("creature-battery-drinker-end-drink", ("target", target), ("drinker", drinker));
            _popup.PopupEntity(popup, target, PopupType.MediumCaution);
        }
        else
        {
            var popup = Loc.GetString("creature-battery-drinker-continue-drink", ("target", target));
            _popup.PopupEntity(popup, target, PopupType.MediumCaution);
        }

        // Target is a robot, EMP and deal less damage.
        if (TryComp<BatteryComponent>(target, out var battery))
        {
            if (battery.CurrentCharge <= 0)
                return;

            _emp.TryEmpEffects(target, drinker.Comp.EnergyGained, drinker.Comp.EmpDuration);
            _damage.TryChangeDamage(target, drinker.Comp.DamageOnDrain / 2, true, targetPart: TargetBodyPart.Chest);
        }
        else // Not a robot, but alive and has a mind, so just burn the shizz outta it
        {
            _damage.TryChangeDamage(target, drinker.Comp.DamageOnDrain, true, targetPart: TargetBodyPart.Chest);
        }

        _battery.AddCharge(drinker, drinker.Comp.EnergyGained); // temp value
        _stamina.TakeStaminaDamage(target, drinker.Comp.StaminaDamageOnDrain);
        _audio.PlayPvs(drinker.Comp.DrinkSound, target);
    }

    private bool CanDrinkEntity(EntityUid target)
    {
        // Dead or no mind? Nope.
        if (_mobState.IsDead(target) /*
            || !_mind.TryGetMind(target, out _, out _)*/) // mind checks disabled for testing
            return false;

        // Robot dead? Nope.
        return !TryComp<SiliconComponent>(target, out var clanker)
               || !clanker.Dead;
    }


}
