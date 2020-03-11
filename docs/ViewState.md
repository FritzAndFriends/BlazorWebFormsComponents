# ViewState

One of the most abused and misunderstood features of ASP<span></span>.NET Web Forms is ViewState.  Originally designed to help stateful Visual Basic 6 Windows application developers migrate to the web, this feature delivered a lot of heartache to developers who didn't fully understand and limit its usage appropriately in their web applications.  In Web Forms, every `Page` and every `Control` carries a ViewState object that contains the state of the Control or Page at the time of request, serialized and output as an HTML hidden input tag.  Without proper diligence in selecting which controls and features need their state tracked, many applications found this HTML tag fill up with megabytes of serialized code and slowing their applications.

Elsewhere in Web Forms, ViewState could be used as a key/value store to place data that should be remembered during the current fetch/post/retrieve cycle for a page.  You would find syntax like the following in Pages and Controls to add the `bar` object with a key named `foo`:

```c#
ViewState.Add("foo", bar);
```

or to fetch the item named `foo` from ViewState:

```c#
ViewState["foo"]
```

It's a simple syntax and implements a typical Dictionary pattern.  After the PreRender phase of a Page, the content would be serialized and inserted into an HTML hidden input so that it would be available when the Page was posted.

## Implementation

In BlazorWebFormsComponents, we want to preserve this API but provide a proper Blazor implementation.  To do this, we have provided a basic `Dictionary<string,object>` property called `ViewState` that you can add items to and retrieve from.  This ViewState is preserved for the duration of the life of the component as your user interacts with it.  This is an in-memory storage on the server (in the case of server-side Blazor) or in the Browser as part of Blazor Web-Assembly.

You can see the implementation in the BaseWebFormsComponent class.

## Moving On

ViewState is not a feature you should continue to use after migrating.  While we have already eliminated the serializeation / deserialization performance issue with ViewState, we still recommend moving any storage you do with ViewState to class-scoped fields and properties that can be validated and strongly-typed.  This will also reduce the boxing/unboxing performance issue that exists with ViewState.

You would define the property for Foo with a syntax similar to, substituting the proper type of bar for `BarType`:

```c#
protected BarType Foo { get; set; }
```

The interaction above would be updated to the following syntax in your component's class:

```c#
// Store bar for retrieval later
Foo = bar;

// Fetch bar for use
bar = Foo;
```

