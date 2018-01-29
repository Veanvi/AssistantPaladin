// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YandexTranslateCSharpSdk;
using AutoIt;
using System.Net;
using System.IO;
using System.Threading;
using System.Media;

namespace AssistantPaladin
{
    /// <summary>
    /// Логика взаимодействия для TranslateWIndow.xaml
    /// </summary>
    public partial class TranslateWIndow : Window
    {

        public TranslateWIndow()
        {
            System.Drawing.Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            InitializeComponent();

            this.Top = bounds.Height - 100 ;   
            this.Left = 10;

            textBlok.KeyDown += (s, e) => OnKeyDownHandler(s, e);
            SystemSounds.Beep.Play();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnTranslate_Click(new object(), new RoutedEventArgs());
            }
            if (e.Key == (Key)96 || e.Key == (Key)1105)
            {
                this.textBlok.Text = string.Empty;
            }
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            AutoItX.WinActivate("Paladins");
            Task.Run(() =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    string translateText = Translate(textBlok.Text);
                    AutoItX.Send(translateText);
                    this.Close();
                });
            });
        }

        private string Translate(string text)
        {
            if (text == "") 
                return "";
            WebRequest request = WebRequest.Create($"https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20171225T185552Z.53826707a8038f06.fcda3b5bc5af96d31c928afd773bbb0af0a159c5&text={text}&lang=ru-en&[format=plain]&[options=1]");
            WebResponse response = request.GetResponse();
            string result = string.Empty;


            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
            var str = result.Split(new char[2] { '[', ']' });
            result = str[1];
            result = result.Substring(1, result.Length - 2);
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoItX.WinActivate(this.Title);
            this.textBlok.Focus();
        }
    }
}
