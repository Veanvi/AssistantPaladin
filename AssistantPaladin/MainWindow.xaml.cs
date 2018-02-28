// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Open.WinKeyboardHook;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using YandexTranslateCSharpSdk;
using AutoIt;
using UpdateCheckerGoogleDrive;
using System.IO;

namespace AssistantPaladin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AutoShot autoSHot;
        private IKeyboardInterceptor _interceptor;
        private bool isActiveOverlay = true;
        private KayKlicker kayKlicker;
        private bool keyLock = false;
        private bool loopKeyClick = false;
        private OverlayWindow overlay;
        private bool translatorActive = false;
        private TranslateWIndow tranWindow;
        private BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            autoSHot = new AutoShot();
            kayKlicker = new KayKlicker();
            DataContext = autoSHot;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (s, e) => autoSHot.Work();

            _interceptor = new KeyboardInterceptor();
            _interceptor.KeyPress += new EventHandler<KeyPressEventArgs>(_interceptor_KeyPress);
            _interceptor.StartCapturing();
        }

        private void _interceptor_KeyPress(object sender, KeyPressEventArgs e)
        {
            var soundPlayer = new SoundPlayer();

            if (e.KeyChar == '5')
            {
                keyLock = !keyLock;
                soundPlayer.Stream = Properties.Resources.lockSong;
                soundPlayer.Play();
            }

            if (!keyLock)
            {
                if (e.KeyChar == 9 && translatorActive) //Tab
                {
                    TranslateChat();
                }

                if (e.KeyChar == '1')
                    autoSHot.AddAimColor();
                if (e.KeyChar == '2')
                    autoSHot.AddAllyColorAsync();
                if (e.KeyChar == '3')
                    autoSHot.AddAnemyColorAsync();
                if (e.KeyChar == '4')
                {
                    ButtonStartWork_Click(new object(), new RoutedEventArgs());
                    SystemSounds.Beep.Play();
                }
                if (e.KeyChar == '6')
                {
                    ClearAllBinds();
                }
                repaintColor();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonStartWork_Click(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy)
            {
                autoSHot.isWork = false;
                worker.CancelAsync();
                ButtonStartWork.Content = "Включить";
                ButtonStartWork.Background = new SolidColorBrush(Colors.Green);

                overlay?.Close();

                kayKlicker.charList.Clear();
                autoSHot.AddActionsList.Remove(kayKlicker.KlickKays);
                this.tbKayList.IsEnabled = true;
                this.chKayKlicker.IsEnabled = true;
            }
            else
            {
                autoSHot.isWork = true;
                worker.RunWorkerAsync();
                ButtonStartWork.Content = "Выключить";
                ButtonStartWork.Background = new SolidColorBrush(Colors.Red);

                if (isActiveOverlay)
                {
                    overlay = new OverlayWindow();
                    overlay.Owner = this;
                    overlay.Show();
                }

                this.tbKayList.IsEnabled = false;
                this.chKayKlicker.IsEnabled = false;
                if (loopKeyClick)
                {
                    foreach (var ch in tbKayList.Text)
                    {
                        if (ch != ',' && ch != ' ')
                        {
                            kayKlicker.charList.Add(ch);
                        }
                    }
                    autoSHot.AddActionsList.Add(kayKlicker.KlickKays);
                }
            }
        }

        private void chBoxLKM_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chBoxLKM.IsChecked)
                autoSHot.isLeftButtonOn = true;
            else
                autoSHot.isLeftButtonOn = false;
        }

        private void chBoxOverlay_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chBoxOverlay.IsChecked)
                this.isActiveOverlay = true;
            else
                this.isActiveOverlay = false;
        }

        private void chBoxTranslater_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chBoxTranslater.IsChecked)
                translatorActive = true;
            else
                translatorActive = false;
        }

        private void chKayKlicker_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chKayKlicker.IsChecked)
                this.loopKeyClick = true;
            else
                this.loopKeyClick = false;
        }

        private void ClearAllBinds()
        {
            autoSHot.AimColor = Colors.White;
            autoSHot.AllyColorList.Clear();
            autoSHot.AnemyColorList.Clear();
            repaintColor();
        }

        private void DisableRightButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.DisableRightButton.IsChecked)
                autoSHot.isRightButtonOn = true;
            else
                autoSHot.isRightButtonOn = false;
        }

        private void EnableBurstShutingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.EnableBurstShutingCheckBox.IsChecked)
                autoSHot.isShootBurst = true;
            else
                autoSHot.isShootBurst = false;
        }

        private void LabelTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void repaintColor()
        {
            aimColor.Fill = new SolidColorBrush(autoSHot.AimColor);

            var anemyColorsBrush = new LinearGradientBrush();
            var allyColorBrush = new LinearGradientBrush();
            anemyColorsBrush.StartPoint = new Point(0, 0);
            anemyColorsBrush.EndPoint = new Point(1, 0);

            double colorStep = 0.0;

            foreach (Color color in autoSHot.AnemyColorList)
            {
                anemyColorsBrush.GradientStops.Add(new GradientStop(color, colorStep));
                colorStep += (double)1 / (double)autoSHot.AnemyColorList.Count;
            }

            anemyColors.Fill = anemyColorsBrush;

            foreach (Color color in autoSHot.AllyColorList)
            {
                allyColorBrush.GradientStops.Add(new GradientStop(color, colorStep));
                colorStep += (double)1 / (double)autoSHot.AllyColorList.Count;
            }

            allyColors.Fill = allyColorBrush;
        }

        private void TranslateChat()
        {
            if (tranWindow == null)
                tranWindow = new TranslateWIndow();
            else if (!tranWindow.IsVisible)
            {
                tranWindow.Close();
                tranWindow = null;
                tranWindow = new TranslateWIndow();
            }
            tranWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            overlay?.Close();
            tranWindow?.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string uri = @"https://drive.google.com/open?id=1Gv4zAnSV71NpR5C0OFbbK56YeljuTppS";
            var updateChecker = new UpdateChecker(
                uri,
                typeof(MainWindow).Assembly.GetName().Name);

            updateChecker.DownloadProgressChanged += (s, n) =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.LoadingPB.Visibility = Visibility.Visible;
                    if (LoadingPB.Value != n.ProgressPercentage)
                        this.LoadingPB.Value = n.ProgressPercentage;
                });
            };

            updateChecker.DownloadFileCompleted += (s, n) =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.LoadingPB.Visibility = Visibility.Hidden;
                    UpdatePanelTitle.Text = "Обновление загружено. \n Дождитесь завершения установки";

                    string updaterName = "Updater.exe";

                    File.WriteAllBytes(updaterName, Properties.Resources.Updater);

                    Process.Start(updaterName);
                    Process.GetCurrentProcess().Kill();
                });
            };

            Task.Run(() =>
            {
                if (typeof(MainWindow).Assembly.GetName().Version < updateChecker.Version)
                {
                    string message = "Вышла новая версия программы, необходимо обновление.";
                    string caption = "Обнаружена новая версия";

                    var result = System.Windows.MessageBox.Show(message, caption);

                    Dispatcher.Invoke(() =>
                    {
                        UpdatePanelTitle.Text = "Загрузка обновления";
                    });

                    updateChecker.DownloadUpdateAsync();
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        string updaterName = "Updater.exe";
                        if (File.Exists(updaterName))
                            File.Delete(updaterName);
                        UpdatePanel.Visibility = Visibility.Hidden;
                    });
                }
            });
        }
    }
}