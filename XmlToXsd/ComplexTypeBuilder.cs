using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace XmlToXsdConverter
{
    class ComplexTypeBuilder
    {
        ElementBuillder elementBuillder = new ElementBuillder();

        /** text에서 node array로 변환하는 함수 **/
        private XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] { doc.CreateTextNode(text) };
        }

        /** complexType 태극 객체를 생성하는 함수 **/
        private XmlSchemaComplexType makeComplexTypeTag(String name)
        {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            complexType.Name = name;

            return complexType;
        }

        /** annoatation 태그 객체와 documenatation 태그 객체를 생성하는 함수 **/
        private XmlSchemaAnnotation makeAnnotationTag(String documentationValue)
        {
            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();

            // documentataion 객체 생성, 값 할당 및 annotation과 연결
            XmlSchemaDocumentation documentataion = new XmlSchemaDocumentation();
            annotation.Items.Add(documentataion);
            documentataion.Markup = TextToNodeArray(documentationValue);

            return annotation;
        }

        /** complexContent 태그 객체를 생성하는 함수 **/
        private XmlSchemaComplexContent makeComplexContentTag()
        {
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            return complexContent;
        }

        /** extension 태그 객체를 생성하고 base를 지정하는 함수 **/
        private XmlSchemaComplexContentExtension makeExtensionTag(String extensionBaseUrl)
        {
            XmlSchemaComplexContentExtension extension = new XmlSchemaComplexContentExtension();
            extension.BaseTypeName = new XmlQualifiedName(extensionBaseUrl);

            return extension;
        }

        /** sequence 태그 객체를 생성하고 그 안의 element 객체들을 생성하는 함수 **/
        private XmlSchemaSequence makeSequenceTag(Element[] elementList)
        {
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            foreach(Element element in elementList)
            {
                XmlSchemaElement newElement = elementBuillder.buildSequenceElement(element.Name, element.Type, element.MinOccurs, element.MaxOccurs);
                sequence.Items.Add(newElement);
            }

            return sequence;
        }

        /** abstract complexType을 생성하는 함수 **/
        /** <!-- type --> 에서 사용 됨 **/
        public XmlSchemaComplexType buildAbstractComplexType(String name, String documentationValue, String extensionBaseUrl)
        {
            XmlSchemaComplexType complexType = makeComplexTypeTag(name);                        // complexType 객체 생성
            complexType.IsAbstract = true;                                                      // abstract 설정

            XmlSchemaAnnotation annotation = makeAnnotationTag(documentationValue);             // annotation 객체 생성
            complexType.Annotation = annotation;                                                // annotation를 complexType과 연결

            XmlSchemaComplexContent complexContent = makeComplexContentTag();                   // complexContent 객체 생성
            complexType.ContentModel = complexContent;                                          // complexContent를 complexType과 연결

            return complexType;
        }

        /** complexContent가 없고 sequence는 있는 complexType을 생성하는 함수 **/
        /**	<!-- ComplexAttributeType (13) --> 에서 사용 됨 **/
        public XmlSchemaComplexType buildSequenceComplexType(String name, String documentationValue, Element[] elementList)
        {
            XmlSchemaComplexType complexType = makeComplexTypeTag(name);            // complexType 객체 생성

            XmlSchemaAnnotation annotation = makeAnnotationTag(documentationValue); // annotation 객체 생성
            complexType.Annotation = annotation;                                    // annotation를 complexType과 연결

            XmlSchemaSequence sequence = makeSequenceTag(elementList);              // sequence 객체 생성
            complexType.Particle = sequence;                                        // sequence를 complexType과 연결

            return complexType;
        }

        /** complexContent와 sequence가 모두 있는 complexType을 생성하는 함수 **/
        /** <!-- infomationType (1) --> 와 <!-- FeatureType (6) --> 에서 사용 됨 **/
        public XmlSchemaComplexType buildComplexType(String name, String documentationValue, String extensionBaseUrl, Element[] elementList)
        {
            XmlSchemaComplexType complexType = makeComplexTypeTag(name);                    // complexType 객체 생성

            XmlSchemaAnnotation annotation = makeAnnotationTag(documentationValue);         // annotation 객체 생성
            complexType.Annotation = annotation;                                            // annotation를 complexType과 연결

            XmlSchemaComplexContent complexContent = makeComplexContentTag();               // complexContent 객체 생성
            complexType.ContentModel = complexContent;                                      // complexContent를 complexType과 연결

            XmlSchemaComplexContentExtension extension = makeExtensionTag(extensionBaseUrl);// extension 객체 생성
            complexContent.Content = extension;                                             // extension을 complexContent와 연결

            XmlSchemaSequence sequence = makeSequenceTag(elementList);                      // sequence 객체 생성
            extension.Particle = sequence;                                                  // sequence를 complexType과 연결

            return complexType;
        }
    }
}
