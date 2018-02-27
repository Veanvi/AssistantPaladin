// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using AutoIt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssistantPaladin.Properties;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace AssistantPaladin
{
    public class AutoShot
    {
        public List<Color> AllyColorList = new List<Color>();
        public List<Color> AnemyColorList = new List<Color>();
        public List<Action> AddActionsList = new List<Action>();
        private IDropper dropper;

        public bool isShootBurst { get; set; } = true;

        public bool isRightButtonOn { get; set; } = false;
        public bool isLeftButtonOn { get; set; } = true;

        public bool isWork { get; set; } = true;

        public Color AimColor { get; set; }

        public Color AllyColor { get; set; }

        public Color AnemyColor { get; set; }

        private bool noOneInSight;
        private bool allyInSight;
        private bool enemyInSight;
        private Point enemyPoint = new Point(0, 0);

        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        public AutoShot() : this(new WinApiDropper())
        {
        }

        public AutoShot(IDropper drop)
        {
            AutoItX.AutoItSetOption("MouseCoordMode", 1);
            AutoItX.AutoItSetOption("PixelCoordMode", 2);
            dropper = drop;
            //SystemParametersInfo(0x0071, 0, 1, 0);
        }

        ~AutoShot()
        {
            //SystemParametersInfo(0x0071, 0, 10, 0);
        }

        public void Work()
        {
            Random randomTimeClick = new Random();
            Parallel.Invoke(new Action[5]
            {
                () =>
                {
                    while (isWork)
                    {
                        if(isLeftButtonOn && enemyInSight)
                        {
                            //SystemParametersInfo(0x0071, 0, 1, 0);
                            var point = AutoItX.MouseGetPos();
                            if (isShootBurst)
                            {
                                AutoItX.MouseDown("LEFT");
                            }
                            else
                            {
                                mouse_event(MouseEventFlags.LEFTDOWN, 0, 0, 0, (UIntPtr)0);
                                mouse_event(MouseEventFlags.LEFTUP, 0, 0, 0, (UIntPtr)0);

                                //AutoItX.MouseClick("LEFT");
                                Thread.Sleep(randomTimeClick.Next(50, 100));
                            }
                            //SystemParametersInfo(0x0071, 0, 10, 0);
                        }
                        else if (isLeftButtonOn && isShootBurst)
                        {
                            AutoItX.MouseUp("LEFT");
                        }
                        Task.Delay(1).Wait();
                    }
                },

                () =>
                {
                    while (isWork)
                    {
                        if (isRightButtonOn && allyInSight)
                        {
                            AutoItX.MouseClick("right");
                            Thread.Sleep(randomTimeClick.Next(160, 250));
                        }
                        Task.Delay(1).Wait();
                    }
                },
                () =>
                {
                    while (isWork)
                    {
                        if(noOneInSight)
                        {
                            foreach (var action in this.AddActionsList)
                                action.Invoke();
                        }
                        Task.Delay(1).Wait();
                    }
                },
                () =>
                {
                    while (false)
                    {
                        if (enemyInSight)
                        {
                            Point nowPoint = new Point(0,0);
                            GetCursorPos(out nowPoint);
                            if(enemyPoint.X != nowPoint.X || enemyPoint.Y != nowPoint.Y)
                            {
                                int x = enemyPoint.X - nowPoint.X;
                                int y = enemyPoint.Y - nowPoint.Y;

                                mouse_event(MouseEventFlags.MOVE, x, y, 0, (UIntPtr)0);
                            }
                        }
                        Task.Delay(1).Wait();
                    }
                },
                () =>
                {
                    while (isWork)
                    {
                        SightСolorАnalysis();
                        //Task.Delay(1).Wait();
                    }
                }
            });
        }

        private void SightСolorАnalysis()
        {
            Color nowAimColor = dropper.GetColor();
            if (ApproximateColorSearch(nowAimColor, AimColor))
            {
                //enemyPoint.SetToZero();
                noOneInSight = true;
                allyInSight = false;
                enemyInSight = false;
                return;
            }

            foreach (Color bindColor in AllyColorList)
            {
                if (ApproximateColorSearch(nowAimColor, bindColor))
                {
                    //enemyPoint.SetToZero();
                    noOneInSight = false;
                    allyInSight = true;
                    enemyInSight = false;
                    return;
                }
            }

            foreach (Color bindColor in AnemyColorList)
            {
                if (ApproximateColorSearch(nowAimColor, bindColor))
                {
                    //if (enemyPoint.X == 0 || enemyPoint.Y == 0)
                    //GetCursorPos(out enemyPoint);
                    noOneInSight = false;
                    allyInSight = false;
                    enemyInSight = true;
                    return;
                }
            }

            noOneInSight = false;
            allyInSight = false;
            enemyInSight = false;
        }

        private bool ApproximateColorSearch(Color currentColor, Color desiretColor,
            int searchSpreads = 500)
        {
            double fi = Math.Pow(currentColor.R - desiretColor.R, 2)
                + Math.Pow(currentColor.G - desiretColor.G, 2)
                + Math.Pow(currentColor.B - desiretColor.B, 2);

            bool result = fi < searchSpreads;
            return result;
        }

        public void AddAimColor()
        {
            AimColor = dropper.GetColor();
            new SoundPlayer(Resources.goodSong).Play();
        }

        public async Task AddAllyColorAsync()
        {
            SoundPlayer ticPlayer = new SoundPlayer();
            ticPlayer.Stream = (Stream)Resources.ticking_clockSong;
            Color faundColor = new Color();
            await Task.Run((Action)(() =>
            {
                for (int index = 0; index < 10; ++index)
                {
                    Color color = dropper.GetColor();
                    if (!ApproximateColorSearch(color, AimColor) &&
                        !ListContainedColorChecking(color, AnemyColorList) &&
                        !ListContainedColorChecking(color, AllyColorList))
                    {
                        faundColor = color;
                        new SoundPlayer((Stream)Resources.goodSong).Play();
                        break;
                    }
                    ticPlayer.Play();
                    Thread.Sleep(200);
                }
            }));
            if (faundColor != new Color())
                AllyColorList.Add(faundColor);
            else
                new SoundPlayer((Stream)Resources.errorSong).Play();
        }

        public async Task AddAnemyColorAsync()
        {
            SoundPlayer ticPlayer = new SoundPlayer();
            ticPlayer.Stream = (Stream)Resources.ticking_clockSong;
            Color faundColor = new Color();
            await Task.Run(() =>
            {
                for (int index = 0; index < 10; ++index)
                {
                    Color color = dropper.GetColor();
                    if (!ApproximateColorSearch(color, AimColor) &&
                        !ListContainedColorChecking(color, AnemyColorList) &&
                        !ListContainedColorChecking(color, AllyColorList))
                    {
                        faundColor = color;
                        new SoundPlayer((Stream)Resources.goodSong).Play();
                        break;
                    }
                    ticPlayer.Play();
                    Thread.Sleep(200);
                }
            });
            if (faundColor != new Color())
                AnemyColorList.Add(faundColor);
            else
                new SoundPlayer((Stream)Resources.errorSong).Play();
        }

        private bool ListContainedColorChecking(Color currentCollor, IEnumerable<Color> ColorList)
        {
            foreach (var color in ColorList)
            {
                if (ApproximateColorSearch(currentCollor, color))
                    return true;
            }

            return false;
        }

        //Изменение чувствительности мыши
        [DllImport("User32.dll")]
        private static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);

        [DllImport("user32.dll")]
        private static extern void mouse_event(MouseEventFlags dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point lpPoint);

        private struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }

            public void SetToZero()
            {
                X = 0;
                Y = 0;
            }
        }
    }
}