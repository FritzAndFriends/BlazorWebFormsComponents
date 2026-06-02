using BlazorWebFormsComponents;
using Microsoft.Extensions.Primitives;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace BlazorWebFormsComponents.Test.GridView
{
	public class GridFormDataParserTests
	{
		[Fact]
		public void Parse_ExtractsRowData_FromWebFormsStyleKeys()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["CartList$ctl02$PurchaseQuantity"] = "3",
				["CartList$ctl02$RemoveItem"] = "true",
				["CartList$ctl03$PurchaseQuantity"] = "1",
				["CartList$ctl03$RemoveItem"] = "false",
				["__RequestVerificationToken"] = "xyz"
			};

			var result = GridFormDataParser.Parse(formData, "CartList");

			result.Count.ShouldBe(2);
			result[0]["PurchaseQuantity"].ShouldBe("3");
			result[0]["RemoveItem"].ShouldBe("true");
			result[1]["PurchaseQuantity"].ShouldBe("1");
			result[1]["RemoveItem"].ShouldBe("false");
		}

		[Fact]
		public void Parse_IgnoresKeysFromDifferentGrid()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["CartList$ctl02$Qty"] = "3",
				["OtherGrid$ctl02$Name"] = "Widget"
			};

			var result = GridFormDataParser.Parse(formData, "CartList");

			result.Count.ShouldBe(1);
			result[0]["Qty"].ShouldBe("3");
		}

		[Fact]
		public void GetRowValues_ReturnsSingleRowData()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["Grid1$ctl02$Name"] = "Widget",
				["Grid1$ctl02$Price"] = "9.99",
				["Grid1$ctl03$Name"] = "Gadget",
				["Grid1$ctl03$Price"] = "19.99"
			};

			var row1 = GridFormDataParser.GetRowValues(formData, "Grid1", 1);

			row1["Name"].ShouldBe("Gadget");
			row1["Price"].ShouldBe("19.99");
		}

		[Fact]
		public void GetRowValues_ReturnsEmptyForMissingRow()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["Grid1$ctl02$Name"] = "Widget"
			};

			var row5 = GridFormDataParser.GetRowValues(formData, "Grid1", 5);

			row5.ShouldBeEmpty();
		}

		[Fact]
		public void GetValue_ReturnsSingleValue()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["CartList$ctl04$PurchaseQuantity"] = "7"
			};

			var value = GridFormDataParser.GetValue(formData, "CartList", 2, "PurchaseQuantity");

			value.ShouldBe("7");
		}

		[Fact]
		public void GetValue_ReturnsNull_WhenKeyNotFound()
		{
			var formData = new Dictionary<string, StringValues>
			{
				["CartList$ctl02$PurchaseQuantity"] = "3"
			};

			var value = GridFormDataParser.GetValue(formData, "CartList", 0, "NonExistent");

			value.ShouldBeNull();
		}

		[Fact]
		public void Parse_HandlesEmptyFormData()
		{
			var formData = new Dictionary<string, StringValues>();

			var result = GridFormDataParser.Parse(formData, "CartList");

			result.ShouldBeEmpty();
		}

		[Fact]
		public void Parse_HandlesNullFormData()
		{
			var result = GridFormDataParser.Parse(null, "CartList");

			result.ShouldBeEmpty();
		}

		[Fact]
		public void FormNamingContext_GetRowCtlId_FollowsWebFormsConvention()
		{
			// Row 0 = ctl02 (header is conceptually ctl01)
			FormNamingContext.GetRowCtlId(0).ShouldBe("ctl02");
			FormNamingContext.GetRowCtlId(1).ShouldBe("ctl03");
			FormNamingContext.GetRowCtlId(8).ShouldBe("ctl10");
			FormNamingContext.GetRowCtlId(98).ShouldBe("ctl100");
		}
	}
}
