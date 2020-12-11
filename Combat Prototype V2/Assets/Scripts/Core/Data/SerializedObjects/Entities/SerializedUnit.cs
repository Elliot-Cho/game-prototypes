using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
[XmlRoot("unit")]
public class SerializedUnit {

  [XmlAttribute("id")]
  public string id = "";

  [XmlElement("name")]
  public string name = "";

  [XmlElement("level")]
  public int level = 1;

  [XmlElement("properties")]
  public UnitProperties properties = new UnitProperties();

  [System.Serializable]
  public class UnitProperties
  {
    [XmlElement("gridSizex")]
    public int gridSizex = 1;

    [XmlElement("gridSizey")]
    public int gridSizey = 1;

    [XmlElement("baseHitPoints")]
    public float baseHitPoints = 50f;
    [XmlElement("baseMove")]
    public float baseMove = 5f;
    [XmlElement("baseActions")]
    public float baseActions = 1f;
    [XmlElement("baseMeleeDamage")]
    public float baseMeleeDamage = 10f;
    [XmlElement("baseRangedDamage")]
    public float baseRangedDamage = 10f;
    [XmlElement("baseMagicDamage")]
    public float baseMagicDamage = 10f;
    [XmlElement("baseDefense")]
    public float baseDefense = 0f;
    [XmlElement("baseInitiative")]
    public float baseInitiative = 5f;
    [XmlElement("baseAccuracy")]
    public float baseAccuracy = 0f;
    [XmlElement("baseEvasion")]
    public float baseEvasion = 0f;
    [XmlElement("baseResist")]
    public float baseResist = 0f;
    [XmlElement("baseSkillPower")]
    public float baseSkillPower = 0f;
    [XmlElement("baseSkillCharge")]
    public float baseSkillCharge = 0f;
    [XmlElement("baseCritChance")]
    public float baseCritChance = 10f;
  }
}
