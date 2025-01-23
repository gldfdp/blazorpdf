using BlazorPdf.Dto;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPdf
{
     public class PdfViewerJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public PdfViewerJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorPdf/blazorpdf.js").AsTask());
        }

        public async ValueTask<PdfViewerInitializedDto> InitializeAsync(
            DotNetObjectReference<PdfViewer> dotNetHelper,
            ElementReference container,
            InitializationOptionsDto initializationOptions)
        {
            var module = await moduleTask.Value;

            return await module.InvokeAsync<PdfViewerInitializedDto>("init", dotNetHelper, container, initializationOptions);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
