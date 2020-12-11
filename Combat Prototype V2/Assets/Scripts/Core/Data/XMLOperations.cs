using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Wayfinder {
  public class XMLOperations {
    public static void Serialize(object item, string path) {
      XmlSerializer serializer = new XmlSerializer(item.GetType());
      StreamWriter writer = new StreamWriter(path);
      serializer.Serialize(writer.BaseStream, item);
      writer.Close();
    }

    public static SerializedObject Deserialize(XElement xElement, string typeString) {
      Type type = Type.GetType($"Wayfinder.{typeString}");

      XmlSerializer serializer = new XmlSerializer(type);
      return (SerializedObject)serializer.Deserialize(xElement.CreateReader());
    }
  }
}
