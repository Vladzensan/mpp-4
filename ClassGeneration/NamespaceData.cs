using System;
using System.Collections.Generic;
using System.Text;

namespace ClassGeneration
{
    public class NamespaceData
    {
        public string Name { private set; get; }
        
        public List<ClassData> ClassList { private set; get; }

        public NamespaceData(string name)
        {
            Name = name;
            ClassList = new List<ClassData>();
        }
    }
}
