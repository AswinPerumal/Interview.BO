using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BidOne.Gateway.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            // Get the field info for the enum value
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            // Get the DescriptionAttribute if it exists
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                            as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}
