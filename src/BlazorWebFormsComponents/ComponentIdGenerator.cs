using System;

namespace BlazorWebFormsComponents
{
  public class ComponentIdGenerator
  {
		public static string Generate(BaseWebFormsComponent component)
		{
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
