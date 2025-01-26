using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdf
{
    public record PdfViewerLabels
    {
        public string Validate { get; set; } = "Validate";

        public string Position { get; set; } = "Position";

        public string Page { get; set; } = "Page";

        public string Signatures { get; set; } = "Signatures";
    }
}
