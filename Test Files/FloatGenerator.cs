using System;
using PluginInterface;

namespace FloatGenerator
{
    public class FloatGenerator : IPlugin
    {
        private string TypeName = typeof(double).Name;
            
        public dynamic GenerateValue()
        {
            var rand = new Random();
            
            return rand.Next() + rand.NextDouble();
        }

        public string GetGeneratorTypeName()
        {
            return TypeName;
        }
    }
}