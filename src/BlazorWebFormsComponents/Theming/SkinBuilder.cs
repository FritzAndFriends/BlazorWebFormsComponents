using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Fluent builder for configuring <see cref="ControlSkin"/> properties
	/// using strongly-typed lambda expressions.
	/// </summary>
	public class SkinBuilder
	{
		internal readonly ControlSkin Skin = new ControlSkin();

		/// <summary>
		/// Sets a property value on the skin using a strongly-typed expression.
		/// Supports direct properties (s => s.BackColor) and nested properties (s => s.Font.Bold).
		/// </summary>
		public SkinBuilder Set<TValue>(Expression<Func<ControlSkin, TValue>> property, TValue value)
		{
			if (property is null)
				throw new ArgumentNullException(nameof(property));

			SetValue(Skin, property.Body, value);
			return this;
		}

		private static void SetValue(object root, Expression expression, object value)
		{
			if (expression is not MemberExpression memberExpr)
				throw new ArgumentException("Expression must be a member access expression.");

			if (memberExpr.Expression is ParameterExpression)
			{
				// Direct property: s => s.BackColor
				SetProperty(root, memberExpr.Member, value);
			}
			else if (memberExpr.Expression is MemberExpression parentExpr)
			{
				// Nested property: s => s.Font.Bold
				var parent = GetOrCreateValue(root, parentExpr);
				SetProperty(parent, memberExpr.Member, value);
			}
			else
			{
				throw new ArgumentException("Unsupported expression structure.");
			}
		}

		private static object GetOrCreateValue(object root, MemberExpression expression)
		{
			object target;

			if (expression.Expression is ParameterExpression)
			{
				target = root;
			}
			else if (expression.Expression is MemberExpression parentExpr)
			{
				target = GetOrCreateValue(root, parentExpr);
			}
			else
			{
				throw new ArgumentException("Unsupported expression structure.");
			}

			var prop = (PropertyInfo)expression.Member;
			var current = prop.GetValue(target);
			if (current is null)
			{
				current = Activator.CreateInstance(prop.PropertyType);
				prop.SetValue(target, current);
			}
			return current;
		}

		private static void SetProperty(object target, MemberInfo member, object value)
		{
			if (member is PropertyInfo prop)
			{
				prop.SetValue(target, value);
			}
			else
			{
				throw new ArgumentException($"Member '{member.Name}' is not a property.");
			}
		}
	}
}
