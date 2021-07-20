using System.Xml;
using System.Xml.Schema;

namespace XmlToXsd
{
    class SimpleTypeBuilder
    {
        /** <simpleType> 객체를 생성하는 함수 **/
        private XmlSchemaSimpleType MakeSimpleTypeTag(string name)
        {
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
            simpleType.Name = name;

            return simpleType;
        }

        /** <restriction> 객체를 생성하는 함수 **/
        private XmlSchemaSimpleTypeRestriction MakeRestrictionTag(string restrictionBase)
        {
            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName(restrictionBase, "http://www.w3.org/2001/XMLSchema");

            return restriction;
        }

        /** <enumeration> 객체를 생성하는 함수 **/
        private XmlSchemaEnumerationFacet MakeEnumerationTag(string enumValue)
        {
            XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = enumValue;

            return enumeration;
        }

        private XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] { doc.CreateTextNode(text) };
        }

        /** simpleType 빌드하는 함수 **/
        /** <!-- Enumeration (8) --> 에서 사용됨 **/
        public XmlSchemaSimpleType BuildSimpleType(Enumeration enumeration)
        {
            // simpleType 태그 생성
            XmlSchemaSimpleType simpleType = MakeSimpleTypeTag(enumeration.name);

            // restriction 태그 생성
            XmlSchemaSimpleTypeRestriction restriction = MakeRestrictionTag(enumeration.restrictionBase);
            simpleType.Content = restriction;

            // enumeration 태그 생성
            foreach (string value in enumeration.value)
            {
                XmlSchemaEnumerationFacet enumerationTag = MakeEnumerationTag(value);
                restriction.Facets.Add(enumerationTag);
            }

            return simpleType;
        }

        /** <!-- Type --> 에서 사용되는 SimpleType 빌드하는 함수**/
        public XmlSchemaSimpleType BuildSimpleTypeOfType(string name, string documentation, string restrictionBase, string patternValue)
        {
            // simpleType 태그 생성
            XmlSchemaSimpleType simpleType = MakeSimpleTypeTag(name);

            // annoatation 태그 생성
            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();
            simpleType.Annotation = annotation;

            // documentation 태그 생성
            XmlSchemaDocumentation documenatation = new XmlSchemaDocumentation();
            annotation.Items.Add(documenatation);
            documenatation.Markup = TextToNodeArray(documentation);

            // restriction 태그 생성
            XmlSchemaSimpleTypeRestriction restriction = MakeRestrictionTag(restrictionBase);
            simpleType.Content = restriction;

            // pattern 태그 생성
            XmlSchemaPatternFacet pattern = new XmlSchemaPatternFacet();
            pattern.Value = patternValue;
            restriction.Facets.Add(pattern);

            return simpleType;
        }
    }
}
