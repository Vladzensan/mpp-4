using System.Collections.Generic;

namespace ClassGeneration
{
    public class ClassData
    {
        public string Name { private set; get; }

        public List<MethodData> MethodList { private set; get; }

        public ClassData(string name)
        {
            Name = name;
            MethodList = new List<MethodData>();
        }

    }
}
