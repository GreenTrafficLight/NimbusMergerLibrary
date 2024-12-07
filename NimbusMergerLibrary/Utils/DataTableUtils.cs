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
            property.Value = new FName(asset, asset.SearchNameReference(property.Value.Value));
        }

        public static void FixPropertyReference(StructPropertyData property, INameMap asset)
        {
            property.StructType = new FName(asset, asset.SearchNameReference(property.StructType.Value));
        }
    }
}
