﻿namespace Gldfdp.Blazor.Pdf.Dto
{
    public class PdfViewerInitializedDto
    {
        public required string ElementId { get; init; }

        public required int PageCount { get; init; }
    }
}
