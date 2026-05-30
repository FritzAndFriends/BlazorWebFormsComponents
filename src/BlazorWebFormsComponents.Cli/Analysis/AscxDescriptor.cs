namespace BlazorWebFormsComponents.Cli.Analysis;

public sealed class AscxDescriptor
{
    public string ControlName { get; init; } = string.Empty;
    public string? MarkupPath { get; init; }
    public string? CodeBehindPath { get; init; }
    public string? InheritsTypeName { get; init; }
    public string? ClassName { get; init; }
    public string? BaseTypeName { get; init; }
    public bool CodeBehindExists { get; init; }
    public bool ParseSucceeded { get; init; }
    public bool HasDataBindCall { get; init; }
    public bool HasPageLoadOverride { get; init; }
    public IReadOnlyList<AscxPropertyDescriptor> Properties { get; init; } = [];
    public IReadOnlyList<AscxEventDescriptor> Events { get; init; } = [];
    public IReadOnlyList<AscxMethodDescriptor> Methods { get; init; } = [];
    public IReadOnlyList<string> ReferencedControlIds { get; init; } = [];
    public IReadOnlyList<string> Diagnostics { get; init; } = [];
}

public sealed record AscxPropertyDescriptor(
    string Name,
    string Type,
    bool HasGetter,
    bool HasSetter,
    string? DefaultValue = null);

public sealed record AscxEventDescriptor(
    string Name,
    string DelegateType);

public sealed record AscxMethodDescriptor(
    string Name,
    string ReturnType,
    IReadOnlyList<AscxMethodParameterDescriptor> Parameters);

public sealed record AscxMethodParameterDescriptor(
    string Name,
    string Type);
