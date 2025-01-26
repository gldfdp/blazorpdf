using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdf
{
    public class SignatureLocation
    {
        public int? X { get; set; }

        public int? Y { get; set; }

        public int? Page { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public Dictionary<string, object> Metadata { get; set; } = [];

        public required string DisplayName { get; set; }
    }
}
