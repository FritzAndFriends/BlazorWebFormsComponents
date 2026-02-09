namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Defines a polygon-shaped hot spot region in an ImageMap control.
	/// </summary>
	public class PolygonHotSpot : HotSpot
	{
		/// <summary>
		/// Gets or sets a string of coordinates that represents the vertexes of a PolygonHotSpot object.
		/// </summary>
		public string Coordinates { get; set; } = string.Empty;

		/// <summary>
		/// Gets the shape type for this hot spot.
		/// </summary>
		/// <returns>"poly"</returns>
		public override string GetShapeType() => "poly";

		/// <summary>
		/// Gets the coordinates for this polygonal hot spot.
		/// </summary>
		/// <returns>A comma-separated string of x,y coordinate pairs</returns>
		public override string GetCoordinates() => Coordinates;
	}
}
