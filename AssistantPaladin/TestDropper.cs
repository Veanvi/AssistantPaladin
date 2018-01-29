// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssistantPaladin
{
    class TestDropper: IDropper
    {
        public System.Windows.Media.Color GetColor()
        {
            var mColor = new System.Windows.Media.Color()
            {
                A = 255,
                R = 42,
                G = 42,
                B = 42
            };
            Thread.Sleep(1);
            return mColor;
        }
    }
}
