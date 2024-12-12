using Ace7LocalizationFormat.Formats;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Versions;
using NimbusMergerLibrary.FileProvider;
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

            // Initialize Game Provider
            NimbusFileProvider gameProvider = new NimbusFileProvider(gameArchivePath, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_AceCombat7));
            gameProvider.Initialize();
            gameProvider.SubmitKey(new(0U), new FAesKey("68747470733a2f2f616365372e616365636f6d6261742e6a702f737065636961"));

            CmnFile gameCmn = gameProvider.GetCmn();
            var gameDats = gameProvider.GetLocalizations();

            // Initialize Mod Provider
            NimbusFileProvider modProvider = new NimbusFileProvider(modArchivePath, SearchOption.AllDirectories, true, new VersionContainer(EGame.GAME_UE4_18));
            modProvider.Initialize();
            modProvider.SubmitKey(new(0U), new FAesKey("0000000000000000000000000000000000000000000000000000000000000000"));

            CmnFile modCmn = modProvider.GetCmn();
            var modDats = modProvider.GetLocalizations();

            if (modCmn != null)
            {
                StreamWriter sw = new StreamWriter("E:\\MODDING\\_Ace Combat 7\\_testForMerger\\test.txt");
                Test(gameCmn, modCmn, gameDats, modDats, gameCmn.MaxStringNumber, modCmn.Root, sw);
                sw.Close();
            }

            watch.Stop();
            Console.WriteLine($"Completed in {watch.ElapsedMilliseconds} ms");
        }

        private static void Test(CmnFile exportCmn, CmnFile modCmn, Dictionary<char, DatFile> exportDats, Dictionary<char, DatFile> modDats, int gameMaxStringNumber, SortedDictionary<string, CmnString> parent, StreamWriter sw)
        {
            // Iterate through all the strings of the Cmn
            foreach (string key in parent.Keys)
            {
                // If it's a new variable
                if (gameMaxStringNumber < parent[key].StringNumber)
                {
                    // Add it to the Cmn that is going to be exported
                    if (exportCmn.AddVariable(parent[key], exportCmn.Root))
                    {
                        sw.WriteLine($"add_string_id {parent[key]}");
                        // Iterate through each localization contained in the game
                        foreach (char letter in exportDats.Keys)
                        {
                            string newString = "\0";
                            // Check if the mod contains that localization
                            if (modDats.ContainsKey(letter))
                            {
                                newString = modDats[letter].Strings.Count <= parent[key].StringNumber
                                    ? "\0"
                                    : modDats[letter].Strings[parent[key].StringNumber];
                                
                            }
                            sw.WriteLine($"add_string {letter} {parent[key]} {newString}");
                        }
                    }
                }
                Test(exportCmn, modCmn, exportDats, modDats, gameMaxStringNumber, parent[key].Childrens, sw);
            }

            
        }

        private void Merge()
        {
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
        }
    }
}
