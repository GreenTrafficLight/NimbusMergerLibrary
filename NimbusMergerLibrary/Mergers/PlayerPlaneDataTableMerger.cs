using Ace7LocalizationFormat.Formats;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Assets.Objects.Properties;
using NimbusMergerLibrary.FileProvider;
using NimbusMergerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace NimbusMergerLibrary.Mergers
{
    public class ReferenceDictionary
    {
        public ReferenceDictionary(int planeID, int sortNumber, int rowIndex)
        {
            PlaneID = planeID;
            SortNumber = sortNumber;
            RowIndex = rowIndex;
        }

        public int PlaneID { get; set; }
        public int SortNumber { get; set; }
        public int RowIndex { get; set; }
    }

    public class PlayerPlaneDataTableMerger : DataTableMerger
    {
        /// <summary>The plane IDs belonging to the game asset. Used to see if there is a new plane added</summary>
        private HashSet<int> _gamePlaneIDs = new HashSet<int>();
        /// <summary>The plane IDs belonging to the asset that is going to be created.</summary>
        private HashSet<int> _exportPlaneIDs;
        
        private HashSet<int> _exportSortNumbers = new HashSet<int>();
        private List<string> _planeStrings = new List<string>();

        private HashSet<string> _gamePlaneReferences = new HashSet<string>();
        /// <summary>
        /// Dictionary to keep track of the row index for the plane references.
        /// The key is the plane reference path.
        /// The value is the row index
        /// </summary>
        private Dictionary<string, ReferenceDictionary> _planeReferences = new Dictionary<string, ReferenceDictionary>();

        /// <summary>
        /// Dictionary to keep track of which plane short name the plane ID has
        /// The key is the plane ID
        /// The value is the string of the plane short name
        /// </summary>
        private Dictionary<int, string> _planeStringsDict = new Dictionary<int, string>();

        /// <summary>
        /// Dictionary to keep track of plane IDs that has been updated
        /// And store their old IDs so they can be referenced when it's used in the OriginalPlaneID
        /// The key is the old plane ID, the value is the new plane ID
        /// </summary>
        private Dictionary<int, int> _updatedIds = new Dictionary<int, int>();

        /// <summary>The mininum plane ID found the PlayerPlaneDataTable.</summary>
        private int _addPlaneId = 100;
        /// <summary>The mininum sort Number found the PlayerPlaneDataTable.</summary>
        private int _addSortNumber = 1;

        private StructPropertyData _gameRowForCopy = null;
        

        public PlayerPlaneDataTableMerger(UAsset gameAsset, CmnFile cmn, DatFile dat)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            _gameRowForCopy = gameRows[0];

            for (int i = 0; i < gameRows.Count; i++)
            {
                StructPropertyData data = gameRows[i];

                // Add the game plane IDs
                IntPropertyData planeId = (IntPropertyData)data["PlaneID"];
                _gamePlaneIDs.Add(planeId.Value);
                // Increment the id for the new plane, so it's doesn't take one that exist
                if (_gamePlaneIDs.Contains(_addPlaneId)) _addPlaneId++;

                // Get the reference to the AcePlayerPawn of the plane
                // Used to see if there any duplicates, if there is, continue the loop
                SoftObjectPropertyData reference = (SoftObjectPropertyData)data["Reference"];
                string assetPath = Path.GetFileNameWithoutExtension(reference.Value.AssetPath.AssetName.ToString());
                _gamePlaneReferences.Add(assetPath);

                // Add the game sort numbers
                IntPropertyData sortNumber = (IntPropertyData)data["SortNumber"];
                _exportSortNumbers.Add(sortNumber.Value);

                // Add the game plane string IDs
                StrPropertyData planeStringID = (StrPropertyData)data["PlaneStringID"];
                string planeString = GetPlaneString(planeStringID, cmn, dat);
                _planeStringsDict.Add(planeId.Value, planeString);

                _planeReferences.Add(assetPath, new ReferenceDictionary(planeId.Value, sortNumber.Value, i));
            }

            _exportSortNumbers.Add(41);
            _exportPlaneIDs = new HashSet<int>(_gamePlaneIDs);
        }
        private void Sort()
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameColumns = gameTable.Data;

            for (int rowIndex = 0; rowIndex < gameColumns.Count; rowIndex++)
            {
                StructPropertyData data = gameColumns[rowIndex];

                IntPropertyData planeId = (IntPropertyData)data["PlaneID"];

                string planeString = _planeStringsDict[planeId.Value];

                IntPropertyData alphabeticalSortNumber = (IntPropertyData)data["AlphabeticalSortNumber"];
                alphabeticalSortNumber.Value = _planeStrings.IndexOf(planeString) + 1; // It's start at 1 and not 0;
            }
        }

        private void AddRow(NimbusFileProvider fileProvider, string planeString, StructPropertyData modRow, List<StructPropertyData> gameRows)
        {
            IntPropertyData planeId = (IntPropertyData)modRow["PlaneID"];
            SoftObjectPropertyData reference = (SoftObjectPropertyData)modRow["Reference"];
            IntPropertyData sortNumber = (IntPropertyData)modRow["SortNumber"];

            // Check if the asset exist in the paks
            // Used for mods that have additional plane rows but doesn't have their pawn
            if (!fileProvider.CheckAssetReference(reference))
            {
                _planeStrings.RemoveAt(_planeStrings.Count - 1);
                return;
            }

            // Update the sort number value until it's one that doesn't exist in the exported asset
            while (_exportSortNumbers.Contains(_addSortNumber)) _addSortNumber++;

            // Update the added id value until it's one that doesn't exist in the exported asset
            while (_exportPlaneIDs.Contains(_addPlaneId)) _addPlaneId++;

            // Update the OriginalPlaneID
            _updatedIds.Add(planeId.Value, _addPlaneId); // Add the old planeId to the dict as key, and put the new id value
            IntPropertyData originalPlaneId = (IntPropertyData)modRow["OriginalPlaneID"];
            if (originalPlaneId.Value != -1) originalPlaneId.Value = _updatedIds[originalPlaneId.Value];

            planeId.Value = _addPlaneId; // Update the PlaneID
            _exportPlaneIDs.Add(_addPlaneId); // Add the PlaneID so it can't be re-used again
            sortNumber.Value = _addSortNumber; // Update the SortNumber
            _exportSortNumbers.Add(sortNumber.Value); // Add the SortNumber so it can't be re-used again 

            // Add the new asseth path name to the name map
            string assetPathName = reference.Value.AssetPath.AssetName.ToString();
            // Fix the reference to point to the asseth path name
            reference.Value = new FSoftObjectPath(new FTopLevelAssetPath(null, new FName(_gameAsset, assetPathName)), null); 
            
            // Copy the first row of the game asset
            StructPropertyData outputRow = (StructPropertyData)_gameRowForCopy.Clone();
            // Write the mod row to the copied row
            CopyRow(modRow, outputRow);
            // Change row name
            outputRow.Name.Number = planeId.Value + 1; 

            // Add that the plane has been added to the table
            string assetPath = Path.GetFileNameWithoutExtension(reference.Value.AssetPath.AssetName.ToString());
            _planeReferences.Add(assetPath, new ReferenceDictionary(planeId.Value, sortNumber.Value, gameRows.Count()));

            gameRows.Add(outputRow); // Add the table to the export one

            _planeStringsDict.Add(planeId.Value, planeString);
        }

        public void Merge(NimbusFileProvider fileProvider, UAsset modAsset, CmnFile cmn = null, DatFile dat = null)
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            DataTableExport modDataTable = (DataTableExport)modAsset.Exports[0];
            UDataTable modTable = modDataTable.Table;
            List<StructPropertyData> modRows = modTable.Data;

            for (int rowIndex = 0; rowIndex < modRows.Count; rowIndex++) 
            {
                StructPropertyData modRow = modRows[rowIndex];

                // Get the reference to the AcePlayerPawn of the plane
                // Used to see if there any duplicates, if there is, continue the loop
                SoftObjectPropertyData reference = (SoftObjectPropertyData)modRow["Reference"];
                string assetPath = Path.GetFileNameWithoutExtension(reference.Value.AssetPath.AssetName.ToString());
                
                // Get the short name of the plane within the localization
                StrPropertyData planeStringID = (StrPropertyData)modRow["PlaneStringID"];
                string planeString = GetPlaneString(planeStringID, cmn, dat);

                // Get the columns
                IntPropertyData planeId = (IntPropertyData)modRow["PlaneID"];
                IntPropertyData sortNumber = (IntPropertyData)modRow["SortNumber"];

                // Check if it's a reference that doesn't exist in the original game asset
                if (!_planeReferences.ContainsKey(assetPath))
                {
                    // If it isn't, add the new row
                    AddRow(fileProvider, planeString, modRow, gameRows);
                }
                // Modify existing row
                else
                {
                    // If the ID of a game existing plane is different
                    /*if (!_gamePlaneIDs.Contains(planeId.Value) && _gamePlaneReferences.Contains(assetPath))
                    {
                        // Update the added id value until it's one that doesn't exist in the exported asset
                        while (_exportPlaneIDs.Contains(_addPlaneId)) _addPlaneId++;

                        // Update the sort number value until it's one that doesn't exist in the exported asset
                        while (_exportSortNumbers.Contains(_addSortNumber)) _addSortNumber++;

                        _planeReferences[assetPath].PlaneID = _addPlaneId;
                        _planeReferences[assetPath].SortNumber = _addSortNumber;
                        planeId.Value = _addPlaneId;
                        sortNumber.Value = _addSortNumber;
                        _exportPlaneIDs.Add(planeId.Value);
                        _exportSortNumbers.Add(sortNumber.Value);
                        _planeStringsDict.Add(planeId.Value, planeString);
                    }

                    StructPropertyData outputRow = (StructPropertyData)_gameRowForCopy.Clone();
                    CopyRow(modRow, outputRow);
                    IntPropertyData outputPlaneId = (IntPropertyData)outputRow["PlaneID"];
                    outputPlaneId.Value = _planeReferences[assetPath].PlaneID;
                    IntPropertyData outputSortNumber = (IntPropertyData)outputRow["SortNumber"];
                    outputSortNumber.Value = _planeReferences[assetPath].SortNumber;
                    gameRows[_planeReferences[assetPath].RowIndex] = outputRow;*/
                }                
            }

            _updatedIds.Clear();

            // Sort the plane string by alphabetical order
            _planeStrings = _planeStrings.OrderBy(s => char.IsDigit(s[0]) ? 1 : 0)
                                        .ThenBy(s => s, StringComparer.Ordinal)
                                        .ToList();

            Sort();
        }
    
        private string GetPlaneString(StrPropertyData planeStringID, CmnFile cmn, DatFile dat)
        {
            string planeString = planeStringID.ToString().ToUpper();
            // Get the localization string of the plane
            // Used for the sorting of planes later
            if (cmn != null && dat != null)
            {
                var stringNumber = cmn[$"AircraftShort_Name_{planeStringID}"]; // Get the plane string
                planeString = dat.Strings[stringNumber].Trim('\0'); // Remove the "\0" at the end of the string
                if (!_planeStrings.Contains(planeString))
                {
                    _planeStrings.Add(planeString);
                }
            }
            else
            {
                _planeStrings.Add(planeString);
            }
            return planeString;
        }
    }
}
