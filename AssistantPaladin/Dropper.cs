﻿using AutoIt;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace AssistantPaladin
{
    internal class Dropper : IDropper
    {
        public System.Windows.Media.Color GetColor()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            var color = Color.FromArgb(AutoItX.PixelGetColor(bounds.Width / 2, bounds.Height / 2));
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
