using Ace7LocalizationFormat.Formats;
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
    public class PlayerWeaponDataTableMerger : DataTableMerger
    {
        /// <summary>The plane IDs belonging to the game asset. Used to see if there is a new plane added</summary>
        private List<int> _gameWeaponIDs = new List<int>();
        /// <summary>The plane IDs belonging to the asset that is going to be created.</summary>
        private List<int> _exportWeaponIDs;

        private int _addWeaponID = 0;

        private StructPropertyData _rowForCopy = null;


        public PlayerWeaponDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\PlayerWeaponDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            _rowForCopy = gameDatas[0];

            // Initialize the dictionary
            foreach (StructPropertyData data in gameDatas)
            {
                // Add the game plane IDs
                IntPropertyData planeId = (IntPropertyData)data["WeaponID"];
                _gameWeaponIDs.Add(planeId.Value);
                // Increment the id for the new plane, so it's doesn't take one that exist
                if (_gameWeaponIDs.Contains(_addWeaponID))
                {
                    _addWeaponID++;
                }
            }

            _exportWeaponIDs = new List<int>(_gameWeaponIDs);
        }

        public void Merge(UAsset modAsset)
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

                IntPropertyData weaponID = (IntPropertyData)modData["WeaponID"];

                if (!_gameWeaponIDs.Contains(weaponID.Value))
                {
                    // Update the added id value until it's one that doesn't exist in the exported asset
                    while (_exportWeaponIDs.Contains(_addWeaponID))
                    {
                        _addWeaponID++;
                    }

                    weaponID.Value = _addWeaponID;
                    _exportWeaponIDs.Add(weaponID.Value);

                    // Copy the first row of the game asset
                    StructPropertyData outputRow = (StructPropertyData)_rowForCopy.Clone();
                    CopyRow(modData, outputRow);
                    outputRow.Name.Number = weaponID.Value + 1; // Change row name

                    gameDatas.Add(outputRow); // Add the table
                }
            }
        }
    }
}
