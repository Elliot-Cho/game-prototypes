using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Test {

    [XmlElement("Name")]
    public string Name;
    [XmlElement("Desc")]
    public string Desc;

    /*[XmlElement("Image")]
    public GameObject Image;*/
}
