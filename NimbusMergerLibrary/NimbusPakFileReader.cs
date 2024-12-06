using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UAssetAPI;
using UAssetAPI.UnrealTypes;
using CUE4Parse.FileProvider.Objects;
using Ace7LocalizationFormat.Formats;
using Ace7LocalizationFormat.Enums;
using CUE4Parse.UE4.Pak;

namespace NimbusMergerLibrary
{
    public static class NimbusPakFileReader
    {
        public static bool TryGetUAsset(PakFileReader pak, string path, out UAsset gameUasset)
        {
            path = path.ToLower();

            gameUasset = null;

            GameFile uasset;
            GameFile uexp;
            if (!pak.Files.TryGetValue(path, out uasset)){
                return false;
            }
            if (!pak.Files.TryGetValue(Path.ChangeExtension(path, "uexp"), out uexp)){
                return false;
            }

            byte[] uassetData = uasset.Read();
            byte[] uexpData = uexp.Read();

            byte[] combinedData = new byte[uassetData.Length + uexpData.Length];
            Array.Copy(uassetData, 0, combinedData, 0, uassetData.Length);
            Array.Copy(uexpData, 0, combinedData, uassetData.Length, uexpData.Length);

            AssetBinaryReader ar = new AssetBinaryReader(new MemoryStream(combinedData));
            gameUasset = new UAsset(ar, EngineVersion.VER_UE4_18, null, true);

            return true;
        }

        public static CmnFile GetCmn(PakFileReader pak)
        {
            if (pak.Files.TryGetValue("Nimbus/Content/Localization/Game/Cmn.dat".ToLower(), out GameFile gameFile)){
                return new CmnFile(gameFile.Read());
            }
            return null;
        }

        public static DatFile GetLocalization(PakFileReader pak, eLanguage language)
        {
            if (pak.Files.TryGetValue(("Nimbus/Content/Localization/Game/" + (char)language + ".dat").ToLower(), out GameFile gameFile)){
                return new DatFile(gameFile.Read(), (char)language);
            }
            return null;
        }

        public static Dictionary<char, DatFile> GetLocalizations(PakFileReader pak)
        {
            Dictionary<char, DatFile> datFiles = new Dictionary<char, DatFile>();
            foreach (eLanguage language in Enum.GetValues<eLanguage>())
            {
                if (pak.Files.TryGetValue(("Nimbus/Content/Localization/Game/" + (char)language + ".dat").ToLower(), out GameFile gameFile)){
                    datFiles.Add((char)language, new DatFile(gameFile.Read(), (char)language));
                }
            }
            return datFiles;
        }
    }
}
