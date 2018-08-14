using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("AbilityCollection")]
public class AbilityCollection {

    [XmlArray("Abilities")]
    [XmlArrayItem("Ability")]
    public List<Ability> Abilities = new List<Ability>();

	public static AbilityCollection Load(string path)
    {
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(AbilityCollection));

        StringReader reader = new StringReader(_xml.text);

        AbilityCollection abilities = serializer.Deserialize(reader) as AbilityCollection;

        reader.Close();

        return abilities;
    }
}
