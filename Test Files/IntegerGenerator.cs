using System;
using PluginInterface;

namespace IntegerGenerator
{
    public class IntegerGenerator : IPlugin
    {
        private string TypeName = typeof(int).Name;
        
        public dynamic GenerateValue()
        {
            var rand = new Random();

            return rand.Next();
        }

        public string GetGeneratorTypeName()
        {
            return TypeName;
        }
    }
}