using System.Collections.Generic;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Creates an image map control that displays an image with defined clickable hot spot regions.
	/// </summary>
	public partial class ImageMap : BaseWebFormsComponent, IImageComponent
	{
		/// <summary>
		/// Gets or sets the alternate text to display in the ImageMap control when the image is unavailable.
		/// </summary>
		[Parameter]
		public string AlternateText { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the URL to a detailed description of the image in the ImageMap control.
		/// </summary>
		[Parameter]
		public string DescriptionUrl { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the alignment of the Image control in relation to other elements on the Web page.
		/// </summary>
		[Parameter]
		public ImageAlign ImageAlign { get; set; } = ImageAlign.NotSet;

		/// <summary>
		/// Gets or sets the URL to the image to display in the ImageMap control.
		/// </summary>
		[Parameter]
		public string ImageUrl { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the ToolTip text for the ImageMap control.
		/// </summary>
		[Parameter]
		public string ToolTip { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the default behavior for the HotSpot objects in the ImageMap control when the HotSpot objects are clicked.
		/// </summary>
		[Parameter]
		public HotSpotMode HotSpotMode { get; set; } = HotSpotMode.Navigate;

		/// <summary>
		/// Gets or sets the target window or frame in which to display the Web page content linked to when a HotSpot object in an ImageMap control is clicked.
		/// </summary>
		[Parameter]
		public string Target { get; set; } = string.Empty;

		/// <summary>
		/// Gets the collection of HotSpot objects defined in the ImageMap control.
		/// </summary>
		[Parameter]
		public List<HotSpot> HotSpots { get; set; } = new List<HotSpot>();

		/// <summary>
		/// Occurs when a HotSpot object in an ImageMap control is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<ImageMapEventArgs> OnClick { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates whether the ImageMap control generates an empty alternate text attribute.
		/// </summary>
		[Parameter]
		public bool GenerateEmptyAlternateText { get; set; }

		private string _mapId = string.Empty;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			// Generate a unique map ID
			_mapId = $"ImageMap_{GetHashCode()}";
		}

		/// <summary>
		/// Handles the click event for a hot spot.
		/// </summary>
		/// <param name="hotSpot">The hot spot that was clicked</param>
		protected async void HandleHotSpotClick(HotSpot hotSpot)
		{
			if (hotSpot.HotSpotMode == HotSpotMode.PostBack || 
			    (hotSpot.HotSpotMode == HotSpotMode.Navigate && HotSpotMode == HotSpotMode.PostBack))
			{
				var eventArgs = new ImageMapEventArgs(hotSpot.PostBackValue);
				await OnClick.InvokeAsync(eventArgs);
			}
		}
	}
}
