using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlToXsd
{
    class InputFileReader
    {
        private XmlDocument inputFile { get; }      // 입력받은 xml 파일
        private XmlNamespaceManager nmspc { get; }  // namespace

        /** valueType 반환 **/
        /** text는 string, integer는 int, date는 date로 변환 **/
        private string ConvertValueType(string valueType)
        {
            switch (valueType)
            {
                case "text":
                    return "string";
                case "integer":
                    return "int";
                case "date":
                    return "date";
                default:
                    return null;
            }
        }

        /** attribute 반환 **/
        /** <S100FC:multiplicity>의 upper, lower, nil, infinite 반환 **/
        private Attribute GetAttribute(XmlNode attributeBinding)
        {
            Attribute attribute = new Attribute();

            attribute.attributeName = attributeBinding.SelectSingleNode("S100FC:attribute", nmspc).Attributes.GetNamedItem("ref").InnerText;    // <S100FC:attribute>의 ref 추출

            XmlNode multiplicity = attributeBinding.SelectSingleNode("S100FC:multiplicity", nmspc);                                                         // <S100FC:multiplicity>
            attribute.lower = Convert.ToInt32(multiplicity.SelectSingleNode("S100Base:lower", nmspc).InnerText);                                            // <S100Base:lower>의 innerText추출
            attribute.nil = Convert.ToBoolean(multiplicity.SelectSingleNode("S100Base:upper", nmspc).Attributes.GetNamedItem("xsi:nil").InnerText);         // <S100Base:upper>의 xsi:nil 추출
            attribute.infinite = Convert.ToBoolean(multiplicity.SelectSingleNode("S100Base:upper", nmspc).Attributes.GetNamedItem("infinite").InnerText);   // <S100Base:lower>의 infinite추출

            // infinite가 false일때만 <S100Base:upper>의 innerText추출
            if (attribute.infinite == false)
            {
                attribute.upper = Convert.ToInt32(multiplicity.SelectSingleNode("S100Base:upper", nmspc).InnerText);
            }

            return attribute;
        }

        /** 생성자 **/
        /** namespace 생성 **/
        public InputFileReader(XmlDocument inputFile)
        {
            this.inputFile = inputFile;

            // namespace 추가
            nmspc = new XmlNamespaceManager(inputFile.NameTable);
            nmspc.AddNamespace("S100FC", "http://www.iho.int/S100FC");
            nmspc.AddNamespace("S100Base", "http://www.iho.int/S100Base");
            nmspc.AddNamespace("S100CI", "http://www.iho.int/S100CI");
            nmspc.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            nmspc.AddNamespace("S100FD", "http://www.iho.int/S100FD");
            nmspc.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            nmspc.AddNamespace("schemaLocation", "http://www.iho.int/S100FC S100FC.xsd");
        }

        /** S100_FC_InformationType 읽어서 필요한 정보 반환 **/
        public S100_FC_InformationType GetS100_FC_InformationType()
        {
            S100_FC_InformationType informationType = new S100_FC_InformationType();
            informationType.documentation = "none";         // documentation은 default가 none
            informationType.baseName = "InformationType";   // baseName의 default는 InformationType

            XmlNodeList informationTypeNode = inputFile.GetElementsByTagName("S100FC:S100_FC_InformationType"); // <S100FC:S100_FC_InformationType> 리스트
            informationType.name = informationTypeNode[0].SelectSingleNode("S100FC:code", nmspc).InnerText;     // <S100FC:code> innerText를 name으로 설정

            XmlNodeList attributeBindingList = informationTypeNode[0].SelectNodes("S100FC:attributeBinding", nmspc);    // <S100FC:attributeBinding> 리스트

            List<Attribute> attributeList = new List<Attribute>();    // Attribute 리스트 생성

            List<S100_FC_SimpleAttribute> simpleAttributeList = GetS100_FC_SimpleAttribute();       // simpleAttribute 리스트 가져옴

            for (int i = 0; i < attributeBindingList.Count; i++)
            {
                Attribute attribute = GetAttribute(attributeBindingList[i]);    // attributeBinding의 이름 및 multiplicity정보를 가져옴

                string valueType = simpleAttributeList[i].valueType;            // attribute와 동일한 이름을 갖는 simpleAttribute의 valueType을 가져옴
                attribute.valueType = ConvertValueType(valueType);              // simpleAttribute의 valueType을 attribute의 valueType으로 설정

                attributeList.Add(attribute);   // attribute를 리스트에 추가
            }
            informationType.attribute = attributeList;  // 구한 attributeList를 informationType의 attribute에 할당

            return informationType;
        }

        /** S100_FC_FeatureType 읽어서 필요한 정보 반환 **/
        public S100_FC_FeatureType GetS100_FC_FeatureType()
        {
            S100_FC_FeatureType featureType = new S100_FC_FeatureType();
            featureType.documentation = "none";     // featureType의 default documentation은 none
            featureType.baseName = "FeatureType";   // featureType의 default baseName은 FeatureType

            XmlNodeList featureTypeNode = inputFile.GetElementsByTagName("S100FC:S100_FC_FeatureType");     // <S100FC:S100_FC_FeatureType>
            XmlNode code = featureTypeNode[0].SelectSingleNode("S100FC:code", nmspc);                       // <S100FC:code>
            featureType.name = code.InnerText;                                                              // <S100FC:code>의 innerText 추출

            XmlNodeList attributeBindingList = featureTypeNode[0].SelectNodes("S100FC:attributeBinding", nmspc);    // <S100FC:attributeBinding> 리스트
            List<Attribute> attributeList = new List<Attribute>(attributeBindingList.Count);                        // <S100FC:attributeBinding> 개수 크기의 리스트 생성

            foreach (XmlNode attributeBinding in attributeBindingList)
            {
                attributeList.Add(GetAttribute(attributeBinding));   // attribute의 필요한 정보(이름 및 multiplicity)를 가져와 리스트에 추가
            }
            featureType.attribute = attributeList;  // 구한 attributeList를 featureType의 attribute에 할당

            return featureType;
        }

        /** S100_FC_SimpleAttribute 읽어서 필요한 정보 반환 **/
        public List<S100_FC_SimpleAttribute> GetS100_FC_SimpleAttribute()
        {
            List<S100_FC_SimpleAttribute> simpleAttributeList = new List<S100_FC_SimpleAttribute>();
            XmlNodeList simpleAttributeNodeList = inputFile.GetElementsByTagName("S100FC:S100_FC_SimpleAttribute");

            foreach (XmlNode simpleAttributeNode in simpleAttributeNodeList)
            {
                S100_FC_SimpleAttribute simpleAttribute = new S100_FC_SimpleAttribute();
                XmlNode code = simpleAttributeNode.SelectSingleNode("S100FC:code", nmspc);
                simpleAttribute.name = code.InnerText;

                XmlNode valueType = simpleAttributeNode.SelectSingleNode("S100FC:valueType", nmspc);
                simpleAttribute.valueType = valueType.InnerText;

                simpleAttributeList.Add(simpleAttribute);
            }

            return simpleAttributeList;
        }

        /** S100_FC_SimpleAttribute의 enumeration을 읽어서 팔요한 정보 반환 **/
        public List<Enumeration> GetEnumeration()
        {
            List<Enumeration> enumerationList = new List<Enumeration>();

            XmlNodeList simpleAttributeNodeList = inputFile.GetElementsByTagName("S100FC:S100_FC_SimpleAttribute");
            foreach (XmlNode simpleAttributeNode in simpleAttributeNodeList)
            {
                Enumeration enumeration = new Enumeration();
                enumeration.restrictionBase = "string";

                XmlNode valueType = simpleAttributeNode.SelectSingleNode("S100FC:valueType", nmspc);
                if (valueType.InnerText != "enumeration") continue;

                XmlNode code = simpleAttributeNode.SelectSingleNode("S100FC:code", nmspc);
                enumeration.name = code.InnerText + "Type";
                XmlNode listedValuesNode = simpleAttributeNode.SelectSingleNode("S100FC:listedValues", nmspc);
                XmlNodeList listedValueList = listedValuesNode.SelectNodes("S100FC:listedValue", nmspc);

                List<string> valueList = new List<string>();

                foreach (XmlNode listedValue in listedValueList)
                {
                    string label = listedValue.SelectSingleNode("S100FC:label", nmspc).InnerText;
                    valueList.Add(label);
                }
                enumeration.value = valueList;
                enumerationList.Add(enumeration);
            }

            return enumerationList;
        }


        /** S100_FC_ComplexAttribute의 enumeration을 읽어서 필요한 정보 반환 **/
        public List<S100_FC_ComplexAttribute> GetS100_FC_ComplexAttribute()
        {
            List<S100_FC_ComplexAttribute> complexAttributeList = new List<S100_FC_ComplexAttribute>();

            XmlNodeList complexAttributeNodeList = inputFile.GetElementsByTagName("S100FC:S100_FC_ComplexAttribute");
            foreach (XmlNode complexAttributeNode in complexAttributeNodeList)
            {
                S100_FC_ComplexAttribute complexAttribute = new S100_FC_ComplexAttribute();

                XmlNode code = complexAttributeNode.SelectSingleNode("S100FC:code", nmspc);
                complexAttribute.name = code.InnerText + "Type";
                complexAttribute.documentation = "none";

                List<Attribute> attributeList = new List<Attribute>();

                XmlNodeList attributeBindingList = complexAttributeNode.SelectNodes("S100FC:subAttributeBinding", nmspc);    // <S100FC:attributeBinding> 리스트
                foreach (XmlNode attributeBinding in attributeBindingList)
                {
                    attributeList.Add(GetAttribute(attributeBinding));   // attribute를 리스트에 추가
                }
                complexAttribute.attribute = attributeList;
                complexAttributeList.Add(complexAttribute);
            }
            return complexAttributeList;
        }
    }


    public struct Attribute
    {
        public string attributeName { get; set; }
        public int lower { get; set; }
        public int upper { get; set; }
        public bool nil { get; set; }
        public bool infinite { get; set; }
        public string valueType { get; set; }
    }

    public struct S100_FC_InformationType
    {
        public string name { get; set; }
        public string documentation { get; set; }
        public string baseName { get; set; }
        public List<Attribute> attribute { get; set; }
    }

    public struct S100_FC_FeatureType
    {
        public string name { get; set; }
        public string documentation { get; set; }
        public string baseName { get; set; }
        public List<Attribute> attribute { get; set; }
    }

    public struct S100_FC_SimpleAttribute
    {
        public string name { get; set; }
        public string valueType { get; set; }
    }

    public struct S100_FC_ComplexAttribute
    {
        public string name { get; set; }
        public string documentation { get; set; }
        public List<Attribute> attribute { get; set; }
    }

    public struct Enumeration
    {
        public string name { get; set; }
        public string restrictionBase { get; set; }
        public List<string> value { get; set; }

    }
}