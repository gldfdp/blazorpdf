using BlazorPdf.Dto;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPdf
{
    public partial class PdfViewer
    {
        private DotNetObjectReference<PdfViewer>? objRef;

        public PdfViewer()
        {
            this.Id = $"blazorpf-{Guid.NewGuid()}";
        }

        public string Id { get; set; }

        [Parameter]
        public required string PdfUrl { get; set; }

        [Inject]
        public required PdfViewerJsInterop PdfViewerJsInterop { get; set; }

        [Parameter]
        public string? Style { get; set; }

        public ElementReference Container { get; set; }

        protected override void OnInitialized()
        {
            this.objRef ??= DotNetObjectReference.Create(this);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                InitializationOptionsDto options = new()
                {
                    Url = this.PdfUrl
                };

                await this.PdfViewerJsInterop.InitializeAsync(this.objRef!, this.Container, options);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}