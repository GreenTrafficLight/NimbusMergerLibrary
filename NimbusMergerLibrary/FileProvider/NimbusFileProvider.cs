using CUE4Parse.FileProvider.Objects;
using CUE4Parse.FileProvider.Vfs;
using CUE4Parse.UE4.Pak;
using CUE4Parse.UE4.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.UnrealTypes;
using UAssetAPI;
using Ace7LocalizationFormat.Formats;
using System.IO;
using Ace7LocalizationFormat.Enums;
using UAssetAPI.PropertyTypes.Objects;
using CUE4Parse.FileProvider;

namespace NimbusMergerLibrary.FileProvider
{
    public class NimbusFileProvider : AbstractVfsFileProvider
    {
        private readonly DirectoryInfo _workingDirectory;
        private readonly DirectoryInfo[] _extraDirectories;
        private readonly SearchOption _searchOption;

        public NimbusFileProvider(string directory, SearchOption searchOption, bool isCaseInsensitive = false, VersionContainer? versions = null)
            : this(new DirectoryInfo(directory), searchOption, isCaseInsensitive, versions) { }
        public NimbusFileProvider(DirectoryInfo directory, SearchOption searchOption, bool isCaseInsensitive = false, VersionContainer? versions = null)
            : this(directory, Array.Empty<DirectoryInfo>(), searchOption, isCaseInsensitive, versions) { }
        public NimbusFileProvider(DirectoryInfo directory, DirectoryInfo[] extraDirectories, SearchOption searchOption, bool isCaseInsensitive = false, VersionContainer? versions = null)
            : base(isCaseInsensitive, versions)
        {
            _workingDirectory = directory;
            _extraDirectories = extraDirectories;
            _searchOption = searchOption;
        }

        public override void Initialize()
        {
            foreach (var file in _workingDirectory.GetFiles())
            {
                RegisterVfs(file);
            }
        }

        public bool TryGetGameFile(string path, out GameFile gameFile)
        {
            path = FixPath(path);

            gameFile = null;
            foreach (PakFileReader pak in MountedVfs.OrderBy(x => x.Name).ToList())
            {
                if (pak.Files.ContainsKey(path))
                {
                    gameFile = pak.Files[path];
                    return true;
                }
            }
            return false;
        }

        public bool TryGetUasset(string path, bool crypted, out UAsset gameUasset)
        {
            gameUasset = null;

            GameFile uasset;
            GameFile uexp;
            if (!TryGetGameFile(path, out uasset))
            {
                return false;
            }
            if (!TryGetGameFile(Path.ChangeExtension(path, "uexp"), out uexp))
            {
                return false;
            }

            byte[] uassetData = uasset.Read();
            byte[] uexpData = uexp.Read();

            if (crypted)
            {
                // Decrypt the asset
                AC7Decrypt ac7decrypt = new AC7Decrypt();
                var xorKey = new AC7XorKey(Path.GetFileNameWithoutExtension(path));
                uassetData = ac7decrypt.DecryptUAssetBytes(uassetData, xorKey);
                uexpData = ac7decrypt.DecryptUexpBytes(uexpData, xorKey);
            }

            byte[] combinedData = new byte[uassetData.Length + uexpData.Length];
            Array.Copy(uassetData, 0, combinedData, 0, uassetData.Length);
            Array.Copy(uexpData, 0, combinedData, uassetData.Length, uexpData.Length);

            AssetBinaryReader ar = new AssetBinaryReader(new MemoryStream(combinedData));

            gameUasset = new UAsset(ar, EngineVersion.VER_UE4_18, null, true);

            return true;
        }

        public UAsset GetUasset(string path, bool crypted)
        {
            GameFile uasset;
            GameFile uexp;
            if (!TryGetGameFile(path, out uasset))
            {
                return null;
            }
            if (!TryGetGameFile(Path.ChangeExtension(path, "uexp"), out uexp))
            {
                return null;
            }

            byte[] uassetData = uasset.Read();
            byte[] uexpData = uexp.Read();

            if (crypted)
            {
                // Decrypt the asset
                AC7Decrypt ac7decrypt = new AC7Decrypt();
                var xorKey = new AC7XorKey(Path.GetFileNameWithoutExtension(path));
                uassetData = ac7decrypt.DecryptUAssetBytes(uassetData, xorKey);
                uexpData = ac7decrypt.DecryptUexpBytes(uexpData, xorKey);
            }

            byte[] combinedData = new byte[uassetData.Length + uexpData.Length];
            Array.Copy(uassetData, 0, combinedData, 0, uassetData.Length);
            Array.Copy(uexpData, 0, combinedData, uassetData.Length, uexpData.Length);

            AssetBinaryReader ar = new AssetBinaryReader(new MemoryStream(combinedData));

            return new UAsset(ar, EngineVersion.VER_UE4_18, null, true);
        }

        public CmnFile GetCmn()
        {
            if (TryGetGameFile("Nimbus/Content/Localization/Game/Cmn.dat", out GameFile gameFile))
            {
                return new CmnFile(gameFile.Read());
            }
            return null;
        }

        public DatFile GetLocalization(eLanguage language)
        {
            if (TryGetGameFile("Nimbus/Content/Localization/Game/" + (char)language + ".dat", out GameFile gameFile))
            {
                return new DatFile(gameFile.Read(), (char)language);
            }
            return null;
        }

        public Dictionary<char, DatFile> GetLocalizations()
        {
            Dictionary<char, DatFile> datFiles = new Dictionary<char, DatFile>();
            foreach (eLanguage language in Enum.GetValues<eLanguage>())
            {
                if (TryGetGameFile("Nimbus/Content/Localization/Game/" + (char)language + ".dat", out GameFile gameFile))
                {
                    datFiles.Add((char)language, new DatFile(gameFile.Read(), (char)language));
                }
            }
            return datFiles;
        }

        public bool CheckAssetReference(SoftObjectPropertyData reference)
        {
            string assetPathName = reference.Value.AssetPath.AssetName.ToString();
            string assetPath = Path.GetDirectoryName(assetPathName) + "\\" + Path.GetFileNameWithoutExtension(assetPathName) + ".uasset";
            if (CheckAsset(assetPath)) return true;
            return false;
        }

        public bool CheckAssetReference(string assetPath)
        {
            if (CheckAsset(assetPath)) return true;
            return false;
        }

        public bool CheckAsset(string assetPath)
        {
            if (assetPath.StartsWith("\\Game"))
            {
                string updatedPath = assetPath.Replace("\\Game", "Nimbus\\Content");
                // If the asset doesn't exist
                if (!TryFindGameFile(updatedPath, out GameFile gameFile))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
