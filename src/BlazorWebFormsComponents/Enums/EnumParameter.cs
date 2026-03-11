using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A wrapper struct that allows Blazor component enum parameters to accept both
	/// enum values and string values. This enables migrated Web Forms markup like
	/// GridLines="None" to work without requiring Razor expression syntax @(GridLines.None).
	/// </summary>
	/// <typeparam name="T">The enum type to wrap.</typeparam>
	public readonly struct EnumParameter<T> : IEquatable<EnumParameter<T>>, IEquatable<T>
		where T : struct, Enum
	{
		public T Value { get; }

		public EnumParameter(T value) => Value = value;

		// Accept enum values directly: GridLines="@(GridLines.None)"
		public static implicit operator EnumParameter<T>(T value) => new(value);

		// Accept string values: GridLines="None"
		public static implicit operator EnumParameter<T>(string value)
			=> new(Enum.Parse<T>(value, ignoreCase: true));

		// Convert back to enum transparently
		public static implicit operator T(EnumParameter<T> param) => param.Value;

		public override string ToString() => Value.ToString();

		public override bool Equals(object obj) => obj switch
		{
			EnumParameter<T> other => Value.Equals(other.Value),
			T enumVal => Value.Equals(enumVal),
			_ => false
		};

		public bool Equals(EnumParameter<T> other) => Value.Equals(other.Value);

		public bool Equals(T other) => Value.Equals(other);

		public override int GetHashCode() => Value.GetHashCode();

		public static bool operator ==(EnumParameter<T> left, EnumParameter<T> right)
			=> left.Value.Equals(right.Value);

		public static bool operator !=(EnumParameter<T> left, EnumParameter<T> right)
			=> !left.Value.Equals(right.Value);

		public static bool operator ==(EnumParameter<T> left, T right)
			=> left.Value.Equals(right);

		public static bool operator !=(EnumParameter<T> left, T right)
			=> !left.Value.Equals(right);

		public static bool operator ==(T left, EnumParameter<T> right)
			=> left.Equals(right.Value);

		public static bool operator !=(T left, EnumParameter<T> right)
			=> !left.Equals(right.Value);
	}
}
