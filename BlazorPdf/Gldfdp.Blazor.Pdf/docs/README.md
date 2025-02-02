# Gldfdp.Blazor.Pdf

To use this package in your project, you need to add the package and the required dependencies. You can do this by running the following command in your project directory:
```bash
dotnet add package Gldfdp.Blazor.Pdf
```

After adding the package, you need :

- to add the following line in your `Program.cs` file:
```csharp
builder.Services.RegisterBlazorPdf();
```

- please see how to use it in your component in the [sample](https://github.com/gldfdp/blazorpdf/blob/main/BlazorPdf/DemoApp/Pages/Home.razor)

