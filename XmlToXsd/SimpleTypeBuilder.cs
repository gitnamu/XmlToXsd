using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlToXsd
{
    class SimpleTypeBuilder
    {
        /** simpleType 태그 객체를 생성하는 함수 **/
        private XmlSchemaSimpleType MakeSimpleTypeTag(string name)
        {
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
            simpleType.Name = name;

            return simpleType;
        }

        /** restriction 태그 객체를 생성하는 함수 **/
        private XmlSchemaSimpleTypeRestriction MakeRestrictionTag(string restrictionBase)
        {
            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName(restrictionBase, "http://www.w3.org/2001/XMLSchema");

            return restriction;
        }

        /** enumeration 태그 객체를 생성하는 함수 **/
        private XmlSchemaEnumerationFacet MakeEnumerationTag(string enumValue)
        {
            XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = enumValue;

            return enumeration;
        }

        /** simpleType 빌드하는 함수 **/
        /** <!-- Enumeration (8) --> 에서 사용됨 **/
        public XmlSchemaSimpleType BuildSimpleType(Enumeration enumeration)
        {
            XmlSchemaSimpleType simpleTypeTag = MakeSimpleTypeTag(enumeration.name);
            XmlSchemaSimpleTypeRestriction restrictionTag = MakeRestrictionTag(enumeration.restrictionBase);
            foreach (string value in enumeration.value)
            {
                XmlSchemaEnumerationFacet enumerationTag = MakeEnumerationTag(value);
                restrictionTag.Facets.Add(enumerationTag);
            }
            simpleTypeTag.Content = restrictionTag;

            return simpleTypeTag;
        }
    }
}
