using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.Windows.Input;

namespace IPTVman.ViewModel
{
    /// <summary>
    /// nVLC lib  and bass lib   Player
    /// </summary>
    public partial class Player : Window
    {
        public static Window header;
        IMediaPlayerFactory m_factory;
        IVideoPlayer m_player;
        IMedia m_media;
        Panel p;
        public static int timer_off = 0;
        int timer_ct = 0;
        int ct_update_tags = 0;
        string play_link;
        Task taskPLAY, taskBASS;
        DispatcherTimer timerDOWNLOAD = new DispatcherTimer();
        

        public Player()
        {
            header = this;
            InitializeComponent();

            if (!data.mode_scan)
            {
                this.Activate();
                this.Focus();
                datafile.ReadFromXML(data.DefaultPath);
                this.Title = data.name;
            }
            else
            {
                Result.print += Result_print;
                //System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

                    Result.bass = new AudioBass();
                    Result.bass.init();
                    this.Title = "СКАННЕР РАДИО ТРЭКОВ";
                    //this.Hide();
                //ni.Icon = new System.Drawing.Icon("blur.ico");
                //ni.Visible = true;
                //ni.DoubleClick += (sndr, args) =>
                //{
                //    this.Show();
                //    this.WindowState = WindowState.Normal;
                //};
                //this.Hide();

                if (!initWCF()) this.Close();
            }
            if (data.mode_radio) data.title = data.name;






            //**************************** ТЕСТЫ
            // 1  указать ссылки на lib
            // 2  после build  скопировать bass.dll  bass_vst.dll  bass_vts.lib
            //

            //data.mode_scan = true;
            //scan();
           // return;

            //data.url = @"https://air2.radiorecord.ru:9003/chil_320";// http://air.radiorecord.ru:8102/mdl_320";
            //data.name = "test";
            //data.mode_radio = true;

            if (data.mode_radio)
            {
                windowsFormsHost1.Visibility = Visibility.Hidden;
                l1.Dispatcher.Invoke(new Action(() =>
                {
                    l1.Content = data.name;
                }));

                l2.Dispatcher.Invoke(new Action(() =>
                {
                    l2.Content = "";
                }));


                timerDOWNLOAD.Interval = new TimeSpan(0, 0, 0, 0, 50);
                timerDOWNLOAD.Tick += timerDLD_Tick;
                //timerDOWNLOAD.Start();


            }
            else
            {
                if (!data.mode_scan) l1.Visibility = Visibility.Hidden;
                l2.Visibility = Visibility.Hidden;
            }
           
                 if (!data.mode_radio && !data.mode_scan)
                {
                 if (data.url == "") this.Title = "Нечего проигрывать";
                }

            slider2.Value = 100;
            reset();

            if (!data.mode_scan)
            {
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);
                windowsFormsHost1.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);
                //use a timer to periodically update the memory usage
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                timer.Tick += timer_Tick;
                timer.Start();

                slider2.Focus();
            }
            else
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                timer.Tick += timer_Tick;
                timer.Start();

                bnSETTING.Visibility = Visibility.Hidden;
                slider2.Visibility = Visibility.Hidden;
                bMUTE.Visibility = Visibility.Hidden;
            }
        }

        private void Result_print(string str)
        {
            l1.Dispatcher.Invoke(new Action(() =>
            {
                l1.Content = str;
            }));
        }

        void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {    
           // if (e.Key == System.Windows.Input.Key.Escape)this.Close();
            if (e.Key == System.Windows.Input.Key.T)
            {
                WindowMessage.WindowAutoClose();
            }

            if (e.Key == System.Windows.Input.Key.Space)
            {
                WinPOP.Create(data.name, 0, header);
            }
        }

       

       

        void reset()
        {
            tick = 0; i = 0;
        }

        uint tick = 0;
        static byte i = 0;
        bool updatename=false;
        int ct = 0;
        bool loc = false;
        bool mode_init_bass = false;

        public static CancellationTokenSource cts1 = new CancellationTokenSource();
        public static CancellationToken cancellationToken;
        bool loktimer = false;
        string en = "";
        string lastPRINT = "";
        bool CLOSE_PROGRAMM = false;

        private void timer_Tick(object sender, EventArgs e)
        {

            if (CLOSE_PROGRAMM)
            {
                Thread.Sleep(1000);
                this.Close();
            }
         
            if (data.startTMRmanag && !timerDOWNLOAD.IsEnabled) 
                timerDOWNLOAD.Start();
            if (!data.startTMRmanag && timerDOWNLOAD.IsEnabled) 
                timerDOWNLOAD.Stop();


            if (loktimer) return;
              if (data.mode_scan)
              {

                if (data.exit_programm)
                {
                    cts1.Cancel();
                    loktimer = true;
                    l1.Dispatcher.Invoke(new Action(() =>
                    {
                        l1.Content = "closing....";
                    }));
                    CLOSE_PROGRAMM = true;
                }
                else
                {
                    en = "";
                    if (Result.data_ok) en = "ENDscan  ";
                    if (lastPRINT != en + data.scanURL)
                    {
                        l1.Dispatcher.Invoke(new Action(() =>
                        {
                            l1.Content = en + data.scanURL;
                            lastPRINT = en + data.scanURL;
                        }));
                    }
                }
              }

                if (loc) return;
                loc = true;

                if (data.mode_scan) return;
          
                if (mode_init_bass)
                {
                    if (data.mode_radio) this.Title = data.title;
                }
                else
                {
                    ct_update_tags++;
                    if (ct_update_tags > 30)
                    {
                    //ct_update_tags = 0;

                    //string bitr = "?";
                    //string s1 = data._bass.get_tags(data.url, ref bitr);
                    //string s2 = "";

                    //if (bitr == "?" || bitr == "0") s2 = data.name;
                    //else s2 = data.name + "   [" + bitr + " кбит/с ]";

                    l1.Dispatcher.Invoke(new Action(() =>
                    {
                        l1.Content = data._bass.TitleAndArtist;
                    }));

                    //l2.Dispatcher.Invoke(new Action(() =>
                    //{
                    //    l2.Content = data._bass.Status;
                    //}));

                }
                }

                if (timer_off != 0)
                {
                    timer_ct++;
                    if (timer_ct > timer_off) this.Close();
                    //18000 - 30min
                }

                dialog.manager();

                if (ct < 10) ct++;
                if (ct > 8 && ct < 11)
                {
                    ct = 255;
                    try
                    {
                        if (!data.mode_radio) if (data.url != "") { PlayVLC(data.url); }
                    }
                    catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка библиотеки vlc " + ex.Message); }

                    try
                    {
                        if (!WinPOP.init_ok)
                        {
                                taskBASS = Task.Factory.StartNew(() =>
                                {
                                    mode_init_bass = true;
                                    data._bass = new AudioBass();
                                    data._bass.init();


                                    data._bass.create_stream(data.url, data.mode_radio, this);
                                    mode_init_bass = false;

                                    data._bass.updPlay();

                                  

                                });
                        }
                    }
                    catch (Exception ex)
                    { 
                        System.Windows.MessageBox.Show("Ошибка библиотеки bass " + ex.ToString());
                    }

                }

                if (data.mode_radio || m_player == null) goto exit;

                try
                {
                    if (!updatename)
                    {

                        if (m_player.IsPlaying)
                        {
                            this.Title = data.name + " " + data.url;
                            this.Height = 430;
                            set_volume((int)slider2.Value);
                            reset();
                            updatename = true;
                        }
                    }
                    if (!m_player.IsPlaying)
                    {
                        updatename = false;
                        if (taskPLAY.IsFaulted || taskPLAY.IsCanceled)
                        { this.Title = "STOPPED!!!"; goto exit; }
                        string s = "Opening";
                        if (i == 0)
                            this.Title = s + " ... ";
                        if (i < 5) this.Title = s + " .   " + tick.ToString();
                        if (i > 5 && i < 10) this.Title = s + " . .   " + tick.ToString();
                        if (i > 10 && i < 15) this.Title = s + " . . .   " + tick.ToString();
                        if (i > 15 && i < 20) this.Title = s + " . . . . . . .   " + tick.ToString();
                        i++;
                        if (i > 20) i = 1;
                        tick++;
                    }
                }
                catch { }
           
            exit:
            loc = false;
        }

        
        private void timerDLD_Tick(object sender, EventArgs e)
        {
            if (AudioBass._chan == 0)
            {
                //if (header != null) dialog.Show("Поток не поддерживается ", header);
                l2.Dispatcher.Invoke(new Action(() =>
                {
                    l2.Content = "Поток не поддерживается ";
                }));
                return;
            }
               

            if (data._bass != null) data._bass.TickBASSmanage();
            l2.Dispatcher.Invoke(new Action(() =>
            {
                l2.Content = data._bass.Status;
            }));
        }



            void Events_PlayerStopped(object sender, EventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    InitControls();
            //}));
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    InitControls();
            //}));
        }

        private void InitControls()
        {

        }

        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            //this.Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    //label1.Content = TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8);
            //}));
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {

        }

        void Events_StateChanged(object sender, MediaStateChange e)
        {

        }

        void Events_DurationChanged(object sender, MediaDurationChange e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //m_player.Pause();
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // m_player.Stop();
        }


        bool mute = false;
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (!data.mode_radio && m_player != null)
            {
                m_player.ToggleMute();
                if (m_player.Mute) { bMUTE.FontSize += 4; } else { bMUTE.FontSize -= 4; }
            }

            
            if (data.mode_radio && data._bass != null)
            {
                if (mute)  mute = false;  else  mute = true; 
                data._bass.mute(mute, (float)slider2.Value);
            }
        }


        void set_volume(int v)
        {
            if(!data.mode_radio && m_player != null) m_player.Volume = v;

            if (data.mode_radio && data._bass != null)
            {
                data._bass.volume(v);
            }
        }
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            set_volume((int)e.NewValue);
        }
        private void slider1_DragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        private void slider1_DragStarted(object sender, DragStartedEventArgs e)
        {
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!data.mode_radio && m_player != null) m_player.Stop();
            if (data.mode_radio && data._bass != null) data._bass.stop();
        }

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void windowsFormsHost1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void windowsFormsHost1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void windowsFormsHost1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void windowsFormsHost1_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {


        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void windowsFormsHost1_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
           
        }

        private void windowsFormsHost1_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
           
        }

        private void win_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
           
        }

        bool locksetting;
        private void bnSETTING_Click(object sender, RoutedEventArgs e)
        {
            if (!WinPOP.init_ok) return;
            if (locksetting) return;
            locksetting = true;
            Window winSETTING = new WindowSettings()
            {
                Title = "Настройка "+data.name,
                Topmost = true,
                WindowStyle = WindowStyle.SingleBorderWindow,
                Name = "setting"
            };


            winSETTING.Closing += winSETTING_Closing;
            winSETTING.Show();
        }

        private void winSETTING_Closing(object sender, CancelEventArgs e)
        {
            locksetting = false;
        }

        private void win_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }
    }
}
