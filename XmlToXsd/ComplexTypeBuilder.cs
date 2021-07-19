using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace XmlToXsd
{
    class ComplexTypeBuilder
    {
        private ElementBuillder elementBuillder;

        /** text에서 node array로 변환하는 함수 **/
        private XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] { doc.CreateTextNode(text) };
        }

        /** complexType 태그 객체를 생성하는 함수 **/
        private XmlSchemaComplexType MakeComplexTypeTag(string name)
        {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            complexType.Name = name;

            return complexType;
        }

        /** annoatation 태그 객체와 documenatation 태그 객체를 생성하는 함수 **/
        private XmlSchemaAnnotation MakeAnnotationTag(string documentationValue)
        {
            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();

            // documentataion 객체 생성, 값 할당 및 annotation과 연결
            XmlSchemaDocumentation documentataion = new XmlSchemaDocumentation();
            annotation.Items.Add(documentataion);
            documentataion.Markup = TextToNodeArray(documentationValue);

            return annotation;
        }

        /** complexContent 태그 객체를 생성하는 함수 **/
        private XmlSchemaComplexContent MakeComplexContentTag()
        {
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            return complexContent;
        }

        /** extension 태그 객체를 생성하고 base를 지정하는 함수 **/
        private XmlSchemaComplexContentExtension MakeExtensionTag(string extensionBaseUrl)
        {
            XmlSchemaComplexContentExtension extension = new XmlSchemaComplexContentExtension();
            extension.BaseTypeName = new XmlQualifiedName(extensionBaseUrl);

            return extension;
        }

        /** sequence 태그 객체를 생성하고 그 안의 element 객체들을 생성하는 함수 **/
        private XmlSchemaSequence MakeSequenceTag(List<Attribute> elementList)
        {
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            foreach(Attribute element in elementList)
            {
                XmlSchemaElement newElement = elementBuillder.BuildSequenceElement(element);
                sequence.Items.Add(newElement);
            }

            return sequence;
        }

        /** 생성자 **/
        public ComplexTypeBuilder()
        {
            elementBuillder = new ElementBuillder();
        }

        /** abstract complexType을 생성하는 함수 **/
        /** <!-- type --> 에서 사용 됨 **/
        public XmlSchemaComplexType BuildAbstractComplexType(string name, string documentationValue, string extensionBaseUrl)
        {
            XmlSchemaComplexType complexType = MakeComplexTypeTag(name);                        // complexType 객체 생성
            complexType.IsAbstract = true;                                                      // abstract 설정

            XmlSchemaAnnotation annotation = MakeAnnotationTag(documentationValue);             // annotation 객체 생성
            complexType.Annotation = annotation;                                                // annotation를 complexType과 연결

            XmlSchemaComplexContent complexContent = MakeComplexContentTag();                   // complexContent 객체 생성
            complexType.ContentModel = complexContent;                                          // complexContent를 complexType과 연결

            XmlSchemaComplexContentExtension extension = new XmlSchemaComplexContentExtension();// extesnsion 객체 생성
            complexContent.Content = extension;                                                 // extension을 complexContent와 연결
            extension.BaseTypeName = new XmlQualifiedName(extensionBaseUrl);


            return complexType;
        }

        /** complexContent가 없고 sequence는 있는 complexType을 생성하는 함수 **/
        /**	<!-- ComplexAttributeType (13) --> 에서 사용 됨 **/
        public XmlSchemaComplexType BuildSequenceComplexType(string name, string documentation, List<Attribute> attributeList)
        {
            XmlSchemaComplexType complexType = MakeComplexTypeTag(name);            // complexType 객체 생성

            XmlSchemaAnnotation annotation = MakeAnnotationTag(documentation);      // annotation 객체 생성
            complexType.Annotation = annotation;                                                    // annotation를 complexType과 연결

            XmlSchemaSequence sequence = MakeSequenceTag(attributeList);                // sequence 객체 생성
            complexType.Particle = sequence;                                                        // sequence를 complexType과 연결

            return complexType;
        }

        /** complexContent와 sequence가 모두 있는 complexType을 생성하는 함수 **/
        /** <!-- infomationType (1) --> 와 <!-- FeatureType (6) --> 에서 사용 됨 **/
        public XmlSchemaComplexType BuildComplexType(string name, string documentation, string baseName, List<Attribute> attributeList)
        {
            XmlSchemaComplexType complexType = MakeComplexTypeTag(name);                    // complexType 객체 생성

            XmlSchemaAnnotation annotation = MakeAnnotationTag(documentation);              // annotation 객체 생성
            complexType.Annotation = annotation;                                                            // annotation를 complexType과 연결

            XmlSchemaComplexContent complexContent = MakeComplexContentTag();                               // complexContent 객체 생성
            complexType.ContentModel = complexContent;                                                      // complexContent를 complexType과 연결

            XmlSchemaComplexContentExtension extension = MakeExtensionTag(baseName);        // extension 객체 생성
            complexContent.Content = extension;                                                             // extension을 complexContent와 연결

            XmlSchemaSequence sequence = MakeSequenceTag(attributeList);                        // sequence 객체 생성
            extension.Particle = sequence;                                                                  // sequence를 complexType과 연결

            return complexType;
        }
    }
}
