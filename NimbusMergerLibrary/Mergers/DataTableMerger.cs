using NimbusMergerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace NimbusMergerLibrary.Mergers
{
    public class DataTableMerger
    {
        protected HashSet<string> RowNames = new HashSet<string>();

        protected UAsset _gameAsset = null;
        protected string _dataTablePath = string.Empty;
        protected StructPropertyData _gameRowForCopy = null;

        public void Write(string path)
        {
            string exportFilePath = path + _dataTablePath;
            string directoryPath = Path.GetDirectoryName(exportFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            _gameAsset.Write(exportFilePath);
        }

        protected void CopyRow(StructPropertyData copiedRow, StructPropertyData outputRow)
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

        protected void RenameRow(StructPropertyData row, int number = 1)
        {
            if (RowNames.Contains(row.Name.ToString()))
            {
                int saveRowNumber = row.Name.Number;
                row.Name = new FName(_gameAsset, _gameAsset.SearchNameReference(new FString("Row")));
                row.Name.Number = saveRowNumber;
                while (RowNames.Contains(row.Name.ToString()))
                {
                    //row.Name.Value = "Row";
                    row.Name.Number = number + 1;
                    number++;
                }
            }
            RowNames.Add(row.Name.ToString());
        }

        protected StructPropertyData PrepareAddingRow(StructPropertyData modRow, int number = 1) 
        {
            if (!RowNames.Any()) throw new Exception("Row names is empty");

            if (_gameRowForCopy == null) throw new Exception("No assigned game row for copy");

            // Copy the first row of the game asset
            StructPropertyData outputRow = (StructPropertyData)_gameRowForCopy.Clone();
            // Write the mod row to the copied row
            CopyRow(modRow, outputRow);
            // Change row name
            RenameRow(outputRow, number);

            return outputRow;
        }
    }
}
