using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.DataBinding;

/// <summary>
/// Resolves and invokes string-named methods on a hosting page, emulating the
/// ASP.NET Web Forms model-binding SelectMethod/InsertMethod/UpdateMethod/DeleteMethod pattern.
/// Web Forms resolves these method names via reflection at runtime against the page class;
/// this class replicates that behavior for Blazor components hosted on a <see cref="WebFormsPageBase"/>.
/// </summary>
internal static class SelectMethodResolver
{
	/// <summary>
	/// Resolves a named method on the target object and invokes it, returning the result
	/// as <see cref="IEnumerable{T}"/>. Supports methods returning IEnumerable&lt;T&gt;,
	/// IQueryable&lt;T&gt;, List&lt;T&gt;, T[], or non-generic IEnumerable/IQueryable.
	/// </summary>
	/// <typeparam name="T">The item type expected by the data-bound component.</typeparam>
	/// <param name="target">The object to search for the method (typically a WebFormsPageBase).</param>
	/// <param name="methodName">The name of the method to invoke.</param>
	/// <returns>The method result cast to IEnumerable&lt;T&gt;, or null if the method returns null.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the method cannot be found, returns an incompatible type, or invocation fails.
	/// </exception>
	public static IEnumerable<T> InvokeSelectMethod<T>(object target, string methodName)
	{
		var method = FindMethod(target, methodName);
		var result = InvokeMethod(target, method);
		return CastResult<T>(result, methodName, target.GetType());
	}

	/// <summary>
	/// Async version of <see cref="InvokeSelectMethod{T}"/>. Handles methods that return
	/// Task&lt;IEnumerable&lt;T&gt;&gt;, Task&lt;IQueryable&lt;T&gt;&gt;, Task&lt;List&lt;T&gt;&gt;, etc.
	/// Falls back to synchronous invocation if the method is not async.
	/// </summary>
	public static async Task<IEnumerable<T>> InvokeSelectMethodAsync<T>(object target, string methodName)
	{
		var method = FindMethod(target, methodName);
		var result = InvokeMethod(target, method);

		// If the method returns a Task, await it
		if (result is Task task)
		{
			await task.ConfigureAwait(false);

			// Extract the result from Task<T>
			var taskType = task.GetType();
			if (taskType.IsGenericType)
			{
				var resultProperty = taskType.GetProperty("Result");
				result = resultProperty?.GetValue(task);
			}
			else
			{
				// Task (no result) — method was void async
				return null;
			}
		}

		return CastResult<T>(result, methodName, target.GetType());
	}

	/// <summary>
	/// Invokes a named void or async method on the target. Used for InsertMethod,
	/// UpdateMethod, and DeleteMethod which typically don't return data.
	/// </summary>
	/// <param name="target">The object to search for the method.</param>
	/// <param name="methodName">The name of the method to invoke.</param>
	/// <param name="args">Optional arguments to pass to the method.</param>
	public static async Task InvokeActionMethodAsync(object target, string methodName, params object[] args)
	{
		var method = FindMethod(target, methodName, args.Length);
		var result = method.Invoke(method.IsStatic ? null : target, args.Length > 0 ? args : null);

		if (result is Task task)
		{
			await task.ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Checks whether the target object has a method with the given name.
	/// </summary>
	public static bool HasMethod(object target, string methodName)
	{
		if (target == null || string.IsNullOrWhiteSpace(methodName))
		{
			return false;
		}

		return TryFindQualifiedStaticMethod(methodName) != null ||
			FindMethodOnType(target.GetType(), methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null;
	}

	private static MethodInfo FindMethod(object target, string methodName, int? expectedParamCount = null)
	{
		var type = target.GetType();
		var method = TryFindQualifiedStaticMethod(methodName, expectedParamCount);

		// Try public instance methods first, then non-public (protected methods in code-behind)
		method ??= FindMethodOnType(type, methodName,
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, expectedParamCount);

		if (method == null)
		{
			var additionalContext = methodName.Contains('.', StringComparison.Ordinal)
				? " or as a static method on the named type"
				: string.Empty;

			throw new InvalidOperationException(
				$"SelectMethod/InsertMethod/UpdateMethod/DeleteMethod resolution failed: " +
				$"No method named '{methodName}' was found on type '{type.Name}'{additionalContext}. " +
				$"Ensure the method exists on the page class that hosts this data-bound component. " +
				$"The page must inherit from WebFormsPageBase to enable string-based method resolution.");
		}

		return method;
	}

	private static MethodInfo FindMethodOnType(Type type, string methodName, BindingFlags flags, int? expectedParamCount = null)
	{
		MethodInfo method = null;

		if (expectedParamCount.HasValue)
		{
			method = type.GetMethods(flags)
				.FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == expectedParamCount.Value);
		}

		method ??= type.GetMethods(flags)
			.FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 0);

		method ??= type.GetMethod(methodName, flags);
		return method;
	}

	private static MethodInfo TryFindQualifiedStaticMethod(string methodName, int? expectedParamCount = null)
	{
		var lastDotIndex = methodName.LastIndexOf('.');
		if (lastDotIndex <= 0 || lastDotIndex == methodName.Length - 1)
		{
			return null;
		}

		var typeName = methodName[..lastDotIndex];
		var actualMethodName = methodName[(lastDotIndex + 1)..];
		var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (var candidateType in GetLoadableTypes(assembly)
				.Where(t => string.Equals(t.Name, typeName, StringComparison.Ordinal)
					|| string.Equals(t.FullName, typeName, StringComparison.Ordinal)))
			{
				var method = FindMethodOnType(candidateType, actualMethodName, flags, expectedParamCount);
				if (method != null)
				{
					return method;
				}
			}
		}

		return null;
	}

	private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where(t => t != null)!;
		}
	}

	private static object InvokeMethod(object target, MethodInfo method)
	{
		var parameters = method.GetParameters();

		if (parameters.Length == 0)
		{
			return method.Invoke(method.IsStatic ? null : target, null);
		}

		// Build default arguments for optional/defaulted parameters
		var args = new object[parameters.Length];
		for (var i = 0; i < parameters.Length; i++)
		{
			var param = parameters[i];
			if (param.HasDefaultValue)
			{
				args[i] = param.DefaultValue;
			}
			else if (param.ParameterType == typeof(int))
			{
				// Pagination: maxRows defaults to int.MaxValue, startRowIndex to 0
				args[i] = param.Name?.Contains("max", StringComparison.OrdinalIgnoreCase) == true
					? int.MaxValue : 0;
			}
			else if (param.ParameterType == typeof(string))
			{
				args[i] = string.Empty;
			}
			else if (param.IsOut)
			{
				args[i] = param.ParameterType.IsByRef
					? Activator.CreateInstance(param.ParameterType.GetElementType()!)
					: null;
			}
			else if (param.ParameterType.IsValueType)
			{
				args[i] = Activator.CreateInstance(param.ParameterType);
			}
			else
			{
				args[i] = null;
			}
		}

		return method.Invoke(method.IsStatic ? null : target, args);
	}

	private static IEnumerable<T> CastResult<T>(object result, string methodName, Type targetType)
	{
		if (result == null) return null;

		// Direct match: IEnumerable<T>
		if (result is IEnumerable<T> typed)
			return typed;

		// IQueryable<T> implements IEnumerable<T>, so it's caught above.
		// Handle non-generic IQueryable/IEnumerable via OfType<T>
		if (result is IEnumerable enumerable)
			return enumerable.OfType<T>();

		throw new InvalidOperationException(
			$"The method '{methodName}' on '{targetType.Name}' returned a value of type " +
			$"'{result.GetType().Name}' which cannot be converted to IEnumerable<{typeof(T).Name}>. " +
			$"SelectMethod methods should return IEnumerable<T>, IQueryable<T>, List<T>, or T[].");
	}
}
