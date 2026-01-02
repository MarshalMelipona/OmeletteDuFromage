using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Omu.Server.VoltLeech.CreatureBatteryDrinker;

/// <summary>
/// It's named this because "CreatureDrinkerComponent" feels wrong.
/// </summary>
[RegisterComponent]
public sealed partial class CreatureBatteryDrinkerComponent : Component
{
    /// <summary>
    ///     How long it takes to drink from a battery, in seconds.
    ///     Is multiplied by the source.
    /// </summary>
    [DataField]
    public TimeSpan DrinkSpeed = TimeSpan.FromSeconds(1.5f);

    /// <summary>
    /// The power gained from absorbing a sentient entity is equal to this number.
    /// </summary>
    [DataField]
    public float EnergyGained = 100f;

    /// <summary>
    /// How much damage is dealt to a creature each tick they are drained.
    /// Cyborgs take half this damage.
    /// </summary>
    [DataField]
    public DamageSpecifier DamageOnDrain = new()
    {
        DamageDict =
        {
            ["Heat"] = 5,
        },
    };

    /// <summary>
    /// How much stamina damage is dealt to a creature each tick they are drained?
    /// </summary>
    [DataField]
    public int StaminaDamageOnDrain = 20;

    [DataField]
    public float EmpDuration = 1f;

    /// <summary>
    ///     The sound to play when drinking
    /// </summary>
    [DataField]
    public SoundSpecifier? DrinkSound = new SoundCollectionSpecifier("sparks");
}
