using NimbusMergerLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace NimbusMergerLibrary.Mergers
{
    public class DataTableMerger
    {
        protected HashSet<string> RowNames = new HashSet<string>();

        protected UAsset _gameAsset = null;
        protected string _dataTablePath = string.Empty;

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

        protected void RenameRow(StructPropertyData row)
        {
            if (RowNames.Contains(row.Name.ToString()))
            {
                int number = 1;
                row.Name.Value.Value = "Row";
                while (RowNames.Contains(row.Name.ToString()))
                {
                    //row.Name.Value = "Row";
                    row.Name.Number = number + 1;
                    number++;
                }
            }
            RowNames.Add(row.Name.ToString());
        }
    }
}
