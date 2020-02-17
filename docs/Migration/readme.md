# Migration - Getting Started

Migration might not be the correct term for this process, it could appear to be more of a rewrite using Blazor.  In this article, you will learn how to get started rewriting your Web Forms application using Blazor with the Blazor Web Forms Components package.
  
<!-- TOC depthFrom:2 -->

- [Step 0 - Acknowledgement](#step-0---acknowledgement)
- [Step 1 - Readiness](#step-1---readiness)
- [Step 2 - Migrate Business Logic to .NET Standard](#step-2---migrate-business-logic-to-net-standard)
- [Step 3 - Create a new Blazor Server Project](#step-3---create-a-new-blazor-server-project)
- [Step 4 - Master Pages](#step-4---master-pages)
- [Step 5 - User Controls](#step-5---user-controls)
- [Step 6 - Pages](#step-6---pages)
- [Step X - Convert inline Visual Basic](#step-x---convert-inline-visual-basic)
- [Follow-up: Move components to Razor Component Library](#follow-up-move-components-to-razor-component-library)

<!-- /TOC -->

## Step 0 - Acknowledgement

The first step is a step of acknowledgement.  This process is not 100% and is not guaranteed to deliver a Blazor application without some amount of rewriting.  Applications are written in many different ways, and the tools provided here are attempting to get your project *CLOSE* to Blazor so that you have to rewrite as little code as possible.

[Back to top](#Migration---Getting-Started)

## Step 1 - Readiness

There are good application architectures and there are not-so-good application architectures to be considered for migration to Blazor.  We've written another document to help you evaluate the [readiness of your application for migration](migration_readiness.md).  It is recommended you read through that documentation to understand what makes an application better prepared for migration to Blazor.

[Back to top](#Migration---Getting-Started)

## Step 2 - Migrate Business Logic to .NET Standard

.NET Standard is the new recommended way to package and reuse business logic across projects and .NET runtimes.  We recommend you migrate:
  - Any class libraries referenced
  - Any classes in your web project that do NOT directly communicate with the web request or response

There is a separate [strategy document](../Strategies/NET-Standard.md) with instructions to migrate your code to .NET Standard libraries.  The goal of the exercise is to place all of your business logic into .NET Standard 2.0 libraries.  This version of .NET Standard will allow you to reference the libraries in both your existing Web Forms application and in your new Blazor application.

**A side benefit**: this is a good architecture practice that should allow you to test your business logic independently from your web project.  Try starting a unit test project with xUnit, NUnit or MSTest to exercise some of your business logic.  You will be able to run your tests either in the Visual Studio Test Runner or at the command line using `dotnet test`

[Back to top](#Migration---Getting-Started)

## Step 3 - Create a new Blazor Server Project

 - Add references to .NET Standard projects

## Step 4 - Master Pages

[Back to top](#Migration---Getting-Started)

## Step 5 - User Controls

 - Copy `ASCX` and `ASCX.cs` files into `Components` folder
 - Rename and convert to `razor` and `razor.cs`

[Back to top](#Migration---Getting-Started)

## Step 6 - Pages

 - Copy `ASPX` and `ASPX.cs` files into `Pages` folder
 - Rename and convert to `razor` and `razor.cs`

[Back to top](#Migration---Getting-Started)

## Step X - Convert inline Visual Basic

Use the tool from Telerik at: https://converter.telerik.com/

[Back to top](#Migration---Getting-Started)

## Follow-up: Move components to Razor Component Library

[Back to top](#Migration---Getting-Started)


