﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdf.Dto
{
    public record PositionDto
    {
        public required int X { get; init; }

        public required int Y { get; init; }
    }
}
