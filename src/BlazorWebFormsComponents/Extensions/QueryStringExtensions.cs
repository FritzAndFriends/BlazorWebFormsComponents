using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

public static class QueryStringExtensions
{
	public static string? Get(this IQueryCollection? queryString, string key)
	{
		if (queryString is null || string.IsNullOrEmpty(key))
			return null;

		return queryString.TryGetValue(key, out var value) ? value.ToString() : null;
	}
}
