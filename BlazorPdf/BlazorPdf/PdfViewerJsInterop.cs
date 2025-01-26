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
                "import", "./_content/BlazorPdf/blazorpdf.js?v=" + DateTime.UtcNow.Ticks).AsTask());
        }

        internal async ValueTask<PdfViewerInitializedDto> InitializeAsync(
            DotNetObjectReference<PdfViewer.BlazorPdfProxy> dotNetHelper,
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

        public async Task GoToPageAsync(string elementId, int pageNumber)
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("renderPage", elementId, pageNumber);
        }

        internal async Task StartSignaturePositionAsync(string elementId, int signatureIndex)
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("startSignaturePosition", elementId, signatureIndex);
        }
    }
}
