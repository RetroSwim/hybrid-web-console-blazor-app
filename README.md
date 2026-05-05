# Hybrid Console+Web Blazor App

Uses Bootstrap.js for web view, and RazorConsole for console view. Components have a simple proxy so the UI can be built in a mostly-agnostic way.

## To run console version

`dotnet run`

<img width="359" height="126" alt="cli" src="https://github.com/user-attachments/assets/c474bdd3-301c-4e60-9122-87334b72da14" />

## To run web version

`dotnet run --web`

<img width="370" height="158" alt="web" src="https://github.com/user-attachments/assets/607ca75a-718d-43a5-bb11-1dcd7f9eda06" />

## To run both at once

`dotnet run --both`

## Example Component

```csharp
@namespace HybridTest.Components.Hybrid
@using Microsoft.AspNetCore.Components
@using HybridTest.Services
@using RazorConsole.Components
@inject IHostingMode HostingMode

@if (HostingMode.IsWeb)
{
    <div class="container">
        <div class="row">@ChildContent</div>
    </div>
}
else
{
    <Columns>@ChildContent</Columns>
}

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

