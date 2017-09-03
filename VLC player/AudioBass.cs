using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Wma;
using Un4seen.Bass.AddOn.Tags;

namespace IPTVman.ViewModel
{
    // NOTE: Needs 'bass.dll' - copy it to your output directory first!
 //       needs 'basswma.dll' - copy it to your output directory first!
    public class AudioBass
    {
        // PINNED
        private string _myUserAgent = "RADIO42";
        [FixedAddressValueType()]
        public IntPtr _myUserAgentPtr;
        string tags;
        private int _Stream = 0;
        private string _url = String.Empty;
        private DOWNLOADPROC myStreamCreateURL;
        private TAG_INFO _tagInfo;
        private SYNCPROC mySync;
        private RECORDPROC myRecProc;
        private int _wmaPlugIn = 0;
        bool isWMA = false;

        public void init()
		{
            if (WinPOP.init_ok) return;
            _myUserAgentPtr = Marshal.StringToHGlobalAnsi(_myUserAgent);

            //BassNet.Registration("your email", "your regkey");

            // check the version..
            if (Utils.HighWord(Bass.BASS_GetVersion()) != Bass.BASSVERSION)
            {
                MessageBox.Show( "Wrong Bass Version!");
            }

            // stupid thing here as well, just to demo...
            //string userAgent = Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT);

            Bass.BASS_SetConfigPtr(BASSConfig.BASS_CONFIG_NET_AGENT, _myUserAgentPtr);
			
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0); // so that we can display the buffering%
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);

			if ( Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero) )
			{
				// Some words about loading add-ons:
				// In order to set an add-on option with BASS_SetConfig, we need to make sure, that the
				// library (in this case basswma.dll) is actually loaded!
				// However, an external library is dynamically loaded in .NET with the first call 
				// to one of it's methods...
				// As BASS will only know about additional config options once the lib has been loaded,
				// we need to make sure, that the lib is loaded before we make the following call.
				// 1) Loading a lib manually :
				 BassWma.LoadMe();  // basswma.dll must be in same directory
				// 2) Using the BASS PlugIn system (recommended):
				//_wmaPlugIn = Bass.BASS_PluginLoad( "basswma.dll" );
				// 3) ALTERNATIVLY you might call any 'dummy' method to load the lib!
				//int[] cbrs = BassWma.BASS_WMA_EncodeGetRates(44100, 2, BASSWMAEncode.BASS_WMA_ENCODE_RATES_CBR);
				// now basswma.dll is loaded and the additional config options are available...

				if ( Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_WMA_PREBUF, 0) == false)
				{
					MessageBox.Show( "ERROR: " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()) );
				}
				// we alraedy create the user callback methods...
				myStreamCreateURL = new DOWNLOADPROC(MyDownloadProc);
                WinPOP.init_ok = true;


            }
			else
				MessageBox.Show( "Bass_Init error!" );
		}

        void init_device()
        {
            // init the two output devices
            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            Bass.BASS_Init(2, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
          
            // set the device context to the first device
            Bass.BASS_SetDevice(1);

            // create a first stream in this context
            //int stream1 = Bass.BASS_StreamCreateFile("test1.mp3", 0L, 0L, BASSFlag.BASS_DEFAULT);
            //Bass.BASS_ChannelPlay(stream1, false);

            // set the device context to the second device
            Bass.BASS_SetDevice(2);
            // create a second stream using this context
            //int stream2 = Bass.BASS_StreamCreateFile("test2.mp3", 0L, 0L, BASSFlag.BASS_DEFAULT);
            //Bass.BASS_ChannelPlay(stream2, false);
           
            // if you want to change the output of the first stream to the second output
            // you might call this (even during playback)
            //Bass.BASS_ChannelSetDevice(stream1, 2);

        }


        public void create_stream(string nm, bool moderad, Window header)
        {
            _Stream = 0;
            Bass.BASS_StreamFree(_Stream);
            _url = nm;

            // create the stream
            _Stream = Bass.BASS_StreamCreateURL(_url, 0, BASSFlag.BASS_STREAM_STATUS, myStreamCreateURL, IntPtr.Zero);
            if (_Stream == 0)
            {
                // try WMA streams...
                _Stream = BassWma.BASS_WMA_StreamCreateFile(_url, 0, 0, BASSFlag.BASS_DEFAULT);
                if (_Stream != 0) isWMA = true;
                else
                {
                    if (moderad) dialog.Show("Поток не поддерживается", header);
                    return;
                }
            }

            init_tag();

            if (header == null) return;

            // ok, do some pre-buffering...
            data.buff = "Buffering ...";
            if (!isWMA)
            {
                // display buffering for MP3, OGG...
                while (true)
                {
                    long len = Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_END);
                    if (len == -1)
                        break; // typical for WMA streams
                               // percentage of buffer filled
                    float progress = (
                        Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD) -
                        Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_CURRENT)
                        ) * 100f / len;

                    if (progress > 75f)
                    {
                        break; // over 75% full, enough
                    }

                    data.buff = String.Format("Buffering ... {0}%", progress);
                }
            }
            else
            {
                // display buffering for WMA...
                while (true)
                {
                    long len = Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_WMA_BUFFER);
                    if (len == -1L)
                        break;
                    // percentage of buffer filled
                    if (len > 75L)
                    {
                        break; // over 75% full, enough
                    }
                    data.buff = String.Format("Buffering... {0}%", len);
                }
            }



        }


        void init_tag()
        {
            try
            {
                _tagInfo = new TAG_INFO(_url);
                BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(_Stream);
                if (info.ctype == BASSChannelType.BASS_CTYPE_STREAM_WMA) isWMA = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
      
        public string get_tags(string nm, ref string bitr)
        {
            if (_Stream == 0) return "";
            try
            {
                
                init_tag();

                // get the meta tags (manually - will not work for WMA streams here)
                //string[] icy = Bass.BASS_ChannelGetTagsICY(_Stream);
                //if (icy == null)
                //{
                //    // try http...
                //    icy = Bass.BASS_ChannelGetTagsHTTP(_Stream);
                //}
                //if (icy != null)
                //{
                //    foreach (string tag in icy)
                //    {
                //        tags += "ICY: " + tag + Environment.NewLine;
                //    }
                //}
                //// get the initial meta data (streamed title...)
                //icy = Bass.BASS_ChannelGetTagsMETA(_Stream);
                //if (icy != null)
                //{
                //    foreach (string tag in icy)
                //    {
                //        tags += "Meta: " + tag + Environment.NewLine;
                //    }
                //}
                //else
                //{
                //    // an ogg stream meta can be obtained here
                //    icy = Bass.BASS_ChannelGetTagsOGG(_Stream);
                //    if (icy != null)
                //    {
                //        foreach (string tag in icy)
                //        {
                //            tags += "Meta: " + tag + Environment.NewLine;
                //        }
                //    }
                //}

                if (BassTags.BASS_TAG_GetFromURL(_Stream, _tagInfo))
                {
                    bitr= _tagInfo.bitrate.ToString();
                    return (_tagInfo.artist + " - " + _tagInfo.title);
                }

                
                    // alternatively to the above, you might use the TAG_INFO (see BassTags add-on)
                    // This will also work for WMA streams here ;-)
                    //if (BassTags.BASS_TAG_GetFromURL(_Stream, _tagInfo))
                    //{
                    //    // and display what we get
                    //    //this.textBoxAlbum.Text = _tagInfo.album;
                    //    //this.textBoxArtist.Text = _tagInfo.artist;
                    //    //this.textBoxTitle.Text = _tagInfo.title;
                    //    //this.textBoxComment.Text = _tagInfo.comment;
                    //    //this.textBoxGenre.Text = _tagInfo.genre;
                    //    //this.textBoxYear.Text = _tagInfo.year;
                    //}

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            return "null";
        }

        public void play()
        {
            // set a sync to get the title updates out of the meta data...
            mySync = new SYNCPROC(MetaSync);
            Bass.BASS_ChannelSetSync(_Stream, BASSSync.BASS_SYNC_META, 0, mySync, IntPtr.Zero);
            Bass.BASS_ChannelSetSync(_Stream, BASSSync.BASS_SYNC_WMA_CHANGE, 0, mySync, IntPtr.Zero);

            //// start recording...
            //int rechandle = 0;
            //if (Bass.BASS_RecordInit(-1))
            //{
            //    _byteswritten = 0;
            //    myRecProc = new RECORDPROC(MyRecoring);
            //    rechandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, myRecProc, IntPtr.Zero);
            //}
            ////this.statusBar1.Text = "Playling...";

            // play the stream

            try
            {
                Bass.BASS_ChannelPlay(_Stream, false);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            // record the stream
            //Bass.BASS_ChannelPlay(rechandle, false);
        }

    

        public void mute(bool m, float value)
        {
            if (_Stream == 0) return;
            if (m) volume(0);
            else
            Bass.BASS_ChannelSetAttribute(_Stream, BASSAttribute.BASS_ATTRIB_VOL, value/100);

            // Bass.BASS_ChannelUpdate(_Stream, 1000);
            //Thread.Sleep(500);

        }

        public void volume(float v)
        {
            if (_Stream == 0) return;
            Bass.BASS_ChannelSetAttribute(_Stream, BASSAttribute.BASS_ATTRIB_VOL, v/100);
        }


        public void stop()
        {

            Bass.BASS_PluginFree(_wmaPlugIn);
            // close bass
            Bass.BASS_Stop();
            Bass.BASS_Free();

            Bass.BASS_PluginFree(_wmaPlugIn);
        }


        private int _byteswritten = 0;
        private byte[] _recbuffer = new byte[1048510]; // 1MB buffer
        private bool MyRecoring(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // just a dummy here...nothing is really written to disk...
            if (length > 0 && buffer != IntPtr.Zero)
            {
                // copy from managed to unmanaged memory
                // it is clever to NOT alloc the byte[] everytime here, since ALL callbacks should be really fast!
                // and if you would do a 'new byte[]' every time here...the GarbageCollector would never really clean up that memory here
                // even other sideeffects might occure, due to the fact, that BASS micht call this callback too fast and too often...
                Marshal.Copy(buffer, _recbuffer, 0, length);
                // write to file
                // NOT implemented her...;-)
                _byteswritten += length;
                Console.WriteLine("Bytes written = {0}", _byteswritten);
                if (_byteswritten < 800000)
                    return true; // continue recording
                else
                    return false;
            }
            return true;
        }

        private void MyDownloadProc(IntPtr buffer, int length, IntPtr user)
        {
            if (buffer != IntPtr.Zero && length == 0)
            {
                // the buffer contains HTTP or ICY tags.
                //string txt = Marshal.PtrToStringAnsi(buffer);
                //this.Invoke(new UpdateMessageDelegate(UpdateMessageDisplay), new object[] { txt });
                // you might instead also use "this.BeginInvoke(...)", which would call the delegate asynchron!
            }
        }

        private void MetaSync(int handle, int channel, int data, IntPtr user)
        {
            try
            {
                // BASS_SYNC_META is triggered on meta changes of SHOUTcast streams
                if (_tagInfo.UpdateFromMETA(Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META), false, true))
                {
                    // this.Invoke(new UpdateTagDelegate(UpdateTagDisplay));
                    // new UpdateTagDelegate(UpdateTagDisplay);
                }
            }
            catch (Exception ex)
            { }// { MessageBox.Show(ex.ToString()); }
        }

        public delegate void UpdateTagDelegate();
        private void UpdateTagDisplay()
        {
            //this.textBoxAlbum.Text = _tagInfo.album;
            //this.textBoxArtist.Text = _tagInfo.artist;
            //this.textBoxTitle.Text = _tagInfo.title;
            //this.textBoxComment.Text = _tagInfo.comment;
            //this.textBoxGenre.Text = _tagInfo.genre;
            //this.textBoxYear.Text = _tagInfo.year;
        }

        public delegate void UpdateStatusDelegate(string txt);
        private void UpdateStatusDisplay(string txt)
        {
            //this.statusBar1.Text = txt;
        }

        public delegate void UpdateMessageDelegate(string txt);
        private void UpdateMessageDisplay(string txt)
        {
            //this.textBox1.Text += "Tags: " + txt + Environment.NewLine;
        }





    }
}
