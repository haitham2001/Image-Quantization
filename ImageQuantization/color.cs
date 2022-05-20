using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class color
    {
        public RGBPixel RGB_color;
        public int int_color;
        public color(RGBPixel RGB_color, int int_color)
        {
            this.RGB_color = RGB_color;
            this.int_color = int_color;
        }
    }
}
