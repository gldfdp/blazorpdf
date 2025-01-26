using BlazorPdf.Dto;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPdf
{
    public partial class PdfViewer
    {
        private DotNetObjectReference<BlazorPdfProxy>? objRef;

        private BlazorPdfProxy? proxy;

        public PdfViewer()
        {
            this.Id = $"blazorpf-{Guid.NewGuid()}";
        }

        public ElementReference Container { get; set; }

        public string Id { get; set; }

        public int Page { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        [Parameter]
        public required string PdfUrl { get; set; }

        [Parameter]
        public IEnumerable<SignatureLocation>? SignatureLocations { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<SignatureLocation>> OnValidated { get; set; }

        [Parameter]
        public PdfViewerLabels Labels { get; set; } = new();

        [Inject]
        public required PdfViewerJsInterop PdfViewerJsInterop { get; set; }

        [Parameter]
        public string? Style { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                InitializationOptionsDto options = new()
                {
                    Url = this.PdfUrl,
                    Signatures = this.SignatureLocations?.Select((location, index) => new SignaturePositionDto
                    {
                        DisplayName = location.DisplayName,
                        Height = location.Height,
                        Index = index,
                        Page = location.Page,
                        Width = location.Width,
                        X = location.X,
                        Y = location.Y
                    })
                };

                var instance = await this.PdfViewerJsInterop.InitializeAsync(this.objRef!, this.Container, options);

                this.TotalPages = instance.PageCount;

                this.StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            this.proxy = new BlazorPdfProxy(this);
            this.objRef ??= DotNetObjectReference.Create(this.proxy);

            base.OnInitialized();
        }

        protected async Task GoToPageAsync(string pageNumber)
        {
            if (int.TryParse(pageNumber, out int page))
            {
                await this.GoToPageAsync(page);
            }
        }

        protected async Task GoToPageAsync(int pageNumber)
        {
            var oldPage = this.Page;

            if (pageNumber < 1 || pageNumber > this.TotalPages)
            {
                this.Page = pageNumber < 1 ? 1 : this.TotalPages;
            }
            else
            {
                this.Page = pageNumber;
            }

            if (oldPage != this.Page)
            {
                await this.PdfViewerJsInterop.GoToPageAsync(this.Id, this.Page);
            }
        }

        protected async Task StartSignaturePositionAsync(SignatureLocation signatureLocation)
        {
            Console.WriteLine("Start signature position");

            signatureLocation.Page = this.Page;
            _currentAction = PdfViewerAction.StartSignaturePosition;

            var signatureIndex = this.SignatureLocations!.ToList().IndexOf(signatureLocation);

            await this.PdfViewerJsInterop.StartSignaturePositionAsync(this.Id, signatureIndex);

        }

        protected async Task NextPageAsync()
        {
            await this.GoToPageAsync(this.Page + 1);
        }

        protected async Task PreviousPageAsync()
        {
            await this.GoToPageAsync(this.Page - 1);
        }

        protected async Task ValidateAsync()
        {

            await this.OnValidated.InvokeAsync(this.SignatureLocations);

        }

        private PdfViewerAction _currentAction = PdfViewerAction.None;

        internal class BlazorPdfProxy
        {
            public BlazorPdfProxy(PdfViewer pdfViewer)
            {
                this.PdfViewer = pdfViewer;
            }

            public PdfViewer PdfViewer { get; }

            [JSInvokable]
            public void OnPageClick(PositionDto position)
            {
                Console.WriteLine($"Page {this.PdfViewer.Page} clicked at {position.X}, {position.Y}");
            }

            [JSInvokable]
            public void OnSignaturePosition(SignaturePositionDto position)
            {
                Console.WriteLine($"Signature position set at {position.X}, {position.Y}");
               
                var signatureLocation = this.PdfViewer.SignatureLocations!.ElementAt(position.Index);
                signatureLocation.X = position.X;
                signatureLocation.Y = position.Y;

                this.PdfViewer.StateHasChanged();
            }
        }

        private enum PdfViewerAction
        {
            None,
            StartSignaturePosition,
            Validate
        }
    }
}