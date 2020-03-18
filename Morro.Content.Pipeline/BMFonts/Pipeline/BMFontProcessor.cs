﻿using Microsoft.Xna.Framework.Content.Pipeline;
using Morro.Content.Pipeline.BMFonts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morro.Content.Pipeline.BMFonts.Pipeline
{
    [ContentProcessor(DisplayName = "BMFont Processor - Morro")]
    class BMFontProcessor : ContentProcessor<BMFont, ProcessedBMFont>
    {
        public override ProcessedBMFont Process(BMFont input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing BMFont");

            return new ProcessedBMFont(input);
        }
    }
}
