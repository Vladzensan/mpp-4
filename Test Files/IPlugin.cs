using System;

namespace IntegerGenerator
{
    public interface IPlugin
    {
        dynamic GenerateValue();

        string GetGeneratorTypeName();
    }
}