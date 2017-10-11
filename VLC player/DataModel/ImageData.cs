using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace IPTVman.ViewModel
{
   
    public class ImageViewModel : INotifyPropertyChanged
    {

        public ImageViewModel()
        {
            ImageSource obj = getscr(AppDomain.CurrentDomain.BaseDirectory + @"setting.png");
            ImageSource = obj;
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");

                LoadImageAsync();
            }
        }

        private void LoadImageAsync()//КАЖДАЯ КАРТИНКА В СВОЕМ ПОТОКЕ ЗАГРУЖАЕТСЯ
        {
            Trace.WriteLine("load");
            IsLoading = true;

            var UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(Path, UriKind.Relative);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }).ContinueWith(x =>
            {
                ImageSource = x.Result;
                IsLoading = false;
            }, UIScheduler);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        static BitmapImage getscr(string s)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(s, UriKind.Relative);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }

   

}