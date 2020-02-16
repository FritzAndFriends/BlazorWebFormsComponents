﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public static class Helpers
	{

		private static MarkupString StackWalker()
		{
			var sb = new StringBuilder();

			for (var i = 2; i < 9; i++)
			{
				var frame = new StackFrame(i);
				var parms = frame.GetMethod().GetParameters();
				var names = string.Join(',', parms.Select(p => p.ParameterType.FullName).ToArray());
				sb.AppendLine($"Frame({i-1}): {frame.GetMethod().Name} {names}");
			}

			return new MarkupString(sb.ToString());
		}
	}

}
