using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Validations
{
	public class DateComparer : IComparer
	{
		public bool TryConvert(string text, bool cultureInvariant, out object value)
		{
			DateTime? date;
			if (cultureInvariant)
			{

				date = ConvertDate(text, "ymd");

			}
			else
			{
				// if the calendar is not gregorian, we should not enable client-side, so just parse it directly:
				if (!(DateTimeFormatInfo.CurrentInfo.Calendar.GetType() == typeof(GregorianCalendar)))
				{

					date = DateTime.Parse(text, CultureInfo.CurrentCulture);

				}
				else
				{

					var dateElementOrder = GetDateElementOrder();
					date = ConvertDate(text, dateElementOrder);

				}
			}

			if (date.HasValue)
			{

				value = date.Value;
				return true;

			}
			else
			{

				value = default;
				return false;

			}
		}

		private static DateTime? ConvertDate(string text, string dateElementOrder)
		{
			// always allow the YMD format, if they specify 4 digits
			var dateYearFirstExpression = "^\\s*((\\d{4})|(\\d{2}))([-/]|\\. ?)(\\d{1,2})\\4(\\d{1,2})\\.?\\s*$";
			var m = Regex.Match(text, dateYearFirstExpression);
			int day, month, year;
			if (m.Success && (m.Groups[2].Success || dateElementOrder == "ymd"))
			{

				day = int.Parse(m.Groups[6].Value, CultureInfo.InvariantCulture);
				month = int.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
				if (m.Groups[2].Success)
				{

					year = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);

				}
				else
				{

					year = GetFullYear(int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture));

				}
			}
			else
			{
				if (dateElementOrder == "ymd")
				{

					return null;

				}

				// also check for the year last format
				var dateYearLastExpression = "^\\s*(\\d{1,2})([-/]|\\. ?)(\\d{1,2})(?:\\s|\\2)((\\d{4})|(\\d{2}))(?:\\s\u0433\\.|\\.)?\\s*$";
				m = Regex.Match(text, dateYearLastExpression);
				if (!m.Success)
				{

					return null;

				}
				if (dateElementOrder == "mdy")
				{

					day = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
					month = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);

				}
				else
				{

					day = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
					month = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);

				}
				if (m.Groups[5].Success)
				{

					year = int.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);

				}
				else
				{

					year = GetFullYear(int.Parse(m.Groups[6].Value, CultureInfo.InvariantCulture));

				}
			}
			return new DateTime(year, month, day);
		}

		private static string GetDateElementOrder()
		{
			var info = DateTimeFormatInfo.CurrentInfo;
			var shortPattern = info.ShortDatePattern;
			if (shortPattern.IndexOf('y') < shortPattern.IndexOf('M'))
			{

				return "ymd";

			}
			else if (shortPattern.IndexOf('M') < shortPattern.IndexOf('d'))
			{

				return "mdy";

			}
			else
			{

				return "dmy";

			}
		}

		private static int GetFullYear(int shortYear)
		{

			return DateTimeFormatInfo.CurrentInfo.Calendar.ToFourDigitYear(shortYear);

		}


		public int CompareTo(object value, object valueToCompare)
		{

			return ((DateTime)value).CompareTo(valueToCompare);

		}
	}
}
