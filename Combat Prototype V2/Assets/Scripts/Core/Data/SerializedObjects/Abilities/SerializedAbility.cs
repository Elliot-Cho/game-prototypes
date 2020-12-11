using System.Xml.Serialization;
using System.Collections.Generic;

namespace Wayfinder {
  [System.Serializable]
  [XmlRoot("ability")]
  public class SerializedAbility : SerializedObject {
    [XmlElement("id")]
    public string id;
    [XmlElement("name")]
    public string name;
    [XmlElement("description")]
    public string description;
    [XmlElement("iconPath")]
    public string iconPath;

    [XmlElement("range")]
    public float range = 1f;
    [XmlElement("areaRange")]
    public float areaRange = 0f;
    [XmlElement("effectMultiplier")]
    public float effectMultiplier = 1f;
    [XmlElement("baseActionPointCost")]
    public float baseActionPointCost = 1f;

    [XmlElement("dangerType")]
    public string dangerType;
    [XmlElement("targetType")]
    public string targetType;

    [XmlArray("abilityEffects")]
    [XmlArrayItem("abilityEffect")]
    public List<string> abilityEffects;
  }
}
