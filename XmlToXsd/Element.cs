using System;
using System.Collections.Generic;
using System.Text;

namespace XmlToXsd
{
    interface Element
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int MinOccurs { get; set; }
        public int MaxOccurs { get; set; }
    }
}
