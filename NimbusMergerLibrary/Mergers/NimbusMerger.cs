using Ace7LocalizationFormat.Formats;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Pak;
using CUE4Parse.UE4.Versions;
using NimbusMergerLibrary.FileProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;

namespace NimbusMergerLibrary.Mergers
{
    public class NimbusMerger
    {
        private readonly string _gameArchivePath;
        private readonly string _modArchivePath;

        private NimbusFileProvider _gameProvider;
        private NimbusFileProvider _modProvider;

        private PlayerPlaneDataTableMerger _playerPlaneDataTableMerger;
        private SkinDataTableMerger _skinDataTableMerger;
        private AircraftViewerDataTableMerger _aircraftViewerDataTableMerger;
        private PlayerWeaponDataTableMerger _playerWeaponDataTableMerger;

        private CmnFile _gameCmn;
        private Dictionary<char, DatFile> _gameDats = new Dictionary<char, DatFile>();

        private int _tildeCount = 0;
        public int TildeCount
        {
            get { return _tildeCount + 1; }
            set { _tildeCount = value; }
        }

        public NimbusMerger(string gameArchivePath, string modArchivePath)
        {
            _gameArchivePath = gameArchivePath;
            _modArchivePath = modArchivePath;
        }

        public void Initialize()
        {
            ZlibHelper.Initialize(Directory.GetCurrentDirectory() + "\\" + "zlib-ng2.dll");

            // Initialize Game Provider
            _gameProvider = new NimbusFileProvider(_gameArchivePath, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_AceCombat7));
            _gameProvider.Initialize();
            _gameProvider.SubmitKey(new(0U), new FAesKey("68747470733a2f2f616365372e616365636f6d6261742e6a702f737065636961"));

            // Load game data tables
            var gamePlayerPlaneDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset", true);
            var gameSkinDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/SkinDataTable.uasset", true);
            var gameAircraftViewerDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/AircraftViewerDataTable.uasset", true);
            var gamePlayerWeaponDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/PlayerWeaponDataTable.uasset", true);

            _gameCmn = _gameProvider.GetCmn();
            _gameDats = _gameProvider.GetLocalizations();

            // Initialize mergers
            _playerPlaneDataTableMerger = new PlayerPlaneDataTableMerger(gamePlayerPlaneDataTable, _gameCmn, _gameDats['A']);
            _skinDataTableMerger = new SkinDataTableMerger(gameSkinDataTable);
            _aircraftViewerDataTableMerger = new AircraftViewerDataTableMerger(gameAircraftViewerDataTable);
            _playerWeaponDataTableMerger = new PlayerWeaponDataTableMerger(gamePlayerWeaponDataTable);

            // Initialize Mod Provider
            _modProvider = new NimbusFileProvider(_modArchivePath, SearchOption.AllDirectories, true, new VersionContainer(EGame.GAME_UE4_18));
            _modProvider.Initialize();
            _modProvider.SubmitKey(new(0U), new FAesKey("0000000000000000000000000000000000000000000000000000000000000000"));

            // Get both folder and files name
            var names = Directory.GetDirectories(_modArchivePath)
                .Select(Path.GetFileName)
                .Concat(Directory.GetFiles(_modArchivePath).Select(Path.GetFileName));

            if (names.Any())
            {
                TildeCount = names.OrderByDescending(f => f)
                    .LastOrDefault()
                    .Count(c => c == '~');
            }
        }

        public void MergeLocalization()
        {
            var sorted = _modProvider.MountedVfs.OrderBy(x => x.Name).ToList();

            var gameMaxStringNumber = _gameCmn.MaxStringNumber;

            foreach (PakFileReader pak in sorted)
            {
                var modCmn = NimbusPakFileReader.GetCmn(pak);
                var modDats = NimbusPakFileReader.GetLocalizations(pak);

                if (modCmn != null)
                {
                    LocalizationMerger.MergeCmn(_gameCmn, modCmn, _gameDats, modDats, gameMaxStringNumber, modCmn.Root);
                }
            }
        }

        public void MergeDataTables() 
        {
            var sorted = _modProvider.MountedVfs.OrderBy(x => x.Name).ToList();

            foreach (PakFileReader pak in sorted)
            {
                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset", out UAsset modPlayerPlaneDataTable))
                {
                    _playerPlaneDataTableMerger.Merge(_modProvider, modPlayerPlaneDataTable, _gameCmn, _gameDats['A']);
                }

                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/SkinDataTable.uasset", out UAsset modSkinDataTable))
                {
                    _skinDataTableMerger.Merge(_modProvider, modSkinDataTable);
                }

                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/AircraftViewerDataTable.uasset", out UAsset modAircraftViewerDataTable))
                {
                    _aircraftViewerDataTableMerger.Merge(modAircraftViewerDataTable);
                }

                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/PlayerWeaponDataTable.uasset", out UAsset modPlayerWeaponDataTable))
                {
                    _playerWeaponDataTableMerger.Merge(modPlayerWeaponDataTable);
                }
            }
        }

        public void WriteMergedLocalization(string path)
        {
            _gameCmn.Write(path + "\\Nimbus\\Content\\Localization\\Game\\Cmn.dat");
            foreach (DatFile gameDat in _gameDats.Values)
            {
                gameDat.Write(path + "\\Nimbus\\Content\\Localization\\Game\\");
            }
        }

        public void WriteMergedDataTables(string path)
        {
            _playerPlaneDataTableMerger.Write(path);
            _skinDataTableMerger.Write(path);
            _aircraftViewerDataTableMerger.Write(path);
            _playerWeaponDataTableMerger.Write(path);
        }

        public void WritePak(string batFilePath, string path)
        {
            // Create and configure the process
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batFilePath,
                    Arguments = $"\"{path.TrimEnd('\\')}\"",  // Pass folder path as an argument
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Start the process
            process.Start();
            // Wait for the batch file to finish
            process.WaitForExit();

#if !DEBUG
            Directory.Delete(path);
#endif
        }
    }
}
