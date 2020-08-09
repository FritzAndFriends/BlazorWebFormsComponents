namespace Library
{
	public class CartLineItem
	{

		public int LineItemID { get; set; }

		public string ProductName { get; set; }

		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }

		public decimal TotalPrice => Quantity * UnitPrice;

	}
}
