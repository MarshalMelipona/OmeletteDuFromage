using Content.Omu.Server.VoltLeech.Roles;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Roles;
using Content.Server.Store.Systems;
using Content.Shared._EinsteinEngines.Silicon.Components;
using Content.Shared.Mind;
using Content.Shared.Roles;

namespace Content.Omu.Server.VoltLeech.GameTicking.Rules;

public sealed partial class VoltLeechRuleSystem : GameRuleSystem<VoltLeechRuleComponent>
{
    [Dependency] private readonly StoreSystem _store = null!;
    [Dependency] private readonly SharedMindSystem _mind = null!;
    [Dependency] private readonly SharedRoleSystem _role = null!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoltLeechRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
        SubscribeLocalEvent<VoltLeechRoleComponent, GetBriefingEvent>(OnGetBrief);
    }

    private void OnSelectAntag(EntityUid uid, VoltLeechRuleComponent comp, ref AfterAntagEntitySelectedEvent args)
    {
        TryMakeVoltLeech(args.EntityUid, comp);
    }

    public bool TryMakeVoltLeech(EntityUid target, VoltLeechRuleComponent rule)
    {
        if (HasComp<SiliconComponent>(target)
            || !_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        var leechComp = EnsureComp<VoltLeechComponent>(target);

        var briefing = MakeBriefing();
        _antag.SendBriefing(target, briefing, Color.CornflowerBlue, rule.BriefingSound);

        return true;
    }

    private void OnGetBrief(Entity<VoltLeechRoleComponent> role, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;

        if (ent is null)
            return;

        args.Append(MakeBriefing());
    }

    private string MakeBriefing()
    {
        return Loc.GetString("volt-leech-role-greeting");
    }
}
