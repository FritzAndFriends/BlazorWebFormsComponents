# Contributing to BlazorWebFormsComponents

Thank you for taking the time to consider contributing to our project.  

The following is a set of guidelines for contributing to the project.  These are mostly guidelines, not rules, and can be changed in the future.  Please submit your suggestions with a pull-request to this document.

1. [Code of Conduct](#code-of-conduct)
1. [What should I know before I get started?](#what-should-i-know-before-i-get-started?)
    1. [Project Folder Structure](#project-folder-structure) 


## Code of Conduct

We have adopted a code of conduct from the Contributor Covenant.  Contributors to this project are expected to adhere to this code.  Please report unwanted behavior to [jeff@jeffreyfritz.com](mailto:jeff@jeffreyfritz.com)

## What should I know before I get started?

This project is currently a single library that will provide a shim, a buffer that will help you convert markup to run in Blazor. The project will grow in the future to support more automated conversion from ASP<span></span>.NET Web Forms to Blazor.

### Project Folder Structure

This project is designed to be built and run primarily with Visual Studio 2019. The folders are configured so that they will support editing and working in other editors and on non-Windows operating systems.  We encourage you to develop with these other environments, because we would like to be able to support developers who use those tools as well.  The folders are configured as follows:

```
/docs         -- User Documentation
/samples      -- Usage Samples 
  BeforeWebForms
  AfterBlazorClientSide
  AfterBlazorServerSide
/src          -- Library source and unit tests
  BlazorWebFormsComponents
  BlazorWebFormsComponents.Test
```

We may add a top level `tests` folder if there are integration tests to show at some point.

All official versions of the project are built and delivered with Azure DevOps Pipelines and linked in the main README.md and [releases tab in GitHub](https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases).

