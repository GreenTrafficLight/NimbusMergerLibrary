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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NimbusMergerLibrary.Mergers
{
    public class AircraftViewerDataTableMerger : DataTableMerger
    {
        private HashSet<int> _gameAircraftViewerIDs = new HashSet<int>();
        private HashSet<int> _exportAircraftViewerIDs;

        private int _addAircraftViewerID = 1; // The mininum plane ID found the PlayerPlaneDataTable

        public AircraftViewerDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\AircraftViewerDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            _gameRowForCopy = gameRows[0];

            // Initialize the dictionary
            foreach (StructPropertyData row in gameRows)
            {
                IntPropertyData aircraftViewerID = (IntPropertyData)row["AircraftViewerID"];
                _gameAircraftViewerIDs.Add(aircraftViewerID.Value);
                if (_gameAircraftViewerIDs.Contains(_addAircraftViewerID)){
                    _addAircraftViewerID++;
                }

                RowNames.Add(row.Name.ToString());
            }

            _exportAircraftViewerIDs = new HashSet<int>(_gameAircraftViewerIDs);
        }

        public void AddRow(StructPropertyData modRow, List<StructPropertyData> gameDatas)
        {
            IntPropertyData aircraftViewerID = (IntPropertyData)modRow["AircraftViewerID"];

            // Update the added id value until it's not in the list
            while (_exportAircraftViewerIDs.Contains(_addAircraftViewerID))
            {
                _addAircraftViewerID++;
            }

            aircraftViewerID.Value = _addAircraftViewerID; // Update the PlaneID
            _exportAircraftViewerIDs.Add(_addAircraftViewerID); // Add the PlaneID to the list so it can't be re-used again

            StructPropertyData outputRow = PrepareModifiedRow(modRow);

            gameDatas.Add(outputRow); // Add the table
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

                IntPropertyData aircraftViewerID = (IntPropertyData)modData["AircraftViewerID"];

                // Add new row
                if (!_gameAircraftViewerIDs.Contains(aircraftViewerID.Value))
                {
                    AddRow(modData, gameDatas);
                }
            }
        }
    }
}
