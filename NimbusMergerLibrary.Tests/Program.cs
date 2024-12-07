﻿using NimbusMergerLibrary.Mergers;

namespace NimbusMergerLibrary.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string gameArchivePath = "E:\\Program Files (x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks";
            string modArchivePath = "E:\\Program Files (x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks\\~mods";
            string exportPath = "E:\\MODDING\\_Ace Combat 7\\_testForMerger\\~~~~~~~MERGER_TEST_P\\";

            NimbusMerger nimbusMerger = new NimbusMerger(gameArchivePath, modArchivePath);

            nimbusMerger.Initialize();

            nimbusMerger.MergeLocalization();
            nimbusMerger.MergeDataTables();

            nimbusMerger.WriteMergedLocalization(exportPath);
            nimbusMerger.WriteMergedDataTables(exportPath);

            watch.Stop();
            Console.WriteLine($"Completed in {watch.ElapsedMilliseconds} ms");
        }
    }
}
