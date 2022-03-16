using System.Xml.Serialization;

[Serializable]
public class UserCollection
{
    [XmlArray]
    public User[] Users { get; set; }
}
