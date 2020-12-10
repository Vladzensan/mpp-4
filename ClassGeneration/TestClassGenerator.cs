using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassGeneration
{
    public static class TestClassGenerator
    {
        public static async Task<string> GenerateCodeAsync(Task<List<string>> code)
        {
            List<NamespaceData> namespaces = new List<NamespaceData>();
            SyntaxTree tree = CSharpSyntaxTree.ParseText((await code)[0]);

            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            foreach (NamespaceDeclarationSyntax namespaceDeclarationSyntax in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>())
            {
                namespaces.Add(new NamespaceData(namespaceDeclarationSyntax.Name.ToString()));
                foreach (ClassDeclarationSyntax classDeclarationSyntax in namespaceDeclarationSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    namespaces.Last().ClassList.Add(new ClassData(classDeclarationSyntax.Identifier.ToString()));
                    foreach (MethodDeclarationSyntax methodDeclarationSyntax in classDeclarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList().Where(method => method.Modifiers.Any(SyntaxKind.PublicKeyword)))
                    {
                        namespaces.Last().ClassList.Last().MethodList.Add(new MethodData(methodDeclarationSyntax.Identifier.ToString()));
                    }
                }
            }
            return CodeGeneratorHelper.GenerateTestCode(namespaces, (await code)[1]);
        }


    }


}
