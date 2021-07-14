using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlToXsd
{
    class ElementBuillder
    {
        /** abstract element 생성 **/
        public XmlSchemaElement BuildAbstractElement(string name, string type, string substitutionGroup)
        {
            XmlSchemaElement newElement = new XmlSchemaElement();
            newElement.Name = name;
            newElement.SchemaTypeName = new XmlQualifiedName(type);
            newElement.IsAbstract = true;
            newElement.SubstitutionGroup = new XmlQualifiedName(substitutionGroup);

            return newElement;
        }

        /** complexType의 sequence 내부의 element 생성 **/
        public XmlSchemaElement BuildSequenceElement(string name, string type, int minOccurs, int maxOccurs)
        {
            XmlSchemaElement newElement = new XmlSchemaElement();
            newElement.Name = name;
            newElement.SchemaTypeName = new XmlQualifiedName(type, "http://www.w3.org/2001/XMLSchema");
            if(minOccurs != 1 || maxOccurs != 1)
            {
                newElement.MinOccurs = minOccurs;
                newElement.MaxOccurs = maxOccurs;
            }

            return newElement;
        }
    }
}
