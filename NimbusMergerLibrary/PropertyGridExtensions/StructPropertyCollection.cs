using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;

namespace NimbusMergerLibrary.PropertyGridExtensions
{
    public class StructPropertyCollection : CollectionBase, ICustomTypeDescriptor
    {
        public void Add(PropertyData column)
        {
            List.Add(column);
        }

        public PropertyData this[int index]
        {
            get
            {
                return (PropertyData)List[index];
            }
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string? GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string? GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter? GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor? GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor? GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object? GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[]? attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
        {
            // Create a new collection object PropertyDescriptorCollection
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < List.Count; i++)
            {
                StructPropertyCollectionPropertyDescriptor pd = new
                              StructPropertyCollectionPropertyDescriptor(this, i, this[i].Name.ToString());

                pds.Add(pd);
            }
            return pds;
        }

        public object? GetPropertyOwner(PropertyDescriptor? pd)
        {
            return this;
        }
    }
}
