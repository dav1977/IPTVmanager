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

namespace IPTVman.ViewModel
{
    static class data
    {
        public static string url="";
        public static string name="";
        public static bool mode_radio = false;
        public static bool mode_scan = false;
        public static string title = "";
        public static string buff = "";

    }
    /// <summary>
    /// nVLC lib  and bass lib   Player
    /// </summary>
    public partial class Player : Window
    {
        //Application.StartupPath
        public static Window header;
        IMediaPlayerFactory m_factory;
        IVideoPlayer m_player;
        IMedia m_media;
        System.Windows.Forms.Panel p;
        public static int timer_off = 0;
        int timer_ct = 0;
        int ct_update_tags = 0;
        string media_info = "";
        string play_link;
        Task taskPLAY, taskBASS;

        public Player()
        {
            header = this;
            InitializeComponent();
            this.Activate();
            this.Focus();

            data.mode_scan = true;
            scan();
            return;

            if (data.mode_scan) {   scan(); return; }
            if (data.mode_radio)data.title = IPTVman.ViewModel.data.name;
            //data.url=  "http://newairhost.com:8034/listen.ram";
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
            }
            else
            {
                l1.Visibility = Visibility.Hidden;
                l2.Visibility = Visibility.Hidden;
            }
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);

            windowsFormsHost1.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);
            //this.Cursor = System.Windows.Input.Cursors.None;
            if (data.url != "") init();  else this.Title = "Нечего проигрывать";
            slider2.Value = 100;
            reset();

            //use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
            timer.Start();

            slider2.Focus();
        }


        void scan()
        {
            try
            {
                List<string> data = new List<string>();
                var m = new MemoryFile();
               //List<string> data = m.ReadObjectFromMMF("C:\\TEMP\\IPTVMANAGERSAVELINKS") as List<string>;

                List<string> savedata = new List<string>();



            

                data.Add("http://radio.oldxit.ru:8010/radio");
                 //data.Add("http://ic2.101.ru:8000/c7_20");

                WinPOP.init_ok = false;
                header = null;


                string playing="";

                var bass = new AudioBass();
                bass.init();

                foreach (var s in data)
                {

                    taskBASS = Task.Factory.StartNew(() =>
                    {
                        mode_init_bass = true;
                        WinPOP._bass = new AudioBass();
                        WinPOP._bass.init();
                        WinPOP._bass.create_stream(s,false, null);
                        mode_init_bass = false;
                       

                    });

                    // bass.create_stream(s, false, null);

                    //string bitr = "";
                    //string playing = bass.get_tags(s, ref bitr);


                    //bass.stop();

                    //savedata.Add(playing);

                    System.Windows.MessageBox.Show("send" + playing);
                    // savedata.Add(bitr.ToString());
                }


               // System.Windows.MessageBox.Show("size="+savedata.Count.ToString());
                m.WriteObjectToMMF("C:\\TEMP\\IPTVMANAGERSAVEPLAYERS", savedata);

            }
            catch(Exception ex) { System.Windows.MessageBox.Show("memorymaps error="+ex.Message); }


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

        void init()
        {
            try
            {
                p = new System.Windows.Forms.Panel();
                p.BackColor = System.Drawing.Color.Black;
                windowsFormsHost1.Child = p;

                m_factory = new MediaPlayerFactory(true);
                m_player = m_factory.CreatePlayer<IVideoPlayer>();

                this.DataContext = m_player;

                //m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
                //m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
                //m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
                //m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

                m_player.WindowHandle = p.Handle;
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка библиотеки vlc "+ex.Message); }
        }

        public void Play(string link)
        {
            taskPLAY = Task.Factory.StartNew(() =>
            {
                try
                {
                    if (m_player == null)
                    {
                        init();
                    }
                    if (m_player.IsPlaying) m_player.Stop();
                    if (play_link != link) m_player.Stop();
                    while (m_player.IsPlaying) Thread.Sleep(1000);

                    play_link = link;
                    m_media = m_factory.CreateMedia<IMedia>(link);

                    // m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
                    // m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);

                    m_player.Open(m_media);
                    m_media.Parse(true);
                    reset();
                    m_player.Play();
                }
                catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка vlc " + ex.Message); }
            });
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (loc) return;
            loc = true;

            if (mode_init_bass)
            {
                if (data.mode_radio) this.Title = data.title;
                //this.Title = data.name + " " + data.url;

            }
            else
            {
                ct_update_tags++;
                if (ct_update_tags > 30)
                {
                    ct_update_tags = 0;
                    //taskTAG = Task.Factory.StartNew(() =>
                    //{
                        string bitr = "?";
                        string s1 = WinPOP._bass.get_tags(data.url, ref bitr);
                        string s2 = "";

                        if (bitr == "?" || bitr == "0") s2 = data.name;
                        else s2 = data.name + "   [" + bitr + " кбит/с ]";

                        l1.Dispatcher.Invoke(new Action(() =>
                        {
                            l1.Content = s2;
                        }));

                        l2.Dispatcher.Invoke(new Action(() =>
                        {
                            l2.Content = s1;
                        }));
                    //}
                }
            }

            if (timer_off != 0)
            {
                timer_ct++;
                if (timer_ct > timer_off) this.Close();
                //18000 - 30min
            }

            dialog.manager();

            if (ct<10)ct++;
            if (ct > 8 && ct<11)
            {
                ct = 255;
                try
                {
                    if (!data.mode_radio) if (data.url != "") { init(); Play(data.url); }
                }
                catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка библиотеки vlc " + ex.Message); }

                try
                {
                    if (!WinPOP.init_ok)
                    {

                        taskBASS = Task.Factory.StartNew(() =>
                        {
                            mode_init_bass = true;
                            WinPOP._bass = new AudioBass();
                            WinPOP._bass.init();
                            WinPOP._bass.create_stream(data.url, data.mode_radio, this);
                            mode_init_bass = false;
                            if (data.mode_radio)
                            { 
                                WinPOP._bass.play();
                            }

                        });
                    }

                }
                catch (Exception ex) { System.Windows.MessageBox.Show("Ошибка библиотеки bass " +ex.ToString()); }
          
            }

            if (data.mode_radio || m_player == null) goto exit;

            try
            {
                if (!updatename)
                {
                    
                    if (m_player.IsPlaying)
                    {
                        this.Title =  data.name +" "+ data.url;
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

            
            if (data.mode_radio && WinPOP._bass != null)
            {
                if (mute)  mute = false;  else  mute = true; 
                WinPOP._bass.mute(mute, (float)slider2.Value);
            }
        }


        void set_volume(int v)
        {
            if(!data.mode_radio && m_player != null) m_player.Volume = v;

            if (data.mode_radio && WinPOP._bass != null)
            {
                WinPOP._bass.volume(v);
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
            if (data.mode_radio && WinPOP._bass != null) WinPOP._bass.stop();
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

        private void win_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }
    }
}
