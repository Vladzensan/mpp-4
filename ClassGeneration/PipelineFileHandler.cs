using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ClassGeneration
{
    public class PipelineFileHandler
    {
        static string outPath = @"C:\5 semester\MPP\Lab_4\Generated Tests";
        static int counter = 0;

        private async Task<List<string>> LoadFileAsync(string path)
        {
            var list = new List<string>();
            list.Add(await File.ReadAllTextAsync(path));
            list.Add(Path.GetFileName(path));
            return list;
        }

        private async Task<string> CreateTestFile(Task<List<string>> fileData)
        {
            return await TestClassGenerator.GenerateCodeAsync(fileData);
        }

        private async void WriteFileDiskAsync(string code)
        {
            counter += 1;
            string fileName = outPath + @"\TestFile" + counter + @".cs";
            await File.WriteAllTextAsync(fileName, code);
        }

        public ITargetBlock<string> CreateTestGeneration()
        {
            var loadFiles = new TransformBlock<string, Task<List<string>>>(path =>
            {

                return LoadFileAsync(path);

            },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 8
                }
            );

            var createTestFile = new TransformBlock<Task<List<string>>, string>(file =>
            {

                return CreateTestFile(file);

            },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 8
                }
            );

            var writeFileToDisk = new ActionBlock<string>(code =>
            {

                WriteFileDiskAsync(code);

            },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 8
                }
            );

            loadFiles.LinkTo(createTestFile, path => path != null);
            createTestFile.LinkTo(writeFileToDisk, code => code != null);

            return loadFiles;
        }
    }
}
