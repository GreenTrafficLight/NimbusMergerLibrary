using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.IO;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace NimbusMergerLibrary.Utils
{
    public static class DataTableUtils
    {
        public static void FixPropertyReference(BytePropertyData property, INameMap asset)
        {
            property.EnumType = new FName(asset, asset.SearchNameReference(property.EnumType.Value));
        }

        public static void FixPropertyReference(EnumPropertyData property, INameMap asset)
        {
            property.EnumType = new FName(asset, asset.SearchNameReference(property.EnumType.Value));
            property.Value = new FName(asset, asset.SearchNameReference(property.Value.Value));
        }

        public static void FixPropertyReference(StructPropertyData property, INameMap asset)
        {
            property.StructType = new FName(asset, asset.SearchNameReference(property.StructType.Value));
        }

        public static void CopyRow(INameMap asset, StructPropertyData copiedRow, StructPropertyData outputRow)
        {
            foreach (PropertyData column in copiedRow.Value)
            {
                string columnName = column.Name.ToString();

                outputRow[columnName] = copiedRow[columnName];
                switch (column.PropertyType.ToString())
                {
                    case "ByteProperty":
                        FixPropertyReference((BytePropertyData)outputRow[columnName], asset);
                        break;

                    case "EnumProperty":
                        FixPropertyReference((EnumPropertyData)outputRow[columnName], asset);
                        break;

                    case "StructProperty":
                        FixPropertyReference((StructPropertyData)outputRow[columnName], asset);
                        break;

                    default:
                        break;
                }
            }
        }

        public static void CopyRowAccurate(INameMap asset, StructPropertyData copiedRow, StructPropertyData outputRow)
        {
            if (copiedRow.Value.Count != outputRow.Value.Count) throw new Exception("Missing columns");

            for (int i = 0; i < copiedRow.Value.Count; i++)
            {
                PropertyData column = copiedRow.Value[i];
                outputRow.Value[i] = column;
                switch (column.PropertyType.ToString())
                {
                    case "ByteProperty":
                        FixPropertyReference((BytePropertyData)outputRow.Value[i], asset);
                        break;

                    case "EnumProperty":
                        FixPropertyReference((EnumPropertyData)outputRow.Value[i], asset);
                        break;

                    case "StructProperty":
                        FixPropertyReference((StructPropertyData)outputRow.Value[i], asset);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
