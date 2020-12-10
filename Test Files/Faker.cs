using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PluginInterface;

namespace Faker
{
    public class Faker
    {
        private List<Type> _innerDTOTypes;
        private List<IPlugin> _plugins;
        private Dictionary<string, Func<dynamic>> _generators;
        
        public Faker()
        {

            _innerDTOTypes = new List<Type>();
            _generators = new Dictionary<string, Func<dynamic>>();
            PluginService.PluginService pluginService = new PluginService.PluginService();
            _plugins = pluginService.Plugins;

            foreach (var plugin in _plugins)
            {
                _generators.Add(plugin.GetGeneratorTypeName(), plugin.GenerateValue);
            }

            _generators.Add("List`1", null);

        }
        
        private ConstructorInfo GetMaxParamsConstr(ConstructorInfo[] constructors)
        {
            int position = 0;
            int amountOfParams = 0;
            for (int i = 0; i < constructors.Length; i++)
            {
                if (constructors[i].GetParameters().Length > amountOfParams)
                {
                    amountOfParams = constructors[i].GetParameters().Length;
                    position = i;
                }
            }
            return constructors[position];
        }

        private dynamic[] GetValuesForConstructor(ConstructorInfo constructorInfo)
        {
            dynamic[] values = new dynamic[constructorInfo.GetParameters().Length];
            int currValuePosition = 0;
            
            foreach (var param in constructorInfo.GetParameters())
            {
                 values[currValuePosition] = GenerateValue(param.ParameterType);
                 currValuePosition++;
            }

            return values;
        }

        private void InitializeFields(dynamic obj, FieldInfo[] fields)
        {
            
            foreach(FieldInfo field in fields)
            {
                if (!field.FieldType.IsValueType && !_generators.ContainsKey(field.FieldType.Name))
                {
                    field.SetValue(obj, CreateDto(field.FieldType));
                }
                else
                {
                    field.SetValue(obj, GenerateValue(field.FieldType));
                }
            }
            
        }

        private void InitializeProperties(dynamic obj, PropertyInfo[] properties)
        {

            foreach (PropertyInfo property in properties)
            {
                if (property.GetSetMethod() != null)
                {
                    if (!property.PropertyType.IsValueType && !_generators.ContainsKey(property.PropertyType.Name))
                    {
                        property.SetValue(obj, CreateDto(property.PropertyType));
                    }
                    else
                    {
                        property.SetValue(obj, GenerateValue(property.PropertyType));
                    }
                }
            }
            
        }

        private object CreateDto(Type objType)
        {
            if (_innerDTOTypes.Contains(objType))
            {
                return null;
            }

            _innerDTOTypes.Add(objType);
            
            object obj = null;
            var classConstructors = objType.GetConstructors();
            if (classConstructors.Length > 0)
            {
                var constructor = GetMaxParamsConstr(classConstructors);
                
                try
                {
                    obj = constructor.Invoke(GetValuesForConstructor(constructor));

                }
                catch
                {
                    return null;
                }
                
                if (objType.GetFields().Length != 0)
                {
                    InitializeFields(obj, objType.GetFields());
                }

                if (objType.GetProperties().Length != 0)
                {
                    InitializeProperties(obj, objType.GetProperties());
                }
            }
            
            _innerDTOTypes.RemoveAt(_innerDTOTypes.Count - 1);
            
            return obj;
            
        }
        
        public T Create<T>()
        {
            return (T) CreateDto(typeof(T));
        }
        
        public  dynamic GenerateValue(Type type)
        {
            Func<dynamic> generator;

            if (type.Name == "List`1")
            {
                return ListGenerator(type);
            }
            
            if (_generators.TryGetValue(type.Name, out generator))
            {
                return generator.Invoke();
                
            }

            return null;

        }

        public IList ListGenerator(Type type)
        {
            
            Type innerType = type.GetTypeInfo().GenericTypeArguments[0];

            var constructor = type.GetConstructor(Type.EmptyTypes);
            
            if (constructor == null)
            {
                return null;
            }

            IList list = (IList) constructor.Invoke(null);

            for (int i = 0; i < 10; i++)
            {
                if (!_innerDTOTypes.Contains(innerType) && !type.IsValueType)
                {
                    list.Add(CreateDto(innerType));
                }
                else
                {
                    list.Add(GenerateValue(innerType));
                }
            }

            return list;
            
        }
        
        
    }
}