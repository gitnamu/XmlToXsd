using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsdConverter
{
    class InputFileReader
    {
        private XmlDocument inputFile { get; }
        private XmlNamespaceManager nmspc { get; }

        public InputFileReader(XmlDocument inputFile)
        {
            this.inputFile = inputFile;

            nmspc = new XmlNamespaceManager(inputFile.NameTable);
            nmspc.AddNamespace("S100FC", "http://www.iho.int/S100FC");
            nmspc.AddNamespace("S100Base", "http://www.iho.int/S100Base");
            nmspc.AddNamespace("S100CI", "http://www.iho.int/S100CI");
            nmspc.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            nmspc.AddNamespace("S100FD", "http://www.iho.int/S100FD");
            nmspc.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            nmspc.AddNamespace("schemaLocation", "http://www.iho.int/S100FC S100FC.xsd");
        }

        public S100_FC_InformationType getS100_FC_InformationType()
        {
            S100_FC_InformationType informationType = new S100_FC_InformationType();
            XmlNodeList informationTypeNode = inputFile.GetElementsByTagName("S100FC:S100_FC_InformationType");
            XmlNode code = informationTypeNode[0].SelectSingleNode("S100FC:code", nmspc);
            informationType.name = code.InnerText;
            XmlNodeList attributeBindingList = informationTypeNode[0].SelectNodes("S100FC:attributeBinding", nmspc);
            List<Attribute> attributeList = new List<Attribute>(attributeBindingList.Count);
            foreach (XmlNode attributeBinding in attributeBindingList)
            {
                Attribute attribute = new Attribute();
                attribute.attributeName = attributeBinding.SelectSingleNode("S100FC:attribute", nmspc).Attributes.GetNamedItem("ref").InnerText;

                XmlNode multiplicity = attributeBinding.SelectSingleNode("S100FC:multiplicity", nmspc);
                attribute.lower = Convert.ToInt32(multiplicity.SelectSingleNode("S100Base:lower", nmspc).InnerText);
                attribute.nil = Convert.ToBoolean(multiplicity.SelectSingleNode("S100Base:upper", nmspc).Attributes.GetNamedItem("xsi:nil").InnerText);
                attribute.infinite = Convert.ToBoolean(multiplicity.SelectSingleNode("S100Base:upper", nmspc).Attributes.GetNamedItem("infinite").InnerText);

                if(attribute.infinite == false)
                {
                    attribute.upper = Convert.ToInt32(multiplicity.SelectSingleNode("S100Base:upper", nmspc).InnerText);
                }

                attributeList.Add(attribute);
            }
            informationType.attribute = attributeList;

            return informationType;
        }
        
    }
}


public sealed class Attribute
{
    public String attributeName { get; set; }
    public int lower { get; set; }
    public int upper { get; set; }
    public bool nil { get; set; }
    public bool infinite { get; set; }
}

public sealed class S100_FC_InformationType
{
    public String name { get; set; }
    public List<Attribute> attribute { get; set; }

}

public sealed class S100_FC_FeatureType
{

}
