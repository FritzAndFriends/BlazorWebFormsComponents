namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Defines a rectangular hot spot region in an ImageMap control.
	/// </summary>
	public class RectangleHotSpot : HotSpot
	{
		/// <summary>
		/// Gets or sets the x-coordinate of the left side of the rectangular region defined by this RectangleHotSpot object.
		/// </summary>
		public int Left { get; set; }

		/// <summary>
		/// Gets or sets the y-coordinate of the top of the rectangular region defined by this RectangleHotSpot object.
		/// </summary>
		public int Top { get; set; }

		/// <summary>
		/// Gets or sets the x-coordinate of the right side of the rectangular region defined by this RectangleHotSpot object.
		/// </summary>
		public int Right { get; set; }

		/// <summary>
		/// Gets or sets the y-coordinate of the bottom of the rectangular region defined by this RectangleHotSpot object.
		/// </summary>
		public int Bottom { get; set; }

		/// <summary>
		/// Gets the shape type for this hot spot.
		/// </summary>
		/// <returns>"rect"</returns>
		public override string GetShapeType() => "rect";

		/// <summary>
		/// Gets the coordinates for this rectangular hot spot.
		/// </summary>
		/// <returns>A comma-separated string of coordinates in format: left,top,right,bottom</returns>
		public override string GetCoordinates() => $"{Left},{Top},{Right},{Bottom}";
	}
}
