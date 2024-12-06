using Ace7LocalizationFormat.Formats;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Pak;
using CUE4Parse.UE4.Versions;
using System;
using System.Collections.Generic;
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

        private CmnFile _gameCmn;
        private Dictionary<char, DatFile> _gameDats = new Dictionary<char, DatFile>();

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

            _gameCmn = _gameProvider.GetCmn();
            _gameDats = _gameProvider.GetLocalizations();

            // Initialize mergers
            _playerPlaneDataTableMerger = new PlayerPlaneDataTableMerger(gamePlayerPlaneDataTable);
            _skinDataTableMerger = new SkinDataTableMerger(gameSkinDataTable);
            _aircraftViewerDataTableMerger = new AircraftViewerDataTableMerger(gameAircraftViewerDataTable);

            // Initialize Mod Provider
            _modProvider = new NimbusFileProvider(_modArchivePath, SearchOption.AllDirectories, true, new VersionContainer(EGame.GAME_UE4_18));
            _modProvider.Initialize();
            _modProvider.SubmitKey(new(0U), new FAesKey("0000000000000000000000000000000000000000000000000000000000000000"));
        }

        public void MergeLocalization()
        {
            var gameMaxStringNumber = _gameCmn.MaxStringNumber;

            foreach (PakFileReader pak in _modProvider.MountedVfs.OrderBy(x => x.Name).ToList())
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
            foreach (PakFileReader pak in _modProvider.MountedVfs.OrderBy(x => x.Name).ToList())
            {
                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset", out UAsset modPlayerPlaneDataTable))
                {
                    _playerPlaneDataTableMerger.Merge(_modProvider, modPlayerPlaneDataTable, _gameCmn, _gameDats['A']);
                }

                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/SkinDataTable.uasset", out UAsset modSkinDataTable))
                {
                    _skinDataTableMerger.Merge(modSkinDataTable);
                }

                if (NimbusPakFileReader.TryGetUAsset(pak, "Nimbus/Content/Blueprint/Information/AircraftViewerDataTable.uasset", out UAsset modAircraftViewerDataTable))
                {
                    _aircraftViewerDataTableMerger.Merge(modAircraftViewerDataTable);
                }
            }
        }

        public void WriteMergedLocalization(string path)
        {
            _gameCmn.Write(path + "Nimbus\\Content\\Localization\\Game\\Cmn.dat");
            foreach (DatFile gameDat in _gameDats.Values)
            {
                gameDat.Write(path + "Nimbus\\Content\\Localization\\Game\\");
            }
        }

        public void WriteMergedDataTables(string path)
        {
            _playerPlaneDataTableMerger.Write(path);
            _skinDataTableMerger.Write(path);
            _aircraftViewerDataTableMerger.Write(path);
        }
    }
}
