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

using com.drollic.app.dreamer.core;
using System.IO;
using System.ComponentModel;


namespace DreamerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker worker2 = new BackgroundWorker();
        static int number = 0;

        public MainWindow()
        {
            InitializeComponent();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.result.Source = e.Result as BitmapImage;

            startTheWorker();           
        }


        void startTheWorker()
        {
            worker.RunWorkerAsync(this.input.Text);
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string text = (string)e.Argument;
            Composition c = new LayeredComposition(1000, 1000, 10, text, Composition.UrlProvider.Flickr);
            c.Compose();

            //c.FinalResult.Save(text + " number " + number++ + ".png", System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage img = BitmapToImageSource(c.FinalResult, System.Drawing.Imaging.ImageFormat.Bmp);
            img.Freeze();

            e.Result = img;
        }
        

        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap, System.Drawing.Imaging.ImageFormat imgFormat)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, imgFormat);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            number = 0;
            worker.RunWorkerAsync(this.input.Text);            
        }
    }
}
