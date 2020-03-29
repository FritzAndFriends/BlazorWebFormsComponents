using System;
using System.IO;
using Bunit;

namespace BlazorWebFormsComponents.Test
{
  public class TestComponent : TestComponentBase
  {
		public TestComponent()
		{
			ContentRoot = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
			Directory.SetCurrentDirectory(ContentRoot);
		}

		protected string ContentRoot { get; }
  }
}
