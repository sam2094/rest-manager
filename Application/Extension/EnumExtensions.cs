using System.ComponentModel;

namespace Application.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value, params object[] args)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .FirstOrDefault() as DescriptionAttribute;

            if (attribute == null)
                return value.ToString();

            return string.Format(attribute.Description, args);
        }
    }
}