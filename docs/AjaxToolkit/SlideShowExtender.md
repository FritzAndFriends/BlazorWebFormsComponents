# SlideShowExtender

The **SlideShowExtender** turns an Image control into an automatic slideshow that cycles through a collection of images. It supports automatic playback, manual navigation controls, optional image titles and descriptions, and looping. Images can be provided client-side as a collection or loaded from a service.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/SlideShowExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the Image control
- `Slides` — Client-side collection of slides (IEnumerable<SlideShowSlide>)
- `AutoPlay` — Whether to start playing automatically
- `Loop` — Whether to loop back to first slide after last
- `PlayInterval` — Interval in milliseconds between transitions
- `NextButtonID` — ID of next button element
- `PreviousButtonID` — ID of previous button element
- `PlayButtonID` — ID of play/pause button element
- `PlayButtonText` — Text for play button when paused
- `StopButtonText` — Text for play button when playing
- `ImageTitleLabelID` — ID of element for image titles
- `ImageDescriptionLabelID` — ID of element for descriptions
- `SlideShowServicePath` — Optional service URL for server-side slides
- `SlideShowServiceMethod` — Service method name
- `ContextKey` — Context key for service calls

## Slide Model

```csharp
public class SlideShowSlide
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

## Web Forms Syntax

```html
<asp:Image ID="imgSlide" runat="server" Width="400" Height="300" />

<div>
    <button id="btnPrev">Previous</button>
    <button id="btnPlay">Play</button>
    <button id="btnNext">Next</button>
</div>

<div id="lblTitle"></div>
<div id="lblDescription"></div>

<ajaxToolkit:SlideShowExtender
    ID="slide1"
    runat="server"
    TargetControlID="imgSlide"
    PlayInterval="3000"
    AutoPlay="true"
    Loop="true"
    NextButtonID="btnNext"
    PreviousButtonID="btnPrev"
    PlayButtonID="btnPlay"
    PlayButtonText="Play"
    StopButtonText="Stop"
    ImageTitleLabelID="lblTitle"
    ImageDescriptionLabelID="lblDescription" />
```

## Blazor Migration

```razor
<Image ID="imgSlide" width="400" height="300" />

<div>
    <button id="btnPrev">Previous</button>
    <button id="btnPlay">Play</button>
    <button id="btnNext">Next</button>
</div>

<div id="lblTitle"></div>
<div id="lblDescription"></div>

<SlideShowExtender
    TargetControlID="imgSlide"
    PlayInterval="3000"
    AutoPlay="true"
    Loop="true"
    NextButtonID="btnNext"
    PreviousButtonID="btnPrev"
    PlayButtonID="btnPlay"
    PlayButtonText="Play"
    StopButtonText="Stop"
    ImageTitleLabelID="lblTitle"
    ImageDescriptionLabelID="lblDescription"
    Slides="slides" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Pass slides via the `Slides` parameter!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the Image control |
| `Slides` | `IEnumerable<SlideShowSlide>` | `null` | Client-side collection of slides to display |
| `AutoPlay` | `bool` | `false` | Whether to start playing automatically |
| `Loop` | `bool` | `true` | Whether to loop back to first slide after last |
| `PlayInterval` | `int` | `3000` | Interval in milliseconds between automatic transitions |
| `NextButtonID` | `string` | `""` | ID of button element for next slide |
| `PreviousButtonID` | `string` | `""` | ID of button element for previous slide |
| `PlayButtonID` | `string` | `""` | ID of button element to toggle play/pause |
| `PlayButtonText` | `string` | `"Play"` | Text for play button when slideshow is paused |
| `StopButtonText` | `string` | `"Stop"` | Text for play button when slideshow is playing |
| `ImageTitleLabelID` | `string` | `""` | ID of element to display current image title |
| `ImageDescriptionLabelID` | `string` | `""` | ID of element to display current image description |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Slideshow with Navigation

```razor
@rendermode InteractiveServer

@code {
    private List<SlideShowSlide> slides = new()
    {
        new() { ImageUrl = "/images/slide1.jpg", Title = "Slide 1", Description = "First image" },
        new() { ImageUrl = "/images/slide2.jpg", Title = "Slide 2", Description = "Second image" },
        new() { ImageUrl = "/images/slide3.jpg", Title = "Slide 3", Description = "Third image" }
    };
}

<div style="border: 1px solid #ccc; padding: 10px; text-align: center;">
    <Image ID="imgSlideshow" width="500" height="350" style="display: block; margin: 0 auto;" />
    
    <div id="slideTitle" style="font-size: 18px; font-weight: bold; margin: 10px 0;"></div>
    <div id="slideDesc" style="color: #666; margin-bottom: 10px;"></div>
    
    <div style="margin: 10px 0;">
        <button id="btnPrev" class="btn btn-sm btn-secondary" style="margin-right: 5px;">← Previous</button>
        <button id="btnPlay" class="btn btn-sm btn-primary">Play</button>
        <button id="btnNext" class="btn btn-sm btn-secondary" style="margin-left: 5px;">Next →</button>
    </div>
</div>

<SlideShowExtender
    TargetControlID="imgSlideshow"
    PlayInterval="3000"
    AutoPlay="false"
    Loop="true"
    NextButtonID="btnNext"
    PreviousButtonID="btnPrev"
    PlayButtonID="btnPlay"
    PlayButtonText="Play"
    StopButtonText="Stop"
    ImageTitleLabelID="slideTitle"
    ImageDescriptionLabelID="slideDesc"
    Slides="slides" />
```

### Automatic Slideshow

```razor
@rendermode InteractiveServer

@code {
    private List<SlideShowSlide> productImages = new()
    {
        new() { ImageUrl = "/products/chair1.jpg", Title = "Chair - Side View" },
        new() { ImageUrl = "/products/chair2.jpg", Title = "Chair - Front View" },
        new() { ImageUrl = "/products/chair3.jpg", Title = "Chair - Detail" }
    };
}

<div style="text-align: center; background-color: #f5f5f5; padding: 20px; border-radius: 5px;">
    <Image ID="imgProduct" width="400" height="400" />
    <h3 id="productTitle" style="margin: 10px 0;"></h3>
</div>

<SlideShowExtender
    TargetControlID="imgProduct"
    PlayInterval="2500"
    AutoPlay="true"
    Loop="true"
    ImageTitleLabelID="productTitle"
    Slides="productImages" />
```

### Manual Navigation Only

```razor
@rendermode InteractiveServer

@code {
    private List<SlideShowSlide> gallerySlides = new()
    {
        new() { ImageUrl = "/gallery/img1.jpg", Title = "Sunset", Description = "Beautiful sunset over the ocean" },
        new() { ImageUrl = "/gallery/img2.jpg", Title = "Mountain", Description = "Snow-capped mountain peaks" },
        new() { ImageUrl = "/gallery/img3.jpg", Title = "Forest", Description = "Dense green forest" },
        new() { ImageUrl = "/gallery/img4.jpg", Title = "Desert", Description = "Golden sand dunes" }
    };
}

<div style="max-width: 600px; margin: 0 auto;">
    <Image ID="imgGallery" width="100%" style="display: block; border: 1px solid #ddd;" />
    
    <div id="galleryTitle" style="font-size: 20px; font-weight: bold; margin-top: 10px;"></div>
    <div id="galleryDesc" style="color: #666; margin-bottom: 15px;"></div>
    
    <div style="text-align: center;">
        <button id="btnGalleryPrev" class="btn btn-primary" style="margin-right: 10px;">← Previous</button>
        <button id="btnGalleryNext" class="btn btn-primary">Next →</button>
    </div>
</div>

<SlideShowExtender
    TargetControlID="imgGallery"
    AutoPlay="false"
    Loop="false"
    NextButtonID="btnGalleryNext"
    PreviousButtonID="btnGalleryPrev"
    ImageTitleLabelID="galleryTitle"
    ImageDescriptionLabelID="galleryDesc"
    Slides="gallerySlides" />
```

## HTML Output

The SlideShowExtender produces no HTML itself — it attaches JavaScript behavior to the target Image control and button elements.

## JavaScript Interop

The SlideShowExtender loads `slide-show-extender.js` as an ES module. JavaScript handles:

- Managing slide collection (client-side or service-side)
- Displaying images in sequence
- Automatic playback with `PlayInterval`
- Manual navigation via button clicks
- Updating title and description labels
- Play/pause toggle functionality
- Looping behavior (restart from first slide or stop)
- Button state management

## Render Mode Requirements

The SlideShowExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. First image displays without slideshow.
- **JavaScript disabled:** Same as SSR — No slideshow functionality.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:SlideShowExtender
   + <SlideShowExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Pass slides via parameter**
   ```diff
   - Server-side slide data
   + Slides="slideList"
   ```

### Before (Web Forms)

```html
<asp:Image ID="imgSlide" runat="server" Width="400" Height="300" />

<button id="btnPrev">Previous</button>
<button id="btnPlay">Play</button>
<button id="btnNext">Next</button>

<ajaxToolkit:SlideShowExtender
    ID="slide1"
    TargetControlID="imgSlide"
    PlayInterval="3000"
    AutoPlay="true"
    NextButtonID="btnNext"
    PreviousButtonID="btnPrev"
    PlayButtonID="btnPlay"
    runat="server" />
```

### After (Blazor)

```razor
<Image ID="imgSlide" width="400" height="300" />

<button id="btnPrev">Previous</button>
<button id="btnPlay">Play</button>
<button id="btnNext">Next</button>

<SlideShowExtender
    TargetControlID="imgSlide"
    PlayInterval="3000"
    AutoPlay="true"
    NextButtonID="btnNext"
    PreviousButtonID="btnPrev"
    PlayButtonID="btnPlay"
    Slides="slides" />
```

## Best Practices

1. **Provide navigation buttons** — Always include previous/next buttons even if autoplay is enabled
2. **Set reasonable interval** — 2500-4000ms typically works well for viewer comfort
3. **Include titles and descriptions** — Help viewers understand what they're seeing
4. **Test with small/large collections** — Verify performance with 5+ slides
5. **Use high-quality images** — Optimize images for web to ensure smooth transitions
6. **Consider accessibility** — Provide alternative text for images via titles

## Troubleshooting

| Issue | Solution |
|---|---|
| Slideshow not starting | Verify `TargetControlID` matches the Image's ID and `Slides` parameter is populated. Ensure `@rendermode InteractiveServer`. |
| Buttons not working | Ensure button IDs in `NextButtonID`, `PreviousButtonID`, etc. match actual button element IDs. |
| Images not displaying | Verify `ImageUrl` paths are correct and images are accessible. Check browser console for 404 errors. |
| Title/description not updating | Verify `ImageTitleLabelID` and `ImageDescriptionLabelID` match actual element IDs. |
| Autoplay not starting | Set `AutoPlay="true"` and verify PlayInterval is a reasonable value (not 0). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- Image Component — The Image control (documentation coming soon)
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
