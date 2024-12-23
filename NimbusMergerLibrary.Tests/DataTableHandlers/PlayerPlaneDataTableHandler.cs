using Ace7LocalizationFormat.Formats;
using NimbusMergerLibrary.Mergers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using UAssetAPI.UnrealTypes;

namespace NimbusMergerLibrary.Tests.DataTableReader
{
    public class PlayerPlaneDataTableHandler
    {
        /// <summary>The plane IDs belonging to the game asset. Used to see if there is a new plane added</summary>
        private Dictionary<string, int> _gamePlaneStrings = new Dictionary<string, int>();
       
        private HashSet<int> _planeIds = new HashSet<int>();
        private int _addPlaneID = 100;

        private UAsset _gameAsset = null;
        private string _bpPath;
        

        public PlayerPlaneDataTableHandler(UAsset gameAsset)
        {
            _gameAsset = gameAsset;
            _bpPath = "Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable";

            DataTableExport dataTable = (DataTableExport)gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            for (int i = 0; i < gameRows.Count; i++)
            {
                StructPropertyData row = gameRows[i];

                IntPropertyData planeId = (IntPropertyData)row["PlaneID"];
                _planeIds.Add(planeId.Value);
                while (_planeIds.Contains(_addPlaneID)) _addPlaneID++;
                // Add the game plane string IDs
                StrPropertyData planeStringID = (StrPropertyData)row["PlaneStringID"];
                _gamePlaneStrings.Add(planeStringID.ToString(), planeId.Value);
            }
        }

        public void Extract(UAsset modAsset, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
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

                    // Get the short name of the plane within the localization
                    StrPropertyData planeStringID = (StrPropertyData)modRow["PlaneStringID"];

                    // Check if it's a reference that doesn't exist in the original game asset
                    if (!_gamePlaneStrings.Keys.Contains(planeStringID.ToString()))
                    {
                        Console.WriteLine($"add_row {_bpPath} {planeStringID.Value}");
                        sw.WriteLine($"add_row {_bpPath} {planeStringID.Value}");
                        for (int columnIndex = 0; columnIndex < modRow.Value.Count; columnIndex++)
                        {
                            PropertyData value = modRow.Value[columnIndex];
                            string line = $"add_column {_bpPath} {planeStringID.Value} {value.Name} {value.PropertyType} ";
                            switch (modRow.Value[columnIndex].PropertyType.ToString())
                            {
                                case "BoolProperty":
                                    line += ((BoolPropertyData)value).Value;
                                    break;

                                case "ByteProperty":
                                    BytePropertyData bytePropertyValue = ((BytePropertyData)value);
                                    line += bytePropertyValue.EnumType + " ";
                                    line += bytePropertyValue.Value;
                                    break;

                                case "EnumProperty":
                                    EnumPropertyData enumPropertyValue = ((EnumPropertyData)value);
                                    line += enumPropertyValue.EnumType + " ";
                                    line += enumPropertyValue.Value;
                                    break;

                                case "FloatProperty":
                                    line += ((FloatPropertyData)value).Value;
                                    break;

                                case "IntProperty":
                                    line += ((IntPropertyData)value).Value;
                                    break;

                                case "StrProperty":
                                    line += ((StrPropertyData)value).Value;
                                    break;

                                case "StructProperty":
                                    // Put StructType comparison
                                    StructPropertyData structPropertyValue = ((StructPropertyData)value);
                                    line += structPropertyValue.StructType.ToString() + " ";
                                    switch (structPropertyValue.StructType.ToString())
                                    {
                                        case "SoftObjectPath":
                                            SoftObjectPathPropertyData softObjectPathPropertyValue = (SoftObjectPathPropertyData)structPropertyValue.Value[0];
                                            line += softObjectPathPropertyValue.Value.AssetPath.AssetName;
                                            break;

                                        default:
                                            break;
                                    }

                                    
                                    break;

                                case "SoftObjectProperty":
                                    line += ((SoftObjectPropertyData)value).Value.AssetPath.AssetName;
                                    break;

                                default:
                                    line += value.PropertyType.ToString();
                                    break;
                            }
                            sw.WriteLine(line);
                            Console.WriteLine(line);

                        }
                    
                    }
                    else
                    {
                    }
                }

                sw.Close();
            }

        }
    
        public void Test(string path)
        {
            DataTableExport dataTable = (DataTableExport)_gameAsset.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            StructPropertyData newRow = new StructPropertyData();
            newRow.Value = new List<PropertyData>();

            var lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];

                string[] splits = line.Split();

                switch (splits[4])
                {
                    case "ByteProperty":
                        BytePropertyData newByteProperty = new BytePropertyData(new FName(_gameAsset, splits[3]));
                        newByteProperty.EnumType = new FName(_gameAsset, splits[5]);
                        byte.TryParse(splits[6], out byte byteResult);
                        newByteProperty.Value = byteResult;
                        newRow.Value.Add(newByteProperty);
                        break;

                    case "BoolProperty":
                        BoolPropertyData newBoolProperty = new BoolPropertyData(new FName(_gameAsset, splits[3]));
                        bool.TryParse(splits[5], out bool boolResult);
                        newBoolProperty.Value = boolResult;
                        newRow.Value.Add(newBoolProperty);
                        break;

                    case "EnumProperty":
                        EnumPropertyData newEnumProperty = new EnumPropertyData(new FName(_gameAsset, splits[3]));
                        newEnumProperty.EnumType = new FName(_gameAsset, splits[5]);
                        newEnumProperty.Value = new FName(_gameAsset, splits[6]);
                        newRow.Value.Add(newEnumProperty);
                        break;

                    case "FloatProperty":
                        FloatPropertyData newFloatProperty = new FloatPropertyData(new FName(_gameAsset, splits[3]));
                        float.TryParse(splits[5], out float floatResult);
                        newFloatProperty.Value = floatResult;
                        newRow.Value.Add(newFloatProperty);
                        break;

                    case "IntProperty":
                        IntPropertyData newIntProperty = new IntPropertyData(new FName(_gameAsset, splits[3]));
                        int.TryParse(splits[5], out int intResult);
                        newIntProperty.Value = intResult;
                        newRow.Value.Add(newIntProperty);
                        break;

                    case "StrProperty":
                        StrPropertyData newStrProperty = new StrPropertyData(new FName(_gameAsset, splits[3]));
                        newStrProperty.Value = new FString(splits[5]);
                        newRow.Value.Add(newStrProperty);
                        break;

                    case "StructProperty":
                        StructPropertyData newStructProperty = new StructPropertyData(new FName(_gameAsset, splits[3]));
                        newStructProperty.Value = new List<PropertyData>();
                        switch (splits[5])
                        {
                            case "SoftObjectPath":
                                SoftObjectPathPropertyData newSoftObjectPathPropertyData = new SoftObjectPathPropertyData(new FName(_gameAsset, splits[3]));
                                newSoftObjectPathPropertyData.Value = new FSoftObjectPath(new FTopLevelAssetPath(null, new FName(_gameAsset, splits[6])), null);
                                newStructProperty.Value.Add(newSoftObjectPathPropertyData);
                                break;

                            default:
                                break;
                        }
                        newRow.Value.Add(newStructProperty);
                        break;


                    case "SoftObjectProperty":
                        SoftObjectPropertyData newSoftObjectProperty = new SoftObjectPropertyData(new FName(_gameAsset, splits[3]));
                        newSoftObjectProperty.Value = new FSoftObjectPath(new FTopLevelAssetPath(null, new FName(_gameAsset, splits[5])), null);
                        newRow.Value.Add(newSoftObjectProperty);
                        break;

                    default:
                        break;
                } 
            }
            Console.WriteLine("test");
        }
    }
}
