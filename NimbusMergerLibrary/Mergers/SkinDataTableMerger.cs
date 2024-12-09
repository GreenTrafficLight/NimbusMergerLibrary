using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI;
using System.Numerics;
using CUE4Parse.FileProvider;
using NimbusMergerLibrary.FileProvider;
using CUE4Parse.UE4.Assets.Objects.Properties;

namespace NimbusMergerLibrary.Mergers
{
    public class PlaneSkinDictionary
    {
        public PlaneSkinDictionary(int rowIndex) 
        {
            RowIndex = rowIndex;
        }
        
        public int RowIndex { get; set; }
        
        public List<int> SkinIDs = new List<int>();
        public List<int> SkinNo = new List<int>();

        
    }

    public class SkinDataTableMerger : DataTableMerger
    {
        private readonly List<int> _defaultSkinsNo = new List<int> { 0, 1, 2, 3, 4, 5 };

        private HashSet<int> _gameSkinIds = new HashSet<int>();
        private HashSet<int> _exportSkinIds;

        private int _skindId = 101;      
        private Dictionary<string, PlaneSkinDictionary> _planeSkinDictionary = new Dictionary<string, PlaneSkinDictionary>();

        public SkinDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\SkinDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            // Initialize the dictionary
            for (int i = 0; i < gameDatas.Count; i++)
            {
                StructPropertyData row = gameDatas[i];

                IntPropertyData skinId = (IntPropertyData)row["SkinID"];
                IntPropertyData skinNo = (IntPropertyData)row["SkinNo"];
                StrPropertyData planeStringId = (StrPropertyData)row["PlaneStringID"];

                if (!_planeSkinDictionary.ContainsKey(planeStringId.ToString()))
                {
                    _planeSkinDictionary.Add(planeStringId.ToString(), new PlaneSkinDictionary(i));
                }
                _planeSkinDictionary[planeStringId.ToString()].SkinIDs.Add(skinId.Value);
                _planeSkinDictionary[planeStringId.ToString()].SkinNo.Add(skinNo.Value);

                _gameSkinIds.Add(skinId.Value);
                if (_gameSkinIds.Contains(_skindId))
                {
                    _skindId += 100;
                }
            }

            _exportSkinIds = new HashSet<int>(_gameSkinIds);
        }

        private void AddRow(StructPropertyData modData, List<StructPropertyData> gameDatas)
        {
        }

        public void Merge(NimbusFileProvider fileProvider, UAsset modAsset)
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            DataTableExport modDataTable = (DataTableExport)modAsset.Exports[0];
            UDataTable modTable = modDataTable.Table;
            List<StructPropertyData> modDatas = modTable.Data;

            for (int i = 0; i < modDatas.Count; i++)
            {
                StructPropertyData modData = modDatas[i];

                IntPropertyData skinId = (IntPropertyData)modData["SkinID"];
                IntPropertyData skinNo = (IntPropertyData)modData["SkinNo"];
                IntPropertyData sortNumber = (IntPropertyData)modData["SortNumber"];
                StrPropertyData planeStringId = (StrPropertyData)modData["PlaneStringID"];
                StrPropertyData planeReference = (StrPropertyData)modData["PlaneReference"];

                // If the new plane hasn't been added in the dictionary
                if (!_planeSkinDictionary.ContainsKey(planeStringId.ToString()))
                {
                    string assetPath = Path.GetDirectoryName(planeReference.Value.ToString()) + "\\" + Path.GetFileNameWithoutExtension(planeReference.Value.ToString()) + ".uasset";
                    if (!fileProvider.CheckAssetReference(assetPath)) continue;

                    _planeSkinDictionary.Add(planeStringId.ToString(), new PlaneSkinDictionary(i));

                    skinNo.Value = _defaultSkinsNo.Contains(skinNo.Value) ? skinNo.Value : 6;
                    skinId.Value = _skindId;
                    sortNumber.Value = skinId.Value;

                    _planeSkinDictionary[planeStringId.ToString()].SkinIDs.Add(skinId.Value);
                    _planeSkinDictionary[planeStringId.ToString()].SkinNo.Add(skinNo.Value);
                    _exportSkinIds.Add(skinId.Value);

                    modData.Name.Number = skinId.Value + 1; // Change row name
                    gameDatas.Add(modData);

                    _skindId += 100;
                }
                else if (!_gameSkinIds.Contains(skinId.Value))
                {
                    // Check if it's a skin No that has already been assigned
                    if (_planeSkinDictionary[planeStringId.ToString()].SkinNo.Contains(skinNo.Value)) continue;

                    skinNo.Value = _defaultSkinsNo.Contains(skinNo.Value) ? skinNo.Value : 6 + skinNo.Value - 6;
                    skinId.Value = _planeSkinDictionary[planeStringId.ToString()].SkinIDs.Last() + 1;
                    sortNumber.Value = skinId.Value;

                    _planeSkinDictionary[planeStringId.ToString()].SkinIDs.Add(skinId.Value);
                    _planeSkinDictionary[planeStringId.ToString()].SkinNo.Add(skinNo.Value);
                    _exportSkinIds.Add(skinId.Value);

                    modData.Name.Number = skinId.Value + 1; // Change row name

                    gameDatas.Add(modData);
                }
            }
        }
    }
}
