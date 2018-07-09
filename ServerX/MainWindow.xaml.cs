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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerCore;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace ServerX
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<byte[]> avatars { get; set; }
        private Server server { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            server = new Server();
            this.LoadConfigs();
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetValues();
        }
        private void SetValues()
        {
            roomName.Text = Settings.Get<String>("name");
            roomBot.Text = Settings.Get<String>("bot");
            port.Text = Settings.Get<ushort>("port").ToString();
            ushort lang_ = Settings.Get<ushort>("lenguage");
            if (lang_ == 23)
                lang.Text = "Spanish";
            else
                lang.Text = "English";
            autologins_checkbox.IsChecked = Settings.Get<bool>("local");
        }
        private void btn1_click(object sender, RoutedEventArgs e)
        {
            Settings.Set("name", roomName.Text);
            Settings.Set("bot", roomBot.Text);
            Settings.Set("port", uint.Parse(port.Text));

            if (lang.Text == "Spanish")
                Settings.Set("lenguage", 23);
            else
                Settings.Set("lenguage", 10);

            Settings.Set("local", this.autologins_checkbox.IsChecked);
            
            if (!this.server.Running)
            {
                this.server.Start();
                this.btn1.Content = "Detener Servidor";
                DisableFormulary(true);
            }else if (this.server.Running)
            {
                this.server.Stop();
                this.server = new Server();
                this.btn1.Content = "Iniciar Servidor";
                DisableFormulary(false);
            }
        }
        private void DisableFormulary(bool enable)
        {
            if (enable)
            {
                this.roomBot.IsEnabled = false;
                this.roomName.IsEnabled = false;
                this.port.IsEnabled = false;
                this.lang.IsEnabled = false;
            }
            else
            {
                this.roomBot.IsEnabled = true;
                this.roomName.IsEnabled = true;
                this.port.IsEnabled = true;
                this.lang.IsEnabled = true;
            }
        }

        private void OnServerData(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\ServerX\\");
        }

        private void OnCb0t(object sender, RoutedEventArgs e)
        {
            Process.Start("cb0t://chatroom:127.0.0.1:" + Settings.Get<ushort>("port").ToString() + "|" + Settings.Get<String>("name").ToString());
        }

        private void OnAres(object sender, RoutedEventArgs e)
        {
            Process.Start("arlnk://chatroom:127.0.0.1:" + Settings.Get<ushort>("port").ToString() + "|" + Settings.Get<String>("name").ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fd.Filter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            fd.Multiselect = false;
            fd.FileName = String.Empty;

            if ((bool)fd.ShowDialog())
            {
                RenderTargetBitmap bm = this.FileToSizedImageSource(fd.FileName, 90, 90);
                this.botAvatar_img.Source = bm;

                byte[] data = this.BitmapSourceToArray(bm);
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ServerX\\Data";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllBytes(path + "\\bot", data);

                if (server.Running)
                    Avatars.UpdateServerAvatar(data);
            }
        }
        private RenderTargetBitmap FileToSizedImageSource(String file, int width, int height)
        {
            byte[] data = File.ReadAllBytes(file);
            return this.FileToSizedImageSource(data, width, height);
        }

        private RenderTargetBitmap FileToSizedImageSource(byte[] data, int width, int height)
        {
            RenderTargetBitmap resizedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);

            using (MemoryStream ms = new MemoryStream(data))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.EndInit();
                Rect rect = new Rect(0, 0, width, height);
                DrawingVisual drawingVisual = new DrawingVisual();

                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    drawingContext.DrawImage(img, rect);

                resizedImage.Render(drawingVisual);
            }

            return resizedImage;
        }

        private byte[] BitmapSourceToArray(BitmapSource img)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(ms);
                result = ms.ToArray();
            }

            return result;
        }
        private void LoadConfigs()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ServerX";
            if (File.Exists(path + "\\Data\\bot"))
            {
                byte[] img = File.ReadAllBytes(path + "\\Data\\bot");
                this.botAvatar_img.Source = this.FileToSizedImageSource(img, 90, 90);
                Avatars.UpdateServerAvatar(img);
            }
            if (File.Exists(path + "\\Data\\default"))
            {
                byte[] img = File.ReadAllBytes(path + "\\Data\\default");
                this.botAvatar_img_Copy.Source = this.FileToSizedImageSource(img, 90, 90);
                Avatars.UpdateDefaultAvatar(img);
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fd.Filter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            fd.Multiselect = false;
            fd.FileName = String.Empty;

            if ((bool)fd.ShowDialog())
            {
                RenderTargetBitmap bm = this.FileToSizedImageSource(fd.FileName, 90, 90);
                this.botAvatar_img_Copy.Source = bm;
                byte[] data = this.BitmapSourceToArray(bm);
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ServerX\\Data";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllBytes(path + "\\default", data);
                if (server.Running)
                    Avatars.UpdateDefaultAvatar(data);
            }

        }

        private void onClosing(object sender, EventArgs e)
        {
            this.server.Stop();
        }
        
    }
}
