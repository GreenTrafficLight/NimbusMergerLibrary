using Ace7LocalizationFormat.Formats;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Assets.Objects.Properties;
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
    public class PlayerPlaneDataTableMerger : DataTableMerger
    {
        /// <summary>The plane IDs belonging to the game asset. Used to see if there is a new plane added</summary>
        private List<int> _gamePlaneIDs = new List<int>();
        /// <summary>The plane IDs belonging to the asset that is going to be created.</summary>
        private List<int> _exportPlaneIDs;
        
        private List<int> _exportSortNumbers = new List<int>();
        private List<string> _planeStrings = new List<string>();
        private List<string> _planeReferences = new List<string>();

        private Dictionary<int, string> _planeStringsDict = new Dictionary<int, string>();

        /// <summary>
        /// Keep track of plane IDs that has been updated
        /// And store their old IDs so they can be referenced when it's used in the OriginalPlaneID
        /// </summary>
        private Dictionary<int, int> _updatedIds = new Dictionary<int, int>();

        /// <summary>The mininum plane ID found the PlayerPlaneDataTable.</summary>
        private int _addPlaneId = 100;
        /// <summary>The mininum sort Number found the PlayerPlaneDataTable.</summary>
        private int _addSortNumber = 1;

        private StructPropertyData _rowForCopy = null;
        

        public PlayerPlaneDataTableMerger(UAsset gameAsset, CmnFile cmn, DatFile dat)
        {
            _gameAsset = gameAsset;
            _dataTablePath = "\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            _rowForCopy = gameDatas[0];

            // Initialize the dictionary
            foreach (StructPropertyData data in gameDatas)
            {
                // Add the game plane IDs
                IntPropertyData planeId = (IntPropertyData)data["PlaneID"];
                _gamePlaneIDs.Add(planeId.Value);
                // Increment the id for the new plane, so it's doesn't take one that exist
                if (_gamePlaneIDs.Contains(_addPlaneId)){
                    _addPlaneId++;
                }

                // Add the game sort numbers
                IntPropertyData sortNumber = (IntPropertyData)data["SortNumber"];
                _exportSortNumbers.Add(sortNumber.Value);

                // Add the game plane string IDs
                StrPropertyData planeStringID = (StrPropertyData)data["PlaneStringID"];
                string planeString = GetPlaneString(planeStringID, cmn, dat);
                _planeStringsDict.Add(planeId.Value, planeString);
            }

            _exportSortNumbers.Add(41);
            _exportPlaneIDs = new List<int>(_gamePlaneIDs);
        }


        private void CopyRow(StructPropertyData copiedRow, StructPropertyData outputRow)
        {
            foreach (PropertyData column in copiedRow.Value)
            {
                string columnName = column.Name.ToString();

                outputRow[columnName] = copiedRow[columnName];
                switch (column.PropertyType.ToString())
                {
                    case "ByteProperty":
                        DataTableUtils.FixPropertyReference((BytePropertyData)outputRow[columnName], _gameAsset);
                        break;

                    case "EnumProperty":
                        DataTableUtils.FixPropertyReference((EnumPropertyData)outputRow[columnName], _gameAsset);
                        break;

                    case "StructProperty":
                        DataTableUtils.FixPropertyReference((StructPropertyData)outputRow[columnName], _gameAsset);
                        break;

                    default:
                        break;
                }
            }
        }

        private void Sort()
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameDatas = gameTable.Data;

            for (int i = 0; i < gameDatas.Count; i++)
            {
                StructPropertyData data = gameDatas[i];

                IntPropertyData planeId = (IntPropertyData)data["PlaneID"];

                string planeString = _planeStringsDict[planeId.Value];

                IntPropertyData alphabeticalSortNumber = (IntPropertyData)data["AlphabeticalSortNumber"];
                alphabeticalSortNumber.Value = _planeStrings.IndexOf(planeString) + 1; // It's start at 1 and not 0;
            }
        }

        private void AddRow(NimbusFileProvider fileProvider, string planeString, StructPropertyData modData, List<StructPropertyData> gameDatas)
        {
            IntPropertyData planeId = (IntPropertyData)modData["PlaneID"];
            SoftObjectPropertyData reference = (SoftObjectPropertyData)modData["Reference"];
            IntPropertyData sortNumber = (IntPropertyData)modData["SortNumber"];

            // Check if the asset exist in the paks
            // Used for mods that have additional plane rows but doesn't have their pawn
            if (!fileProvider.CheckAssetReference(reference))
            {
                _planeStrings.RemoveAt(_planeStrings.Count - 1);
                _planeReferences.RemoveAt(_planeReferences.Count - 1);
                return;
            }

            // Update the sort number value until it's one that doesn't exist in the exported asset
            while (_exportSortNumbers.Contains(_addSortNumber))
            {
                _addSortNumber++;
            }

            // Update the added id value until it's one that doesn't exist in the exported asset
            while (_exportPlaneIDs.Contains(_addPlaneId))
            {
                _addPlaneId++;
            }

            // Update the OriginalPlaneID
            _updatedIds.Add(planeId.Value, _addPlaneId); // Add the old planeId to the dict as key, and put the new id value
            IntPropertyData originalPlaneId = (IntPropertyData)modData["OriginalPlaneID"];
            if (originalPlaneId.Value != -1)
            {
                originalPlaneId.Value = _updatedIds[originalPlaneId.Value];
            }

            planeId.Value = _addPlaneId; // Update the PlaneID
            _exportPlaneIDs.Add(_addPlaneId); // Add the PlaneID to the list so it can't be re-used again
            sortNumber.Value = _addSortNumber;
            _exportSortNumbers.Add(sortNumber.Value);

            // Add the new asseth path name to the name map
            string assetPathName = reference.Value.AssetPath.AssetName.ToString();
            // Fix the reference to point to the asseth path name
            reference.Value = new FSoftObjectPath(new FTopLevelAssetPath(null, new FName(_gameAsset, assetPathName)), null); 
            
            // Copy the first row of the game asset
            StructPropertyData outputRow = (StructPropertyData)_rowForCopy.Clone();
            CopyRow(modData, outputRow);
            outputRow.Name.Number = planeId.Value + 1; // Change row name
            
            gameDatas.Add(outputRow); // Add the table

            _planeStringsDict.Add(planeId.Value, planeString);
        }

        public void Merge(NimbusFileProvider fileProvider, UAsset modAsset, CmnFile cmn = null, DatFile dat = null)
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

                // Get the reference to the AcePlayerPawn of the plane
                // Used to see if there any duplicates, if there is, continue the loop
                SoftObjectPropertyData reference = (SoftObjectPropertyData)modData["Reference"];
                string assetPath = Path.GetFileNameWithoutExtension(reference.Value.AssetPath.AssetName.ToString());
                if (_planeReferences.Contains(assetPath)){
                    continue;
                }
                _planeReferences.Add(assetPath);

                StrPropertyData planeStringID = (StrPropertyData)modData["PlaneStringID"];
                string planeString = GetPlaneString(planeStringID, cmn, dat);

                IntPropertyData planeId = (IntPropertyData)modData["PlaneID"];
                IntPropertyData sortNumber = (IntPropertyData)modData["SortNumber"];

                // Check if it's a id that doesn't exist in the original game asset
                if (!_gamePlaneIDs.Contains(planeId.Value))
                {
                    // If it isn't, add the new row
                    AddRow(fileProvider, planeString, modData, gameDatas);
                }
                else
                {
                    
                }
                // Modify existing row
                /*else if (_originalPlaneIDs.Contains(planeId.Value))
                {
                    gameDatas[i] = modData;
                }*/

                
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
