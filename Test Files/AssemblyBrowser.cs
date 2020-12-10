using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AssemblyBrowser
{
    public class AssemblyBrowser
    {
        private BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

        public AssemblyData assemblyData { get; private set; }

        public AssemblyBrowser(string path)
        {
            var assembly = Assembly.LoadFrom(path);

            assemblyData = new AssemblyData();

            var nameSpaces = assemblyData.NameSpaces;

            SetNameSpaces(nameSpaces, assembly);
        }


        private void SetNameSpaces(Dictionary<string, NameSpaceData> nameSpaces, Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Namespace != null)
                {
                    NameSpaceData nameSpaceData;

                    if (!nameSpaces.TryGetValue(type.Namespace, out nameSpaceData))
                    {
                        nameSpaceData = new NameSpaceData();
                        nameSpaces.Add(type.Namespace, nameSpaceData);
                    }

                    var types = nameSpaceData.TypesList;

                    SetTypes(types, type);

                }
            }
        }

        private void SetTypes(List<TypeData> types, Type type)
        {
            TypeData typeData = GetTypeData(types, type);

            if (typeData == null)
            {
                typeData = new TypeData(type.Name);
            }

            var methods = typeData.Methods;

            SetMethods(types, methods, type);

            var fields = typeData.Fields;

            SetFields(fields, type);

            var properties = typeData.Properties;

            SetProperties(properties, type);

            types.Add(typeData);
        }

        private TypeData GetTypeData(List<TypeData> types, Type type)
        {
            TypeData data = null;
            foreach (TypeData typeData in types)
            {
                if (typeData.Name == type.Name)
                {
                    data = typeData;
                    break;
                }
            }
            return data;
        }

        private void SetMethods(List<TypeData> types, List<MethodData> methods, Type type)
        {
            foreach (MethodInfo methodInfo in type.GetMethods(bindingFlags))
            {
                var parameters = GetMethodParams(methodInfo);

                string accessModifier;

                if (methodInfo.IsPublic)
                {
                    accessModifier = "Public";
                }
                else
                {
                    accessModifier = "Non-public";
                }

                if (methodInfo.IsDefined(typeof(ExtensionAttribute), true))
                {
                    var typeData = GetTypeData(types, methodInfo.GetParameters()[0].ParameterType);

                    if (typeData != null)
                    {
                        typeData.Methods.Add(new MethodData(accessModifier, methodInfo.Name, parameters, methodInfo.ReturnType.Name));
                    }
                    else
                    {
                        TypeData newTypeData = new TypeData(type.Name);
                        newTypeData.Methods.Add(new MethodData(accessModifier, methodInfo.Name, parameters, methodInfo.ReturnType.Name));
                        types.Add(newTypeData);
                    }
                }
                else
                {
                    methods.Add(new MethodData(accessModifier, methodInfo.Name, parameters, methodInfo.ReturnType.Name));
                }

            }
        }

        private Dictionary<string, string> GetMethodParams(MethodInfo methodInfo)
        {
            var parameters = new Dictionary<string, string>();

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                parameters.Add(parameterInfo.Name, parameterInfo.ParameterType.Name);
            }

            return parameters;
        }

        private void SetFields(List<FieldData> fields, Type type)
        {
            foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
            {
                if (fieldInfo.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) == null)
                {
                    string accessModifier;

                    if (fieldInfo.IsPublic)
                    {
                        accessModifier = "Public";
                    }
                    else
                    {
                        accessModifier = "Non-public";
                    }

                    fields.Add(new FieldData(accessModifier, fieldInfo.Name, fieldInfo.FieldType.Name));
                }
            }
        }

        private void SetProperties(List<PropertyData> properties, Type type)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties(bindingFlags))
            {
                properties.Add(new PropertyData(propertyInfo.Name, propertyInfo.PropertyType.Name));
            }
        }

    }
}
