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

namespace NimbusMergerLibrary.Mergers
{
    public class SkinDataTableMerger : DataTableMerger
    {
        private List<int> _gameSkinIds = new List<int>();
        private List<int> _exportSkinIds;

        private int _skindId = 101;      
        private Dictionary<string, List<int>> _planeSkinIds = new Dictionary<string, List<int>>();

        public SkinDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "Nimbus\\Content\\Blueprint\\Information\\SkinDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            // Initialize the dictionary
            foreach (StructPropertyData data in gameDatas)
            {
                IntPropertyData skinId = (IntPropertyData)data["SkinID"];
                StrPropertyData planeStringId = (StrPropertyData)data["PlaneStringID"];
                
                if (!_planeSkinIds.ContainsKey(planeStringId.ToString())){
                    _planeSkinIds.Add(planeStringId.ToString(), new List<int>());
                }
                _planeSkinIds[planeStringId.ToString()].Add(skinId.Value);

                _gameSkinIds.Add(skinId.Value);
                if (_gameSkinIds.Contains(_skindId)) {
                    _skindId += 100;
                }
            }

            _exportSkinIds = new List<int>(_gameSkinIds);
        }

        private void AddRow(StructPropertyData modData, List<StructPropertyData> gameDatas)
        {
        }

        public void Merge(UAsset modAsset)
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;
            var gameNameMap = _gameAsset.GetNameMapIndexList();

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

                // If the new plane hasn't been added
                if (!_planeSkinIds.ContainsKey(planeStringId.ToString()))
                {
                    _planeSkinIds.Add(planeStringId.ToString(), new List<int>());

                    skinNo.Value = 0; // The first skin
                    skinId.Value = _skindId;
                    sortNumber.Value = skinId.Value;

                    _planeSkinIds[planeStringId.ToString()].Add(skinId.Value);
                    _exportSkinIds.Add(skinId.Value);

                    modData.Name.Number = skinId.Value + 1; // Change row name
                    gameDatas.Add(modData);

                    _skindId += 100;
                }
                else if (!_gameSkinIds.Contains(skinId.Value))
                {
                    skinNo.Value = _planeSkinIds[planeStringId.ToString()].Count();
                    skinId.Value = _planeSkinIds[planeStringId.ToString()].Last() + 1;
                    sortNumber.Value = skinId.Value;

                    _planeSkinIds[planeStringId.ToString()].Add(skinId.Value);
                    _exportSkinIds.Add(skinId.Value);

                    modData.Name.Number = skinId.Value + 1; // Change row name
                    gameDatas.Add(modData);
                }
            }
        }
    }
}
