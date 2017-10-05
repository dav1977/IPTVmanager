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
    partial class Player
    {
        /// <summary>
        /// initVLClib
        /// </summary>
        void initVLClib()
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
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка библиотеки vlc " + ex.Message); }
        }

        

        /// <summary>
        /// PLAY VLC
        /// </summary>
        /// <param name="link"></param>
        public void PlayVLC(string link)
        {

            try
            {
                if (m_player == null)
                {
                    initVLClib();
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
                taskPLAY = Task.Factory.StartNew(() =>
                {
                    m_player.Play();
                });
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Ошибка vlc " + ex.Message); }

        }



    }//
}//
