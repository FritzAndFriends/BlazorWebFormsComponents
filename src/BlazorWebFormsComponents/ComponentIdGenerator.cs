using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a utility class to generate the component Id.
	/// </summary>
  public class ComponentIdGenerator
  {
		/// <summary>
		/// Generates an Id for a given component.
		/// </summary>
		/// <param name="component">The <see cref="BaseWebFormsComponent"/>.</param>
		/// <returns>The generated component Id.</returns>
		/// <remarks>The generated Id format is {ComponentType}_{GUID} if the <see cref="BaseWebFormsComponent.ID"/> is not set.</remarks>
		/// <example>Login_8a1112a8-bdf3-4a3c-91f6-caa9ee1ff974.</example>
		public static string Generate(BaseWebFormsComponent component)
		{
			// TODO: Use ClientIDMode
			var generatedId = component.ID;
			if (!string.IsNullOrEmpty(generatedId))
			{
				return generatedId;
			}

			if (component.AdditionalAttributes == null)
			{
				return GenerateId(component);
			}

			generatedId = component.AdditionalAttributes.ContainsKey("name")
						? component.AdditionalAttributes["name"].ToString()
						: GenerateId(component);

			return generatedId;

			string GenerateId(BaseWebFormsComponent component)
				=> $"{component.GetType().Name}_{Guid.NewGuid().ToString()}";
		}
	}
}
