using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
[XmlRoot("character")]
public class SerializedCharacter : SerializedUnit {

  [XmlElement("playerRelations"), Range(-100f, 100f)]
  public float playerRelations = 0;

  [XmlElement("factionLoyalty"), Range(-100f, 100f)]
  public float factionLoyalty = 0;

  [XmlElement("baseAttributes")]
  public BaseAttributes baseAttributes = new BaseAttributes();

  [XmlElement("baseTalents")]
  public BaseTalents baseTalents = new BaseTalents();

  [XmlElement("talentStats")]
  public TalentStats talentStats = new TalentStats();

  [System.Serializable]
  public class BaseAttributes
  {
    [XmlElement("str")]
    public float strength = 10;

    [XmlElement("dex")]
    public float dexterity = 10;

    [XmlElement("spd")]
    public float speed = 10;

    [XmlElement("con")]
    public float constitution = 10;

    [XmlElement("apt")]
    public float aptitude = 10;
  }

  [System.Serializable]
  public class BaseTalents
  {
    [XmlElement("expl")]
    public float exploration = 0;

    [XmlElement("crft")]
    public float crafting = 0;

    [XmlElement("smth")]
    public float smithing = 0;

    [XmlElement("thau")]
    public float thaumaturgy = 0;
  }

  [System.Serializable]
  public class TalentStats
  {
    [XmlElement("travelSpeed")]
    public float travelSpeed = 1f;
    [XmlElement("regionClearSpeed")]
    public float regionClearSpeed = 1f;
    [XmlElement("craftingQuality")]
    public float craftingQuality = 0f;
    [XmlElement("craftingSpeed")]
    public float craftingSpeed = 1f;
    [XmlElement("buildingSpeed")]
    public float buildingSpeed = 1f;
    [XmlElement("smithingQuality")]
    public float smithingQuality = 0f;
    [XmlElement("smithingSpeed")]
    public float smithingSpeed = 1f;
    [XmlElement("thaumQuality")]
    public float thaumQuality = 0f;
    [XmlElement("thaumSpeed")]
    public float thaumSpeed = 1f;
  }
}
