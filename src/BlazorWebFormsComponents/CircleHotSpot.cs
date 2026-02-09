namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Defines a circular hot spot region in an ImageMap control.
	/// </summary>
	public class CircleHotSpot : HotSpot
	{
		/// <summary>
		/// Gets or sets the x-coordinate of the center of the circular region defined by this CircleHotSpot object.
		/// </summary>
		public int X { get; set; }

		/// <summary>
		/// Gets or sets the y-coordinate of the center of the circular region defined by this CircleHotSpot object.
		/// </summary>
		public int Y { get; set; }

		/// <summary>
		/// Gets or sets the distance from the center to the edge of the circular region defined by this CircleHotSpot object.
		/// </summary>
		public int Radius { get; set; }

		/// <summary>
		/// Gets the shape type for this hot spot.
		/// </summary>
		/// <returns>"circle"</returns>
		public override string GetShapeType() => "circle";

		/// <summary>
		/// Gets the coordinates for this circular hot spot.
		/// </summary>
		/// <returns>A comma-separated string of coordinates in format: x,y,radius</returns>
		public override string GetCoordinates() => $"{X},{Y},{Radius}";
	}
}
