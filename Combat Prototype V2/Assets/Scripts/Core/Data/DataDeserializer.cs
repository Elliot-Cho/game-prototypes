using System.Collections.Generic;
using System.Xml.Linq;

namespace Wayfinder {
  public class DataDeserializer {
    private static readonly Dictionary<string, string> DESERIALIZE_TYPE =
      new Dictionary<string, string>
        {
          { "abilities", "Ability" },
          { "units", "Unit" }
        };

    public static List<SerializedObject> DeserializeFile(string path) {
      XDocument document = XDocument.Load(path);

      string rootName = document.Root.Name.ToString();

      if (!DESERIALIZE_TYPE.ContainsKey(rootName)) {
        throw new System.Exception($"No deserializer found for {rootName}");
      }

      List<SerializedObject> result = new List<SerializedObject>();
      IEnumerable<XElement> elements =
        document.Root.Elements(DESERIALIZE_TYPE[rootName].ToLower());

      foreach(XElement element in elements) {
        string serializedObject = $"Serialized{DESERIALIZE_TYPE[rootName]}";
        result.Add(XMLOperations.Deserialize(element, serializedObject));
      }

      return result;
    }
  }
}
