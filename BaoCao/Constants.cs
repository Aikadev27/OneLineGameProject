using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaoCao
{
    static public class Constants
    {
        public static readonly Brush BoardBackgroundColor = ColorRGB(40, 75,99);


        public static readonly Brush NodeBackgroundColorSelect = ColorRGB(0,175,185);

        


        public static readonly Brush NodeBackgroundColorDefault = ColorRGB(242,100,25);


        public static Brush ColorRGB(int r, int g, int b, int alpha=255)
        {
            return new SolidColorBrush(Color.FromArgb((byte)alpha, (byte)r, (byte)g, (byte)b));            
        }
    }
}
