using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace NimbusMergerLibrary.PropertyGridExtensions
{
    public class PropertyDataConverter : ExpandableObjectConverter
    {
        private readonly PropertyData _propertyData = null;

        public PropertyDataConverter(PropertyData propertyData) : base() {
            _propertyData = propertyData;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (_propertyData.PropertyType.ToString() == "StructProperty")
            {
                PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

                StructPropertyData structPropertyValue = _propertyData as StructPropertyData;
                switch (structPropertyValue.StructType.ToString())
                {
                    case "SoftObjectPath":
                        StructPropertyCollection structPropertyCollection = [structPropertyValue.Value[0]];
                        StructPropertyCollectionPropertyDescriptor test = new StructPropertyCollectionPropertyDescriptor(structPropertyCollection, 0, "[0]");
                        pds.Add(test);
                        return pds;

                    default:
                        break;
                }
            }

            return null;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (_propertyData.PropertyType.ToString() == "StructProperty") return true;

            return false;
        }
    }
}
