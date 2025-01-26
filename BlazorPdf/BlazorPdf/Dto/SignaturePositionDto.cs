using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdf.Dto
{
    public class SignaturePositionDto
    {
        public int Index { get; init; }

        public int? X { get; init; }

        public int? Y { get; init; }

        public int? Page { get; init; }

        public int? Width { get; init; }

        public int? Height { get; init; }

        public required string DisplayName { get; init; }
    }
}
