using System;
using System.Collections.Generic;
using System.Text;

namespace XmlToXsdConverter
{
    interface Element
    {
            public String Name { get; set; }
            public String Type { get; set; }
            public int MinOccurs { get; set; }
            public int MaxOccurs { get; set; }
    }
}
