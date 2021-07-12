using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;

namespace XmlToXsdConverter
{
    class ImportBuilder
    {
        public XmlSchemaImport buildImport(String schemaNamespace, String schemaLocation)
        {
            XmlSchemaImport newImport = new XmlSchemaImport();
            newImport.Namespace = schemaNamespace;
            newImport.SchemaLocation = schemaLocation;

            return newImport;
        }
    }
}
