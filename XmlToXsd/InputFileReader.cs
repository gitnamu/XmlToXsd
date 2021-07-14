﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsd
{
    class InputFileReader
    {
        private XmlDocument inputFile { get; }
        private XmlNamespaceManager nmspc { get; }

        /** 생성자 **/
        /** namespace 생성 **/
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


        /** S100_FC_InformationType 읽어서 구조체 형태로 반환 **/
        public S100_FC_InformationType GetS100_FC_InformationType()
        {
            S100_FC_InformationType informationType = new S100_FC_InformationType();

            XmlNodeList informationTypeNode = inputFile.GetElementsByTagName("S100FC:S100_FC_InformationType"); // <S100FC:S100_FC_InformationType> 리스트
            XmlNode code = informationTypeNode[0].SelectSingleNode("S100FC:code", nmspc);                       // <S100FC:code>
            informationType.name = code.InnerText;                                                              // <S100FC:code>의 innerText 추출

            XmlNodeList attributeBindingList = informationTypeNode[0].SelectNodes("S100FC:attributeBinding", nmspc);    // <S100FC:attributeBinding> 리스트

            List<Attribute> attributeList = new List<Attribute>(attributeBindingList.Count);    // <S100FC:attributeBinding> 개수 크기의 리스트 생성

            foreach (XmlNode attributeBinding in attributeBindingList)
            {
                attributeList.Add(GetAttribute(attributeBinding));   // attribute를 리스트에 추가
            }
            informationType.attribute = attributeList;  // 구한 attributeList를 informationType의 attribute에 할당

            return informationType;
        }


        public S100_FC_FeatureType GetS100_FC_FeatureType()
        {
            S100_FC_FeatureType featureType = new S100_FC_FeatureType();


            return featureType;
        }


        /** S100_FC_SimpleAttribute의 enumeration을 읽어서 구조체 형태로 반환 **/
        public List<EnumerationOfS100_FC_SimpleAttribute> GetEnumerationOfS100_FC_SimpleAttribute()
        {
            List<EnumerationOfS100_FC_SimpleAttribute> enumerationList = new List<EnumerationOfS100_FC_SimpleAttribute>();

            XmlNodeList simpleAttributeNodeList = inputFile.GetElementsByTagName("S100FC:S100_FC_SimpleAttribute");
            foreach (XmlNode simpleAttributeNode in simpleAttributeNodeList)
            {
                EnumerationOfS100_FC_SimpleAttribute enumeration = new EnumerationOfS100_FC_SimpleAttribute();

                XmlNode valueType = simpleAttributeNode.SelectSingleNode("S100FC:valueType", nmspc);
                if (valueType.InnerText != "enumeration")
                {
                    continue;
                }

                XmlNode code = simpleAttributeNode.SelectSingleNode("S100FC:code", nmspc);
                enumeration.name = code.InnerText;
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


        /** S100_FC_ComplexAttribute의 enumeration을 읽어서 구조체 형태로 반환 **/
        public List<S100_FC_ComplexAttribute> GetS100_FC_ComplexAttribute()
        {
            List<S100_FC_ComplexAttribute> complexAttributeList = new List<S100_FC_ComplexAttribute>();

            XmlNodeList complexAttributeNodeList = inputFile.GetElementsByTagName("S100FC:S100_FC_ComplexAttribute");
            foreach (XmlNode complexAttributeNode in complexAttributeNodeList)
            {
                S100_FC_ComplexAttribute complexAttribute = new S100_FC_ComplexAttribute();

                XmlNode code = complexAttributeNode.SelectSingleNode("S100FC:code", nmspc);
                complexAttribute.name = code.InnerText;

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


    public sealed class Attribute
    {
        public string attributeName { get; set; }
        public int lower { get; set; }
        public int upper { get; set; }
        public bool nil { get; set; }
        public bool infinite { get; set; }
    }

    public sealed class S100_FC_InformationType
    {
        public string name { get; set; }
        public List<Attribute> attribute { get; set; }
    }

    public sealed class S100_FC_FeatureType
    {
        public string name { get; set; }
    }

    public sealed class S100_FC_ComplexAttribute
    {
        public string name { get; set; }
        public List<Attribute> attribute { get; set; }
    }

    public sealed class EnumerationOfS100_FC_SimpleAttribute
    {
        public string name { get; set; }
        public string restrictionBase { get; set; }
        public List<string> value { get; set; }

    }
}