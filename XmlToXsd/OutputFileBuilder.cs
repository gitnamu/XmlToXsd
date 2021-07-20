using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlToXsd
{
    class OutputFileBuilder
    {
        /** private 변수 **/
        /** schema 및 각 빌더 **/
        private XmlSchema schema { get; }
        private ElementBuillder elementBuilder { get; }
        private ComplexTypeBuilder complexTypeBuilder { get; }
        private SimpleTypeBuilder simpleTypeBuilder { get; }
        private ImportBuilder importBuilder { get; }
        private InputFileReader inputFileReader { get; }

        /** 생성자 **/
        /** schema 생성 및 ElementBuiler, ComplexTypeBuilder, SimpleTypeBuilder 객체 생성   **/
        public OutputFileBuilder(XmlDocument inputFile)
        {
            // schema 객체 생성 및 namespace 추가
            schema = new XmlSchema();
            schema.TargetNamespace = "http://www.iala-aism.org/S-247/gml/1.0";
            schema.Namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
            schema.Namespaces.Add("S100", "http://www.iho.int/s100gml/1.0");
            schema.Namespaces.Add("gml", "http://www.opengis.net/gml/3.2");
            schema.Namespaces.Add("S100EXT", "http://www.iho.int/s100gml/1.0+EXT");
            schema.Namespaces.Add("ns1", "http://www.iala-aism.org/S-100/profile/s100_gmlProfile");

            // builder들 객체 생성
            importBuilder = new ImportBuilder();
            elementBuilder = new ElementBuillder();
            complexTypeBuilder = new ComplexTypeBuilder();
            simpleTypeBuilder = new SimpleTypeBuilder();
            inputFileReader = new InputFileReader(inputFile);
        }

        /** xsd파일 생성 **/
        /** 생성한 스키마를 XDocument로 변환 **/
        private XDocument MatchSchemaToXsd()
        {
            StringWriter schemaWriter = new StringWriter();
            schema.Write(schemaWriter);
            XDocument xsdDoc = XDocument.Parse(schemaWriter.ToString());

            return xsdDoc;
        }

        /** <!-- file --> 부분 **/
        /** importBuilder객체의 buildImport 함수를 호출하여 <import> 생성 및 <schema>에 연결 **/
        private void BuildFilePart()
        {
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/s100gml/1.0", "s100gmlbase.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.opengis.net/gml/3.2", "S100_gmlProfile.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/S-100/profile/s100_gmlProfile", "S100_gmlProfileLevels.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/s100gml/1.0+EXT", "s100gmlbaseExt.xsd"));
        }

        /** <!-- Type --> 부분 **/
        private void BuildTypePart()
        {
            // featureType 생성
            XmlSchemaElement featureElement = elementBuilder.BuildAbstractElement("FeatureType", "FeatureType", "gml:AbstractFeature");
            schema.Items.Add(featureElement);
            XmlSchemaComplexType feautreComplexType = complexTypeBuilder.BuildAbstractComplexType("FeatureType", "Generalized feature type which carry all the common attributes", "S100:AbstractFeatureType");
            schema.Items.Add(feautreComplexType);

            // informationType 생성
            XmlSchemaElement informationElement = elementBuilder.BuildAbstractElement("InformationType", "InformationType", "gml:AbstractGML");
            schema.Items.Add(informationElement);
            XmlSchemaComplexType informationComplexType = complexTypeBuilder.BuildAbstractComplexType("InformationType", "Generalized information type which carry all the common attributes", "S100:AbstractInformationType");
            schema.Items.Add(informationComplexType);

            // SimpleType ISO639-3 생성
            XmlSchemaSimpleType simpleType = simpleTypeBuilder.BuildSimpleTypeOfType("ISO639-3", "stub for ISO 639-3 language codes", "xs:string", @"\w{3}");
            schema.Items.Add(simpleType);

            // complexType GM_Point 생성
            XmlSchemaComplexType complexType = complexTypeBuilder.BuildChoiceComplexType("GM_Point", "S100:pointProperty");
            schema.Items.Add(complexType);
        }

        /** <!-- Enumeration(8) --> 부분 **/
        private void BuildEnumerationPart()
        {
            List<Enumeration> enumerationList = inputFileReader.GetEnumeration();

            foreach (Enumeration enumeration in enumerationList)
            {
                XmlSchemaSimpleType simpleType = simpleTypeBuilder.BuildSimpleType(enumeration);
                schema.Items.Add(simpleType);
            }
        }

        /** <!-- ComplexAttributeType(13) --> 부분 **/
        private void BuildComplexAttributeTypePart()
        {
            List<S100_FC_ComplexAttribute> complexAttributeList = inputFileReader.GetS100_FC_ComplexAttribute();
            foreach(S100_FC_ComplexAttribute complexAttribute in complexAttributeList)
            {
                XmlSchemaComplexType complexType = complexTypeBuilder.BuildSequenceComplexType(complexAttribute.name, complexAttribute.documentation, complexAttribute.attribute);
                schema.Items.Add(complexType);
            }
        }

        /** <!-- infomationType (1) --> 부분 **/
        private void BuildInfomationTypePart()
        {
            S100_FC_InformationType informationType = inputFileReader.GetS100_FC_InformationType();
            XmlSchemaComplexType complexType = complexTypeBuilder.BuildComplexType(informationType.name, informationType.documentation, informationType.baseName, informationType.attribute);
            schema.Items.Add(complexType);
        }

        /** <!-- FeatureType (6) --> 부분 **/
        private void BuildFeatureTypePart()
        {
            S100_FC_FeatureType featureType = inputFileReader.GetS100_FC_FeatureType();
            XmlSchemaComplexType complexType = complexTypeBuilder.BuildComplexType(featureType.name, featureType.documentation, featureType.baseName, featureType.attribute);
            schema.Items.Add(complexType);
        }

        /** 전체 output 파일 생성 **/
        public XDocument BuildOutputFile()
        {
            BuildFilePart();
            BuildTypePart();
            BuildEnumerationPart();
            BuildComplexAttributeTypePart();
            BuildInfomationTypePart();
            BuildFeatureTypePart();

            return MatchSchemaToXsd();
        }
    }
}
