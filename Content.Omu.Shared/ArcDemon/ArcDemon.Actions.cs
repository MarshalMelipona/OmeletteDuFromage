using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Omu.Shared.ArcDemon;

[Serializable, NetSerializable]
public sealed partial class CreatureBatteryDrinkerDoAfterEvent : SimpleDoAfterEvent;
