using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server.VoltLeech.GameTicking.Rules;

[RegisterComponent]
public sealed partial class VoltLeechRuleComponent : Component
{
    [DataField]
    public SoundPathSpecifier BriefingSound = new("/Audio/_Goobstation/Ambience/Antag/devil_start.ogg"); // Change this later
}
