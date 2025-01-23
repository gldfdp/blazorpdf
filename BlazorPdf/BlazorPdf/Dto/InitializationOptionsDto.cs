using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdf.Dto
{
    public record InitializationOptionsDto
    {
        public required string Url { get; init; }
    }
}
