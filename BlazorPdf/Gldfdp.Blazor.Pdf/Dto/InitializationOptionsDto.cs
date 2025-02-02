using Gldfdp.Blazor.Pdf.Dto;

namespace Gldfdp.Blazor.Pdf.Dto
{
    public record InitializationOptionsDto
    {
        public required string Url { get; init; }

        public IEnumerable<SignaturePositionDto>? Signatures { get; set; }
    }
}
