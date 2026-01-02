using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Omu.Shared.VoltLeech;

[Serializable, NetSerializable]
public sealed partial class CreatureBatteryDrinkerDoAfterEvent : SimpleDoAfterEvent;
