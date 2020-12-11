using System;

namespace Wayfinder {
  /// <summary>
  /// Primary unit attributes
  /// </summary>
  [Serializable]
  public class Attributes {
    public CharacterStat strength = new CharacterStat();          // Affects meleeDamage and resist
    public CharacterStat dexterity = new CharacterStat();         // Affects rangedDamage, accuracy, and critChance
    public CharacterStat speed = new CharacterStat();             // Affects initiative, evasion
    public CharacterStat constitution = new CharacterStat();      // Affects hitPoints, defense
    public CharacterStat aptitude = new CharacterStat();          // Affects magicDamage, skillPower, skillCharge
  }

  /// <summary>
  /// Secondary unit combat stats
  /// </summary>
  [Serializable]
  public class CombatStats {
    public CharacterStat maxActionPoints = new CharacterStat();   // How many actions unit can perform in a turn
    public CharacterStat meleeDamage = new CharacterStat();       // How much damage unit deals with melee attacks
    public CharacterStat rangedDamage = new CharacterStat();      // How much damage unit deals with ranged attacks
    public CharacterStat magicDamage = new CharacterStat();       // How much damage unit deals with magic attacks
    public CharacterStat defense = new CharacterStat();           // How much damage this unit absorbs
    public CharacterStat initiative = new CharacterStat();        // How early this unit moves in the turn order
    public CharacterStat accuracy = new CharacterStat();          // How well this unit can connect its attacks
    public CharacterStat evasion = new CharacterStat();           // How well this unit can avoid attacks
    public CharacterStat resist = new CharacterStat();            // How well this unit resists skill effects
    public CharacterStat skillPower = new CharacterStat();        // How effective this unit's skills are
    public CharacterStat skillCharge = new CharacterStat();       // How fast this unit's skills recharge
    public CharacterStat critChance = new CharacterStat();        // Chance of this unit landing a critical hit
  }

  /// <summary>
  /// Primary talent attributes
  /// </summary>
  [Serializable]
  public class Talents {
    public CharacterStat exploration = new CharacterStat();       // Affects travelSpeed & regionClearSpeed
    public CharacterStat crafting = new CharacterStat();          // Affects craftingQuality & craftingSpeed
    public CharacterStat smithing = new CharacterStat();          // Affects smithingQuality & smithingSpeed
    public CharacterStat thaumaturgy = new CharacterStat();       // Affects thaumQuality & thaumSpeed
  }

  /// <summary>
  /// Secondary talent stats
  /// </summary>
  [Serializable]
  public class TalentStats {
    public CharacterStat travelSpeed = new CharacterStat();       // How fast this unit can travel between overworld regions
    public CharacterStat regionClearSpeed = new CharacterStat();  // How fast this NPC unit can clear regions
    public CharacterStat craftingQuality = new CharacterStat();   // Quality of crafted items for certain applicable items
    public CharacterStat craftingSpeed = new CharacterStat();     // Crafting speed for NPC crafters
    public CharacterStat buildingSpeed = new CharacterStat();     // Building speed for NPC builders
    public CharacterStat smithingQuality = new CharacterStat();   // Quality of smithed items for certain applicable items
    public CharacterStat smithingSpeed = new CharacterStat();     // Smithing speed for NPC smiths
    public CharacterStat thaumQuality = new CharacterStat();      // Quality of magic items for certain applicable items
    public CharacterStat thaumSpeed = new CharacterStat();        // Fabrication speed for NPC thaumaturgists
  }
}
