using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace NimbusMergerLibrary.PropertyGridExtensions
{

    public class StructPropertyCollectionPropertyDescriptor : PropertyDescriptor
    {
        private StructPropertyCollection _collection = null;
        private int _index = -1;

        public StructPropertyCollectionPropertyDescriptor(StructPropertyCollection coll, int index, string name) : base(name, null)
        {
            _collection = coll;
            _index = index;
        }

        public override Type ComponentType
        {
            get
            {
                return _collection.GetType();
            }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _collection[_index].GetType(); }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object? GetValue(object? component)
        {
            PropertyData value = _collection[_index];
            switch (value.PropertyType.ToString())
            {
                case "BoolProperty":
                    BoolPropertyData boolPropertyData = value as BoolPropertyData;
                    return boolPropertyData.ToString();

                case "ByteProperty":
                    BytePropertyData bytePropertyData = value as BytePropertyData;
                    return bytePropertyData.ToString();

                case "EnumProperty":
                    EnumPropertyData enumPropertyData = value as EnumPropertyData;
                    return enumPropertyData.ToString();

                case "FloatProperty":
                    FloatPropertyData floatPropertyData = value as FloatPropertyData;
                    return floatPropertyData.ToString();

                case "IntProperty":
                    IntPropertyData intPropertyData = value as IntPropertyData;
                    return intPropertyData.ToString();

                case "StrProperty":
                    StrPropertyData strPropertyData = value as StrPropertyData;
                    return strPropertyData.ToString();

                case "StructProperty":
                    StructPropertyData structPropertyValue = value as StructPropertyData;

                    /*switch (structPropertyValue.StructType.ToString())
                    {
                        case "SoftObjectPath":
                            SoftObjectPathPropertyData softObjectPathPropertyValue = structPropertyValue.Value[0] as SoftObjectPathPropertyData;
                            test.Add(softObjectPathPropertyValue);
                            break;

                        default:
                            break;
                    }*/

                    break;

                case "SoftObjectPath":
                    SoftObjectPathPropertyData softObjectPathPropertyValue = value as SoftObjectPathPropertyData;
                    return softObjectPathPropertyValue.Value.AssetPath.AssetName.ToString();

                case "SoftObjectProperty":
                    SoftObjectPropertyData softObjectPropertyValue = value as SoftObjectPropertyData;
                    return softObjectPropertyValue.Value.AssetPath.AssetName.ToString();

                default:
                    break;
            }

            return "";
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override TypeConverter Converter
        {
            get
            {
                return new PropertyDataConverter(_collection[_index]);
            }
        }
    }
}
