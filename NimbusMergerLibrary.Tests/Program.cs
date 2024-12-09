using NimbusMergerLibrary.Mergers;

namespace NimbusMergerLibrary.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string gameArchivePath = "E:\\Program Files (x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks";
            string modArchivePath = "E:\\Program Files (x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks\\~mods";
            string exportPath = "E:\\MODDING\\_Ace Combat 7\\_testForMerger";

            NimbusMerger nimbusMerger = new NimbusMerger(gameArchivePath, modArchivePath);

            nimbusMerger.Initialize();

            exportPath += "\\" + new string('~', nimbusMerger.TildeCount) + "export\\" + "export_P";

            nimbusMerger.MergeLocalization();
            nimbusMerger.MergeDataTables();

            nimbusMerger.WriteMergedLocalization(exportPath);
            nimbusMerger.WriteMergedDataTables(exportPath);
            nimbusMerger.WritePak(Directory.GetCurrentDirectory() + "\\" + "UnrealPak-Batch-No-Compression.bat", exportPath);

            watch.Stop();
            Console.WriteLine($"Completed in {watch.ElapsedMilliseconds} ms");
        }
    }
}
