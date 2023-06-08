using System.ComponentModel;

namespace IntermediateAPI.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        {
            var dictionary = obj.ToDictionary();
            dictionary.Add(name, value);
            return dictionary;
        }

        // helper
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                result.Add(property.Name, property.GetValue(obj));
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return result;
        }
    }
}
