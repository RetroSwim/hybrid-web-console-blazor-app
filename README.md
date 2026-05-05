# Hybrid Console+Web Blazor App

Uses Bootstrap.js for web view, and RazorConsole for console view. Components have a simple proxy so the UI can be built in a mostly-agnostic way.

## To run console version

`dotnet run`

<img width="974" height="390" alt="cli" src="https://github.com/user-attachments/assets/c9a566d5-a15b-4ee9-992e-43963c6320f3" />

## To run web version

`dotnet run --web`

<img width="1000" height="430" alt="web" src="https://github.com/user-attachments/assets/92bb5389-a40e-4660-a3d3-73be3a4c6813" />

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

