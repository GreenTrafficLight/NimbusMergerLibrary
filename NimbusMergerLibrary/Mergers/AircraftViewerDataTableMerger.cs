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
    public class AircraftViewerDataTableMerger : DataTableMerger
    {
        private List<int> _gameAircraftViewerIDs = new List<int>();
        private List<int> _exportAircraftViewerIDs;

        private int _addAircraftViewerID = 1; // The mininum plane ID found the PlayerPlaneDataTable

        public AircraftViewerDataTableMerger(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\AircraftViewerDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            // Initialize the dictionary
            foreach (StructPropertyData data in gameDatas)
            {
                IntPropertyData aircraftViewerID = (IntPropertyData)data["AircraftViewerID"];
                _gameAircraftViewerIDs.Add(aircraftViewerID.Value);
                if (_gameAircraftViewerIDs.Contains(_addAircraftViewerID)){
                    _addAircraftViewerID++;
                }
            }

            _exportAircraftViewerIDs = new List<int>(_gameAircraftViewerIDs);
        }

        public void AddRow(StructPropertyData modData, List<StructPropertyData> gameDatas)
        {
            IntPropertyData aircraftViewerID = (IntPropertyData)modData["AircraftViewerID"];

            // Update the added id value until it's not in the list
            while (_exportAircraftViewerIDs.Contains(_addAircraftViewerID))
            {
                _addAircraftViewerID++;
            }

            aircraftViewerID.Value = _addAircraftViewerID; // Update the PlaneID
            _exportAircraftViewerIDs.Add(_addAircraftViewerID); // Add the PlaneID to the list so it can't be re-used again

            modData.Name.Number = aircraftViewerID.Value + 1; // Change row name
            
            gameDatas.Add(modData); // Add the table
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
