// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssistantPaladin
{
    /// <summary>
    /// Логика взаимодействия для OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private Rect rect;
        private const string gameName = "Paladins (64-bit, DX11)";
        private IntPtr gameHandle;

        private struct Rect
        {
            public int left, top, right, bottom;
        }

        public OverlayWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            var backgroundBrash = new SolidColorBrush(Colors.Wheat);
            backgroundBrash.Opacity = 0;
            this.Background = backgroundBrash;
            this.Topmost = true;
            this.IsHitTestVisible = false;
            this.ShowInTaskbar = false;
        }

        private void WatchActiveWindow()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    gameHandle = FindWindow(null, gameName);
                    var p = GetForegroundWindow();
                    if (gameHandle != GetForegroundWindow())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            this.Opacity = 0;
                        });
                    }
                    else
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            this.Opacity = 1;
                        });
                    }
                    Task.Delay(300).Wait();
                }
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, -20);
            SetWindowLong(hwnd, -20, extendedStyle | 0x00000020);

            gameHandle = FindWindow(null, gameName);

            GetWindowRect(gameHandle, out rect);

            this.Width = rect.right - rect.left;
            this.Height = rect.bottom - rect.top;

            this.Top = rect.top - 1;
            this.Left = rect.left + 1;

            WatchActiveWindow();
            WatchColorAim();
        }

        private void WatchColorAim()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    IDropper dropper = new WinApiDropper();
                    Color color = dropper.GetColor();

                    Dispatcher.InvokeAsync(() =>
                    {
                        var aimBrush = new SolidColorBrush(color);
                        this.ellipseAim.Stroke = aimBrush;
                    });
                    Task.Delay(25).Wait();
                }
            });
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}