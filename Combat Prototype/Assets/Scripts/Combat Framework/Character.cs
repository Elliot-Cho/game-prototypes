using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : GenericUnit {

    public CharacterClass Class;

    // Abilities that are in the character's ability bar
    [HideInInspector]
    public List<Ability> AbilityBar;

    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();

        this.Abilities = Class.Abilities;

        // UPDATE ME: Used for test, replace with appropriate abilitybar algo later
        AbilityBar = this.Abilities;
    }
}
