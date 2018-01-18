using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace AssistantPaladin
{
    class WinApiDropper : IDropper
    {
        public System.Windows.Media.Color GetColor()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            IntPtr hDC = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hDC, bounds.Width / 2, bounds.Height / 2);
            ReleaseDC(IntPtr.Zero, hDC);

            byte r = (byte)(pixel & 0x000000FF);
            byte g = (byte)((pixel & 0x0000FF00) >> 8);
            byte b = (byte)((pixel & 0x00FF0000) >> 16);


            var mColor = new System.Windows.Media.Color()
            {
                A = 255,
                R = r,
                G = g,
                B = b
            };
            return mColor;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hDC, int x, int y);

    }
}
