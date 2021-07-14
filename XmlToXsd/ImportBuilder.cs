using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsd
{
    class ImportBuilder
    {
        public XmlSchemaImport BuildImport(string schemaNamespace, string schemaLocation)
        {
            XmlSchemaImport newImport = new XmlSchemaImport();
            newImport.Namespace = schemaNamespace;
            newImport.SchemaLocation = schemaLocation;

            return newImport;
        }
    }
}
