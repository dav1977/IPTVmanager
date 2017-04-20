using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Threading;
//using Vlc.DotNet.Wpf;
//using Vlc.DotNet.Forms;
//using Vlc.DotNet.Core;
using System.Windows.Threading;


namespace Vlc.DotNet
{
    /// <summary>
    /// </summary>
    public partial class Player : Window
    {
        public Player()
        {
            InitializeComponent();
       
            myControl.MediaPlayer.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
            myControl.MediaPlayer.EndInit();
            
           

            string url = IPTVman.Model.data.URLPLAY;
            try
            {
                myControl.MediaPlayer.Play(new Uri(url));
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "error");

            }


            //use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        bool lok=false;
        private void timer_Tick(object sender, EventArgs e)
        {
            if (lok) return;
            lok = true;
            if (IPTVman.Model.data.playerUPDATE)
            {
                key1.Content = "not playing";
                IPTVman.Model.data.playerUPDATE = false;
                if (myControl.MediaPlayer.IsPlaying) myControl.MediaPlayer.Stop();

                Thread.Sleep(500);
                string url = IPTVman.Model.data.URLPLAY;
                try
                {
                    myControl.MediaPlayer.Play(new Uri(url));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "error");

                }
            }

            key1.Content = "STOP";
            lok = false;
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;


            if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
                e.VlcLibDirectory = new DirectoryInfo(Path.GetDirectoryName("c:/VLC/"));
            else
                e.VlcLibDirectory = new DirectoryInfo(Path.GetDirectoryName("c:/VLC/"));
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {


            myControl.MediaPlayer.Stop();

            // myControl.MediaPlayer.Play(new Uri("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi"));
            //myControl.MediaPlayer.Play(new FileInfo(@"..\..\..\Vlc.DotNet\Samples\Videos\BBB trailer.mov"));
        }

        private void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            myControl.MediaPlayer.Rate = 2;
        }

        private void GetLength_Click(object sender, RoutedEventArgs e)
        {
            GetLength.Content = myControl.MediaPlayer.Length + " ms";
        }

        private void GetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentTime.Content = myControl.MediaPlayer.Time + " ms";
        }

        private void SetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            myControl.MediaPlayer.Time = 5000;
            SetCurrentTime.Content = myControl.MediaPlayer.Time + " ms";
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (myControl.MediaPlayer != null)
            {
                myControl.MediaPlayer.Stop();
                myControl.MediaPlayer.Dispose();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (myControl.MediaPlayer != null)
            {
                myControl.MediaPlayer.Stop();
                myControl.MediaPlayer.Dispose();
            }
        }
    }
}
