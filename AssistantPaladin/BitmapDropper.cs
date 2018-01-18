using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace AssistantPaladin
{
    class BitmapDropper : IDropper
    {
        public System.Windows.Media.Color GetColor()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            Graphics graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            var color = bmp.GetPixel(bounds.Width / 2, bounds.Height / 2);
            
            var mColor = new System.Windows.Media.Color()
            {
                A = 255,
                R = color.R,
                G = color.G,
                B = color.B
            };

            return mColor;
        }
    }
}
