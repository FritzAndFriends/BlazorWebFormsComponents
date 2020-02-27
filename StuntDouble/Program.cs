using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StuntDouble
{
	class Program
	{
		private const string FunctionPattern = "function(";

		static void Main(string[] args)
		{
			string text;
			var fileName = @"E:\Dev\OpenSource\BlazorWebFormsComponents\src\BlazorWebFormsComponents\wwwroot\js\Basepage.js";
			var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
			{
				text = streamReader.ReadToEnd();
			}

			var lines = text.Split(new[] { '\r', '\n' });

			var functionLines = new List<string>();
			foreach (var line in lines)
			{
				if (line.Contains(FunctionPattern))
				{
					functionLines.Add(line.Substring(0, line.IndexOf(FunctionPattern)).Trim(new []{' ', ':'}));
				}
			}

			// var functionLines = lines.Where(l => l.Contains(FunctionPattern)).Select((l) => l.Substring(0, l.IndexOf(FunctionPattern)));

			// var functionLine = string.Empty;
			//foreach (var line in lines)
			//{

			//	if (line.Contains(FunctionPattern))
			//	{
			//		var end = line.IndexOf(FunctionPattern);
			//		functionLine = line.Substring(0, end);
			//	}
			//}

			foreach (var functionLine in functionLines)
			{
				var functionName = functionLine.Trim().Trim(':');
			}	

			Console.WriteLine(text);

		}

	}
}

