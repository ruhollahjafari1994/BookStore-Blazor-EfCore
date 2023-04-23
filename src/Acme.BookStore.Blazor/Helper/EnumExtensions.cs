using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Reflection;

namespace Acme.BookStore.Blazor.Helper
{
    public static class EnumExtensions
    {
        public static DisplayAttributeValues GetDisplayAttributeValues(this Enum enumValue)
        {

            if (enumValue == null)
                return default;

            var displayAttribute = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();

            return new DisplayAttributeValues(displayAttribute);
        }

        public sealed class DisplayAttributeValues
        {
            private readonly DisplayAttribute _displayAttribute;

            public DisplayAttributeValues(DisplayAttribute displayAttribute)
            {
                _displayAttribute = displayAttribute;
            }

            public bool? AutoGenerateField => _displayAttribute?.GetAutoGenerateField();

            public bool? AutoGenerateFilter => _displayAttribute?.GetAutoGenerateFilter();

            public int? Order => _displayAttribute?.GetOrder();

            public string Description => _displayAttribute?.GetDescription() ?? string.Empty;

            public string GroupName => _displayAttribute?.GetGroupName() ?? string.Empty;

            public string Name => _displayAttribute?.GetName() ?? string.Empty;

            public string Prompt => _displayAttribute?.GetPrompt() ?? string.Empty;

            public string ShortName => _displayAttribute?.GetShortName() ?? string.Empty;

        }
    }
}
