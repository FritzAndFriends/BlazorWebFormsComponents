using System;
using System.ComponentModel;
using System.Globalization;

namespace BlazorWebFormsComponents;

public sealed class UnitTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
		=> sourceType == typeof(string) || sourceType == typeof(int) || sourceType == typeof(double) || base.CanConvertFrom(context, sourceType);

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
	{
		if (value is null)
			return Unit.Empty;

		return value switch
		{
			string s when string.IsNullOrWhiteSpace(s) => Unit.Empty,
			string s => Unit.Parse(s, culture ?? CultureInfo.CurrentCulture),
			int i => new Unit(i),
			double d => new Unit(d),
			_ => base.ConvertFrom(context, culture, value)
		};
	}

	public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
		=> destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		if (destinationType == typeof(string) && value is Unit unit)
			return unit.ToString(culture ?? CultureInfo.CurrentCulture);

		return base.ConvertTo(context, culture, value, destinationType);
	}
}
