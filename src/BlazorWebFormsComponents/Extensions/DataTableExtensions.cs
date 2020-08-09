using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BlazorWebFormsComponents.Extensions
{
	public static class DataTableExtensions
	{
		public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
		{
			if (table == null)
			{
				yield break;
			}

			var columns = table.Columns.Cast<DataColumn>();
			var columnNames = columns.Select(column => column.ColumnName);
			var rows = table.AsEnumerable()
					.Select(dataRow => columns.Select(column => new { Column = column.ColumnName, Value = dataRow[column] })
					.ToDictionary(data => data.Column, data => data.Value))
					.ToList();
			var objectType = GetObjectType(columnNames);
			foreach (var row in rows)
			{
				var obj = Activator.CreateInstance(objectType);
				var properties = obj.GetType().GetProperties();
				foreach (var rowData in row)
				{
					foreach (var property in properties)
					{
						if (property.Name == rowData.Key)
						{
							property.SetValue(obj, rowData.Value.ToString(), null);
							break;
						}
					}
				}

				yield return obj;
			}
		}

		private static Type GetObjectType(IEnumerable<string> propertiesNames)
		{
			var assemblyName = new AssemblyName("TempAssembly");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("TempModule");
			var typeBuilder = moduleBuilder.DefineType("GridRowCellCollection", TypeAttributes.Public);
			foreach (var propertyName in propertiesNames)
			{
				var fieldBuilder = typeBuilder.DefineField(propertyName, typeof(string), FieldAttributes.Public);
				var propertyBuilder = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.None, typeof(string), new Type[] { typeof(string) });

				var getPropertyMethodBuilder = typeBuilder.DefineMethod("get_value", MethodAttributes.Public | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
				var getILGenerator = getPropertyMethodBuilder.GetILGenerator();
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
				getILGenerator.Emit(OpCodes.Ret);
				propertyBuilder.SetGetMethod(getPropertyMethodBuilder);

				var setPropertyMethodBuilder = typeBuilder.DefineMethod("set_value", MethodAttributes.Public | MethodAttributes.HideBySig, null, new Type[] { typeof(string) });
				var setILGenerator = setPropertyMethodBuilder.GetILGenerator();
				setILGenerator.Emit(OpCodes.Ldarg_0);
				setILGenerator.Emit(OpCodes.Ldarg_1);
				setILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
				setILGenerator.Emit(OpCodes.Ret);
				propertyBuilder.SetSetMethod(setPropertyMethodBuilder);
			}

			return typeBuilder.CreateType();
		}
	}
}
