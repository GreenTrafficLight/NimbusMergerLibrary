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
        private HashSet<int> _gameWeaponIDs = new HashSet<int>();
        /// <summary>The plane IDs belonging to the asset that is going to be created.</summary>
        private HashSet<int> _exportWeaponIDs;

        private int _addWeaponID = 0;


        public PlayerWeaponDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\PlayerWeaponDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            _gameRowForCopy = gameRows[0];

            // Initialize the dictionary
            foreach (StructPropertyData row in gameRows)
            {
                // Add the game plane IDs
                IntPropertyData planeId = (IntPropertyData)row["WeaponID"];
                _gameWeaponIDs.Add(planeId.Value);
                // Increment the id for the new plane, so it's doesn't take one that exist
                if (_gameWeaponIDs.Contains(_addWeaponID))
                {
                    _addWeaponID++;
                }

                RowNames.Add(row.Name.ToString());
            }

            _exportWeaponIDs = new HashSet<int>(_gameWeaponIDs);

            
        }

        public void Merge(UAsset modAsset)
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            DataTableExport modDataTable = (DataTableExport)modAsset.Exports[0];
            UDataTable modTable = modDataTable.Table;
            List<StructPropertyData> modRows = modTable.Data;

            for (int i = 0; i < modRows.Count; i++)
            {
                StructPropertyData modRow = modRows[i];

                IntPropertyData weaponID = (IntPropertyData)modRow["WeaponID"];

                if (!_gameWeaponIDs.Contains(weaponID.Value))
                {
                    // Update the added id value until it's one that doesn't exist in the exported asset
                    while (_exportWeaponIDs.Contains(_addWeaponID))
                    {
                        _addWeaponID++;
                    }

                    weaponID.Value = _addWeaponID;
                    _exportWeaponIDs.Add(weaponID.Value);

                    StructPropertyData outputRow = PrepareModifiedRow(modRow);

                    gameRows.Add(outputRow); // Add the table
                }
            }
        }
    }
}
