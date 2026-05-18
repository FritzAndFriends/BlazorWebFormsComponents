using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Event arguments for postback events, mirroring the Web Forms postback model.
/// Contains the <see cref="EventTarget"/> (control ID) and <see cref="EventArgument"/>
/// that were passed to <c>__doPostBack(eventTarget, eventArgument)</c>.
/// </summary>
public class PostBackEventArgs : EventArgs
{
	/// <summary>The ID of the control that initiated the postback.</summary>
	public string EventTarget { get; }

	/// <summary>The argument string associated with the postback.</summary>
	public string EventArgument { get; }

	public PostBackEventArgs(string eventTarget, string eventArgument)
	{
		EventTarget = eventTarget;
		EventArgument = eventArgument;
	}
}
