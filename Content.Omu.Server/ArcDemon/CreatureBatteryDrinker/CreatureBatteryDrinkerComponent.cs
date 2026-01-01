using Content.Shared.Damage;

namespace Content.Omu.Server.ArcDemon.CreatureBatteryDrinker;

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
    ///     The multiplier for the amount of power to attempt to drink.
    ///     Default amount is 1000
    /// </summary>
    [DataField]
    public float DrinkMultiplier = 5f;

    /// <summary>
    /// How much damage is dealt to a creature each tick they are drained.
    /// Cyborgs take half this damage.
    /// </summary>
    [DataField]
    public DamageSpecifier DamageOnDrain = new()
    {
        DamageDict =
        {
            ["Burn"] = 5,
        },
    };

    /// <summary>
    /// How much stamina damage is dealt to a creature each tick they are drained?
    /// </summary>
    [DataField]
    public int StaminaDamageOnDrain = 20;

    [DataField]
    public float EmpDuration = 1f;
}
