using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Structs;

namespace NimbusMergerLibrary.PropertyGridExtensions
{
    public class StructProperties
    {
        StructPropertyCollection properties = new StructPropertyCollection();

        public StructProperties(StructPropertyData structPropertyData) 
        {
            foreach (var propertyData in structPropertyData.Value)
            {
                properties.Add(propertyData);
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StructPropertyCollection Properties
        {
            get { return properties; }
        }
    }
}
