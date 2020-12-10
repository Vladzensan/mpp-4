namespace ClassGeneration
{
    public class CSharpFileData
    {
        public string Name { private set; get; }
        
        public string Code { private set; get; }

        public CSharpFileData(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
