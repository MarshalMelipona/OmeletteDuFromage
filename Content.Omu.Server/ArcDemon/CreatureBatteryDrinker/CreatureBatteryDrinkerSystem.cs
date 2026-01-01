using Content.Omu.Shared.ArcDemon;
using Content.Server.Emp;
using Content.Shared._EinsteinEngines.Silicon.Components;
using Content.Shared._Shitmed.Targeting;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Emp;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Verbs;

namespace Content.Omu.Server.ArcDemon.CreatureBatteryDrinker;

public sealed partial class CreatureBatteryDrinkerSystem : EntitySystem
{
    [Dependency] private readonly MobStateSystem _mobState = null!;
    [Dependency] private readonly EmpSystem _emp = null!;
    [Dependency] private readonly DamageableSystem _damage = null!;
    [Dependency] private readonly SharedMindSystem _mind = null!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = null!;

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
            || _mobState.IsDead(target)
            || !_mind.TryGetMind(target, out _, out _))
            return;


    }

    private void DrinkCreature(EntityUid target, Entity<CreatureBatteryDrinkerComponent> drinker)
    {
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
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnDoAfter(Entity<CreatureBatteryDrinkerComponent> drinker, ref CreatureBatteryDrinkerDoAfterEvent args)
    {
        if (args.Cancelled
            || args.Handled
            || args.Target is not { } target)
            return;

        // Target is a robot, EMP and deal less damage.
        if (TryComp<SiliconComponent>(target, out var silicon)
            && !silicon.Dead)
        {
            _emp.TryEmpEffects(target, 0, drinker.Comp.EmpDuration);
            _damage.TryChangeDamage(target, drinker.Comp.DamageOnDrain / 2, true, targetPart: TargetBodyPart.Chest);
        }
        else // Not a robot, but alive and has a mind, so just burn the shizz outta it
        {
            _damage.TryChangeDamage(target, drinker.Comp.DamageOnDrain, true, targetPart: TargetBodyPart.Chest);
        }
    }


}
