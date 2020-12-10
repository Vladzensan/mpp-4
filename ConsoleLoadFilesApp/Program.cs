using System.IO;
using System.Threading.Tasks.Dataflow;
using ClassGeneration;

namespace ConsoleLoadFilesApp
{
    class Program
    {
        static string inPath = @"C:\5 semester\MPP\Lab_4\Test Files";

        static void Main(string[] args)
        {
            PipelineFileHandler pipeline = new PipelineFileHandler();

            ITargetBlock<string> headBlock = null;

            headBlock ??= pipeline.CreateTestGeneration();

            foreach (string fileName in Directory.GetFiles(inPath, "*.cs"))
            {
                headBlock.Post(fileName);
            }

            while (true)
            {

            }

        }

    }
}
