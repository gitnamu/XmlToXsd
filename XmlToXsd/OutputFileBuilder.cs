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
        /** importBuilder객체의 buildImport 함수를 호출하여 import 요소들 생성 및 schema에 연결 **/
        private void BuildFilePart()
        {
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/s100gml/1.0", "s100gmlbase.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.opengis.net/gml/3.2", "S100_gmlProfile.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/S-100/profile/s100_gmlProfile", "S100_gmlProfileLevels.xsd"));
            schema.Includes.Add(importBuilder.BuildImport("http://www.iho.int/s100gml/1.0+EXT", "s100gmlbaseExt.xsd"));
        }

        /** <!-- type --> 부분 **/
        private void BuildTypePart()
        {
            // featureType 생성
            XmlSchemaElement featureElement = elementBuilder.BuildAbstractElement("FeatureType", "FeatureType", "gml:AbstractFeature");
            schema.Includes.Add(featureElement);
            XmlSchemaComplexType feautreComplexType = complexTypeBuilder.BuildAbstractComplexType("FeatureType", "Generalized feature type which carry all the common attributes", "S100:AbstractFeatureType");
            schema.Includes.Add(feautreComplexType);

            // informationType 생성
            XmlSchemaElement informationElement = elementBuilder.BuildAbstractElement("InformationType", "InformationType", "gml:AbstractGML");
            schema.Includes.Add(informationElement);
            XmlSchemaComplexType informationComplexType = complexTypeBuilder.BuildAbstractComplexType("InformationType", "Generalized information type which carry all the common attributes", "S100:AbstractInformationType");
            schema.Includes.Add(informationComplexType);
        }

        /** <!-- Enumeration(8) --> 부분 **/
        private void BuildEnumerationPart(List<EnumerationOfS100_FC_SimpleAttribute> enumerationList)
        {
            foreach(EnumerationOfS100_FC_SimpleAttribute enumeration in enumerationList)
            {
                XmlSchemaSimpleType simpleType = simpleTypeBuilder.BuildSimpleType(enumeration);
                schema.Includes.Add(simpleType);
            }
        }

        /** <!-- ComplexAttributeType(13) --> 부분 **/
        private void BuildComplexAttributeTypePart()
        {

        }

        /** <!-- infomationType (1) --> 부분 **/
        private void BuildInfomationTypePart()
        {

        }

        /** <!-- FeatureType (6) --> 부분 **/
        private void BuildFeatureTypePart()
        {

        }

        /** 전체 output 파일 생성 **/
        public XDocument BuildOutputFile()
        {
            XDocument outputFile = MatchSchemaToXsd();
            BuildFilePart();
            BuildTypePart();
            BuildEnumerationPart(inputFileReader.GetEnumerationOfS100_FC_SimpleAttribute());
            BuildComplexAttributeTypePart();
            BuildInfomationTypePart();
            BuildFeatureTypePart();

            return outputFile;
        }
    }
}
