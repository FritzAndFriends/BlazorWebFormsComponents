using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Turns an Image control into an automatic slideshow that cycles through images.
/// Emulates the Ajax Control Toolkit SlideShowExtender.
/// Supports optional navigation controls and auto-play functionality.
/// </summary>
public class SlideShowExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/slide-show-extender.js";

	/// <summary>
	/// URL path to a service that provides slide data.
	/// For Blazor, this is optional - use <see cref="Slides"/> for client-side data.
	/// </summary>
	[Parameter]
	public string SlideShowServicePath { get; set; } = string.Empty;

	/// <summary>
	/// Method name to call on the service.
	/// </summary>
	[Parameter]
	public string SlideShowServiceMethod { get; set; } = string.Empty;

	/// <summary>
	/// Context key to pass to the service method.
	/// </summary>
	[Parameter]
	public string ContextKey { get; set; } = string.Empty;

	/// <summary>
	/// Whether to use the context key when calling the service.
	/// </summary>
	[Parameter]
	public bool UseContextKey { get; set; } = false;

	/// <summary>
	/// ID of an element to display the current image description.
	/// </summary>
	[Parameter]
	public string ImageDescriptionLabelID { get; set; } = string.Empty;

	/// <summary>
	/// ID of a button element that navigates to the next slide.
	/// </summary>
	[Parameter]
	public string NextButtonID { get; set; } = string.Empty;

	/// <summary>
	/// ID of a button element that navigates to the previous slide.
	/// </summary>
	[Parameter]
	public string PreviousButtonID { get; set; } = string.Empty;

	/// <summary>
	/// ID of a button element that toggles play/pause.
	/// </summary>
	[Parameter]
	public string PlayButtonID { get; set; } = string.Empty;

	/// <summary>
	/// Text to show on the play button when slideshow is paused.
	/// </summary>
	[Parameter]
	public string PlayButtonText { get; set; } = "Play";

	/// <summary>
	/// Text to show on the play button when slideshow is playing.
	/// </summary>
	[Parameter]
	public string StopButtonText { get; set; } = "Stop";

	/// <summary>
	/// Interval in milliseconds between automatic slide transitions.
	/// </summary>
	[Parameter]
	public int PlayInterval { get; set; } = 3000;

	/// <summary>
	/// Whether the slideshow loops back to the first slide after the last.
	/// </summary>
	[Parameter]
	public bool Loop { get; set; } = true;

	/// <summary>
	/// Whether to start playing automatically when loaded.
	/// </summary>
	[Parameter]
	public bool AutoPlay { get; set; } = false;

	/// <summary>
	/// ID of an element to display the current image title.
	/// </summary>
	[Parameter]
	public string ImageTitleLabelID { get; set; } = string.Empty;

	/// <summary>
	/// Client-side collection of slides to display.
	/// Each slide should have ImageUrl, Title, and Description properties.
	/// </summary>
	[Parameter]
	public IEnumerable<SlideShowSlide> Slides { get; set; }

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		slideShowServicePath = SlideShowServicePath,
		slideShowServiceMethod = SlideShowServiceMethod,
		contextKey = ContextKey,
		useContextKey = UseContextKey,
		imageDescriptionLabelId = ImageDescriptionLabelID,
		nextButtonId = NextButtonID,
		previousButtonId = PreviousButtonID,
		playButtonId = PlayButtonID,
		playButtonText = PlayButtonText,
		stopButtonText = StopButtonText,
		playInterval = PlayInterval,
		loop = Loop,
		autoPlay = AutoPlay,
		imageTitleLabelId = ImageTitleLabelID,
		slides = Slides
	};
}

/// <summary>
/// Represents a single slide in the slideshow.
/// </summary>
public class SlideShowSlide
{
	/// <summary>
	/// URL of the image to display.
	/// </summary>
	public string ImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// Title of the slide (shown in ImageTitleLabelID element).
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// Description of the slide (shown in ImageDescriptionLabelID element).
	/// </summary>
	public string Description { get; set; } = string.Empty;
}
