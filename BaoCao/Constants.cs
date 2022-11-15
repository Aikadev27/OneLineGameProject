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
        public static readonly double NodeSize = 55;
        public static readonly double NodeFontSize = 16;

        public static readonly Brush BoardBackgroundColor = ColorRGB(188, 184, 177);


        public static readonly Brush NodeBackgroundColorSelect = ColorRGB(0,175,185);
        public static readonly Brush EdgeColorPlay = Brushes.Blue;



        public static readonly Brush NodeBackgroundColorDefault = Brushes.OrangeRed;
        public static readonly Brush EdgeColorDefault = Brushes.Black;
        public static readonly double EdgeThickness = 3;



        public static Brush ColorRGB(int r, int g, int b, int alpha=255)
        {
            return new SolidColorBrush(Color.FromArgb((byte)alpha, (byte)r, (byte)g, (byte)b));            
        }
    }
}
