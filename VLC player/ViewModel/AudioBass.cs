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
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Linq;
using ManagedBass;
//using Un4seen.Bass;
//using Un4seen.Bass.AddOn.Wma;
//using Un4seen.Bass.AddOn.Tags;
//sing Un4seen.Bass.AddOn.Vst;
using System.Threading.Tasks;

//https://github.com/ManagedBass/ManagedBass.DocFX/tree/master/apidoc/Bass



/// <summary>
/// //////////Assembly: Bass.Net (in Bass.Net.dll) Version: 2.4.15
/// </summary>

namespace IPTVman.ViewModel
{

    // NOTE: Needs 'bass.dll' - copy it to your output directory first!
    //       needs 'basswma.dll' - copy it to your output directory first!
     public partial class AudioBass
    {
        // PINNED
        private string _myUserAgent = "RADIO42";
        [FixedAddressValueType()]
        public IntPtr _myUserAgentPtr;
        //string tags;
        private int _Stream = 0;
        private string _url = String.Empty;
        //private DOWNLOADPROC myStreamCreateURL;
        //private TAG_INFO _tagInfo;
        //private SYNCPROC mySync;
        //private RECORDPROC myRecProc;
        //private int _wmaPlugIn = 0;
        // bool isWMA = false;

        int currentdevice=-1;


        public void init()
		{

            if (WinPOP.init_ok) return;
            _myUserAgentPtr = Marshal.StringToHGlobalAnsi(_myUserAgent);

            //BassNet.Registration("your email", "your regkey");

            // check the version..
            try
            {
                    //if (Utils.HighWord(Bass.BASS_GetVersion()) != Bass.BASSVERSION)
                    if (!Bass.Init())
                    {
                        System.Windows.MessageBox.Show("Ошибка bass.dll");
                        while (true) Thread.Sleep(5000);
                     }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(">>  Ошибка bass.dll "+ ex.ToString());
                while (true) Thread.Sleep(5000);
            }
            // enable playlist processing
            Bass.NetPlaylist = 1;

            // minimize automatic pre-buffering, so we can do it (and display it) instead
            Bass.NetPreBuffer = 0;

            TitleAndArtist = IcyMeta = null;

            WinPOP.init_ok = true;

            //if (Bass.BASS_ErrorGetCode()!=BASSError.BASS_OK)
            //System.Windows.MessageBox.Show("ErrBassStream11!    " +
            //                Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));

            // stupid thing here as well, just to demo...
            //string userAgent = Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT);

            //         Bass.BASS_SetConfigPtr(BASSConfig.BASS_CONFIG_NET_AGENT, _myUserAgentPtr);

            //         Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0); // so that we can display the buffering%
            //         Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);

            //         if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            //             System.Windows.MessageBox.Show("ErrBassStream11!    " +
            //                             Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));

            //         if ( Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero) )
            //{
            //	// Some words about loading add-ons:
            //	// In order to set an add-on option with BASS_SetConfig, we need to make sure, that the
            //	// library (in this case basswma.dll) is actually loaded!
            //	// However, an external library is dynamically loaded in .NET with the first call 
            //	// to one of it's methods...
            //	// As BASS will only know about additional config options once the lib has been loaded,
            //	// we need to make sure, that the lib is loaded before we make the following call.
            //	// 1) Loading a lib manually :
            //	 BassWma.LoadMe();  // basswma.dll must be in same directory
            //                                 // 2) Using the BASS PlugIn system (recommended):
            //                                 //_wmaPlugIn = Bass.BASS_PluginLoad( "basswma.dll" );
            //                                 // 3) ALTERNATIVLY you might call any 'dummy' method to load the lib!
            //                                 //int[] cbrs = BassWma.BASS_WMA_EncodeGetRates(44100, 2, BASSWMAEncode.BASS_WMA_ENCODE_RATES_CBR);
            //                                 // now basswma.dll is loaded and the additional config options are available...

            //             //if (Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_WMA_PREBUF, 0) == false)
            //             //{
            //             //    System.Windows.MessageBox.Show("Ошибка Bass_Init!    " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
            //             //}
            //             // we alraedy create the user callback methods...
            //             myStreamCreateURL = new DOWNLOADPROC(MyDownloadProc);
            //             WinPOP.init_ok = true;
            //         }
            //else
            //             System.Windows.MessageBox.Show( "Bass_Init error!" );


            //         if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            //             System.Windows.MessageBox.Show("ErrBassStream13!    " +
            //                             Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
        }
       


    


        static readonly object Lock = new object();
        int _req=0; // request number/counter
        public static int _chan =0; // stream handle
        bool _directConnect;

        public bool DirectConnection
        {
            get { return _directConnect; }
            set
            {
                _directConnect = value;
                //OnPropertyChanged();
            }
        }

        string _proxy;

        public string Proxy
        {
            get { return _proxy; }
            set
            {
                _proxy = value;
               // OnPropertyChanged();
            }
        }
        string _icyMeta;

        public string IcyMeta
        {
            get { return _icyMeta; }
            set
            {
                _icyMeta = value;
              //  OnPropertyChanged();
            }
        }

        string _titleAndArtist;

        public string TitleAndArtist
        {
            get { return _titleAndArtist; }
            set
            {
                _titleAndArtist = value;
               // OnPropertyChanged();
            }
        }
        void StatusProc(IntPtr buffer, int length, IntPtr user)
        {
            if (buffer != IntPtr.Zero
                && length == 0
                && user.ToInt32() == _req) // got HTTP/ICY tags, and this is still the current request

                Status = Marshal.PtrToStringAnsi(buffer); // display status
        }

        void EndSync(int Handle, int Channel, int Data, IntPtr User) => Status = "Not Playing";

        void MetaSync(int Handle, int Channel, int Data, IntPtr User) => DoMeta();


        /// <summary>
        /// Playing info
        /// </summary>
        void DoMeta()
        {
            TitleAndArtist = "----";

            var meta = Bass.ChannelGetTags(_chan, TagType.META);

            if (meta != IntPtr.Zero)
            {
                // got Shoutcast metadata
                var data = Marshal.PtrToStringAnsi(meta);

                var i = data.IndexOf("'"); // locate the title

                if (i == -1)
                    return;

                var j = data.IndexOf("';", i); // locate the end of it

                if (j != -1)
                    TitleAndArtist = $" {data.Substring(i  +1, j - i + 1 -2)}";
            }
            else
            {
                meta = Bass.ChannelGetTags(_chan, TagType.OGG);

                if (meta == IntPtr.Zero)
                    return;

                // got Icecast/OGG tags
                foreach (var tag in Extensions.ExtractMultiStringUtf8(meta))
                {
                    string artist = null, title = null;

                    if (tag.StartsWith("artist="))
                        artist = $"Artist: {tag.Substring(7)}";

                    if (tag.StartsWith("title="))
                        title = $"Title: {tag.Substring(6)}";

                    if (title != null)
                    {
                        TitleAndArtist = artist != null ? $"{title} - {artist}" : title;
                        TitleAndArtist = StrEncod(TitleAndArtist);
                    }
                }
            }
        }

        bool stopbuff = false;
        public void TickBASSmanage()
        {
            // percentage of buffer filled
            var progress = Bass.StreamGetFilePosition(_chan, FileStreamPosition.Buffer)
                * 100 / Bass.StreamGetFilePosition(_chan, FileStreamPosition.End);

            if (progress > 75 || Bass.StreamGetFilePosition(_chan, FileStreamPosition.Connected) == 0)
            {
                // over 75% full (or end of download)
                // _timer.Stop(); // finished prebuffering, stop monitoring
                data.startTMRmanag = false;// _timer.Stop(); // stop prebuffer monitoring
                //Thread.Sleep(200);

                init_tag();

  
                Status = "Playing";
                stopbuff = true;
                // get the broadcast name and URL
                var icy = Bass.ChannelGetTags(_chan, TagType.ICY); 

                if (icy == IntPtr.Zero)
                    icy = Bass.ChannelGetTags(_chan, TagType.HTTP); // no ICY tags, try HTTP

                if (icy != IntPtr.Zero)
                {
                    foreach (var tag in Extensions.ExtractMultiStringAnsi(icy))
                    {
                        var icymeta = string.Empty;

                        if (tag.StartsWith("icy-name:"))
                            icymeta += $"ICY Name: {tag.Substring(9)}";

                        if (tag.StartsWith("icy-url:"))
                            icymeta += $"ICY Url: {tag.Substring(8)}";

                        IcyMeta = icymeta;
                    }
                }

                // get the stream title and set sync for subsequent titles
                DoMeta();

                Bass.ChannelSetSync(_chan, SyncFlags.MetadataReceived, 0, MetaSync); // Shoutcast
                Bass.ChannelSetSync(_chan, SyncFlags.OggChange, 0, MetaSync); // Icecast/OGG

                // set sync for end of stream
                Bass.ChannelSetSync(_chan, SyncFlags.End, 0, EndSync);

                //// play it!
                Bass.ChannelPlay(_chan);
            }

            else if (!stopbuff) Status = $"Buffering... {progress}%";
        }

        /// <summary>
        /// create the stream
        /// </summary>
        /// <param name="Url"></param>
        void new_stream(string Url)
        {
   
            Task.Factory.StartNew(() =>
            {
                int r = 0;

                lock (Lock) // make sure only 1 thread at a time can do the following
                    r = ++_req; // increment the request counter for this request

                data.startTMRmanag = false;// _timer.Stop(); // stop prebuffer monitoring

                Thread.Sleep(100);

                Bass.StreamFree(_chan); // close old stream

                if (currentdevice != -1) Bass.CurrentDevice = currentdevice;


                Status = "Connecting...";

                var c = Bass.CreateStream(Url, 0,
                    BassFlags.StreamDownloadBlocks | BassFlags.StreamStatus | BassFlags.AutoFree, StatusProc,
                    new IntPtr(r));



                lock (Lock)
                {
                    if (r != _req)
                    {
                        // there is a newer request, discard this stream
                        if (c != 0)
                            Bass.StreamFree(c);

                        return;
                    }

                    _chan = c; // this is now the current stream
                }


                //if (_chan < 0)
                //{
                //    // failed to open
                //    Status = "Can't play the stream";
                //   if (header!=null) dialog.Show("Поток не поддерживается ", header);
                //}
                //else
                //{
                //    data.startTMRmanag = true;// _timer.Stop(); // stop prebuffer monitoring
                //    Thread.Sleep(600);
                //}//_timer.Start(); // start prebuffer monitoring


                //Thread.Sleep(600);

            });


            int ct = 0;
            while (true)
            {
                Thread.Sleep(50);
                ct++;
                if (ct > 15) break;
                if (_chan != 0) break;


            }
            data.startTMRmanag = true;
           
        }

        public void create_stream(string nm, bool moderad, Window header)
        {
            //if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            //    System.Windows.MessageBox.Show("ErrBassStream4!    " +
            //                    Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));


            _Stream = 0;
         //  bool rez= Bass.BASS_StreamFree(_Stream);
            _url = nm;


            //if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            //    System.Windows.MessageBox.Show(rez.ToString()+ " ErrBassStream5!    " +
            //                    Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));

            Bass.NetProxy = DirectConnection ? null : Proxy;

            new_stream(nm);

            //_Stream = Bass.BASS_StreamCreateURL(_url, 0, BASSFlag.BASS_STREAM_STATUS, myStreamCreateURL, IntPtr.Zero);
            //if (_Stream == 0)
            //{


            //    System.Windows.MessageBox.Show("ErrBassStream8!    " +
            //           Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));


            //    // try WMA streams...
            //    _Stream = BassWma.BASS_WMA_StreamCreateFile(_url, 0, 0, BASSFlag.BASS_DEFAULT);
            //    if (_Stream != 0) isWMA = true;
            //    else
            //    {
            //        // if (moderad) dialog.Show("Поток не поддерживается " + nm, header);
            //        System.Windows.MessageBox.Show("ErrBassStream9!    " +
            //          Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));


            //        return;
            //    }
            //}

            //init_tag();

            //if (header == null) return;

            //// ok, do some pre-buffering...
            //data.buff = "Buffering ...";
            //if (!isWMA)
            //{
            //    // display buffering for MP3, OGG...
            //    while (true)
            //    {
            //        long len = Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_END);
            //        if (len == -1)
            //            break; // typical for WMA streams
            //                   // percentage of buffer filled
            //        float progress = (
            //            Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD) -
            //            Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_CURRENT)
            //            ) * 100f / len;

            //        if (progress > 75f)
            //        {
            //            break; // over 75% full, enough
            //        }

            //        data.buff = String.Format("Buffering ... {0}%", progress);
            //    }
            //}
            //else
            //{
            //    // display buffering for WMA...
            //    while (true)
            //    {
            //        long len = Bass.BASS_StreamGetFilePosition(_Stream, BASSStreamFilePosition.BASS_FILEPOS_WMA_BUFFER);
            //        if (len == -1L)
            //            break;
            //        // percentage of buffer filled
            //        if (len > 75L)
            //        {
            //            break; // over 75% full, enough
            //        }
            //        data.buff = String.Format("Buffering... {0}%", len);
            //    }
            //}
        }


        /// <summary>
        /// декодирование тэга
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string StrEncod(string str)
        {
            bool need_coding = false;
            //string tt = "";
            //кодировка
            var fromEncodind = Encoding.UTF8;
            var bytes = fromEncodind.GetBytes(str);

            int l = 0;
            byte[] bt = new byte[bytes.Length];

            foreach (var b in bytes)
            {
                bt[l] = 32;
                l++;
            }

            byte[] test = new byte[1];

            int k = 0;
            foreach (var b in bytes)
            {
                bt[k] = b;

                if (b > 127 && b < 200) need_coding = true;

                if (b >= 176 && b <= 193) bt[k] = (byte)(b + 48);

                if (b != 195) k++;

            }


            string ans = "";
            if (need_coding)
            {
                var toEncoding = Encoding.GetEncoding(866);//в какую кодировку
                ans = toEncoding.GetString(bt);
            }
            else ans = str;


            //Поcт анализ
            fromEncodind = Encoding.UTF8;//из какой кодировки
            bytes = fromEncodind.GetBytes(ans);
            foreach (var b in bytes)
            {
                //test[0] = b;
                //var toEncoding = Encoding.GetEncoding(866);
                //tt += toEncoding.GetString(test) + ":" + b.ToString() + "; ";
                if (b == 226) { ans = str; break; }

            }
            return ans.Trim();
        }


        void init_tag()
        {
           
            try
            {
                var info = Bass.ChannelGetInfo(_chan);

                data.infosteam = info.Resolution.ToString();

                
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
        }

        /// <summary>
        /// Name Playing
        /// </summary>
        /// <param name="nm"></param>
        /// <param name="bitr"></param>
        /// <returns></returns>
        public string scan_get_tags(string nm, ref string bitr)
        {
         
            try
            {
                Thread.Sleep(1000);
                DoMeta();
                stop();
                return TitleAndArtist;


                // init_tag();

                // if (BassTags.BASS_TAG_GetFromURL(_Stream, _tagInfo))
                // {
                //     bitr = _tagInfo.bitrate.ToString();

                //     string artist = StrEncod(_tagInfo.artist);
                //     string title = StrEncod(_tagInfo.title);
                //     title = title.Replace("- 0:00", "");
                //     string rez = artist + " - " + title;

                //     if (artist == "" || artist == " " || artist == ";") rez = title;
                //     if (title == "" || title == " " || title == ";" || title == "0:00" || title == "0") rez = artist;

                //     if (artist == "" && title == "") rez = "no artist";

                //     if (rez == " ") rez = "none";
                //     return (rez);
                // }

                // get the meta tags(manually -will not work for WMA streams here)
                //     string[] icy = Bass.BASS_ChannelGetTagsICY(_Stream);
                // if (icy == null)
                // {
                //     // try http...
                //     icy = Bass.BASS_ChannelGetTagsHTTP(_Stream);
                // }
                // if (icy != null)
                // {
                //     foreach (string tag in icy)
                //     {
                //         tags += "ICY: " + tag + Environment.NewLine;
                //     }
                // }
                // // get the initial meta data (streamed title...)
                // icy = Bass.BASS_ChannelGetTagsMETA(_Stream);
                // if (icy != null)
                // {
                //     foreach (string tag in icy)
                //     {
                //         tags += "Meta: " + tag + Environment.NewLine;
                //     }
                // }
                // else
                // {
                //     // an ogg stream meta can be obtained here
                //     icy = Bass.BASS_ChannelGetTagsOGG(_Stream);
                //     if (icy != null)
                //     {
                //         foreach (string tag in icy)
                //         {
                //             tags += "Meta: " + tag + Environment.NewLine;
                //         }
                //     }
                // }



                ////// alternatively to the above, you might use the TAG_INFO(see BassTags add-on)
                // //// This will also work for WMA streams here; -)
                //     if (BassTags.BASS_TAG_GetFromURL(_Stream, _tagInfo))
                //     {
                //         // and display what we get
                //         //this.textBoxAlbum.Text = _tagInfo.album;
                //         //this.textBoxArtist.Text = _tagInfo.artist;
                //         //this.textBoxTitle.Text = _tagInfo.title;
                //         //this.textBoxComment.Text = _tagInfo.comment;
                //         //this.textBoxGenre.Text = _tagInfo.genre;
                //         //this.textBoxYear.Text = _tagInfo.year;
                //     }

            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
            return "error stream";
        }
      

        public void playStream()
        {

            //DoMeta();

            //Bass.ChannelSetSync(_chan, SyncFlags.MetadataReceived, 0, MetaSync); // Shoutcast
            //Bass.ChannelSetSync(_chan, SyncFlags.OggChange, 0, MetaSync); // Icecast/OGG

            //// set sync for end of stream
            //Bass.ChannelSetSync(_chan, SyncFlags.End, 0, EndSync);

            //Thread.Sleep(3000);

            // play it!
           // Bass.ChannelPlay(_chan);


            // set a sync to get the title updates out of the meta data...
            //mySync = new SYNCPROC(MetaSync);
            //Bass.BASS_ChannelSetSync(_Stream, BASSSync.BASS_SYNC_META, 0, mySync, IntPtr.Zero);
            //Bass.BASS_ChannelSetSync(_Stream, BASSSync.BASS_SYNC_WMA_CHANGE, 0, mySync, IntPtr.Zero);

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

            //try
            //{
            //    Bass.BASS_ChannelPlay(_Stream, false);    
            //}
            //catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
            // record the stream
            //Bass.BASS_ChannelPlay(rechandle, false);
        }

        bool mutestate=false;
        float mem_val;
        public void mute(bool m, float value)
        {
            //if (_Stream == 0) return;
            //if (m) volume(0);
            //else
            //Bass.BASS_ChannelSetAttribute(_Stream, BASSAttribute.BASS_ATTRIB_VOL, value/100);
            var level = Bass.ChannelGetLevel(_chan);

            float v = 0;
            if (!mutestate) { mem_val = value; v = 0; mutestate = true; }
            else { mem_val = value; v = value; mutestate = false; }
            Bass.ChannelSetAttribute(_chan, ChannelAttribute.Volume, v/100);
        }

        public void volume(float v)
        {
            //if (_Stream == 0) return;
            //Bass.BASS_ChannelSetAttribute(_Stream, BASSAttribute.BASS_ATTRIB_VOL, v/100);

            //var channels = 0;
            //float dummy;

            //while (Bass.ChannelGetAttribute(_chan, ChannelAttribute.MusicVolumeChannel + channels, out dummy))
            //    channels++;


            //Bass.ChannelSetAttribute(_chan, ChannelAttribute.MusicVolumeInstrument, v / 100);

            Bass.ChannelSetAttribute(_chan, ChannelAttribute.Volume, v/100);
        }

        public void stop()
        {
            data.startTMRmanag = false;

           // Bass.Stop();
            Bass.StreamFree(_chan);
            _chan = 0;
            //Bass.BASS_PluginFree(_wmaPlugIn);
            //// close bass
            //Bass.BASS_Stop();
            //Bass.BASS_Free();

            //Bass.BASS_PluginFree(_wmaPlugIn);
        }

        //void init_device()
        //{
        //    // init the two output devices
        //    Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        //    Bass.BASS_Init(2, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

        //    // set the device context to the first device
        //    Bass.BASS_SetDevice(1);

        //    // create a first stream in this context
        //    //int stream1 = Bass.BASS_StreamCreateFile("test1.mp3", 0L, 0L, BASSFlag.BASS_DEFAULT);
        //    //Bass.BASS_ChannelPlay(stream1, false);

        //    // set the device context to the second device
        //    Bass.BASS_SetDevice(2);
        //    // create a second stream using this context
        //    //int stream2 = Bass.BASS_StreamCreateFile("test2.mp3", 0L, 0L, BASSFlag.BASS_DEFAULT);
        //    //Bass.BASS_ChannelPlay(stream2, false);

        //}
        public string getNameDevice(byte num)
        {
            //BASS_DEVICEINFO info = new BASS_DEVICEINFO();
            //info.name = string.Empty;
            //Bass.BASS_GetDeviceInfo(num, info);
            // return info.name;

            string nn = "";

            var count = 0;
            DeviceInfo info;

            for (var a = 0; Bass.GetDeviceInfo(a, out info); a++)
            {
                if (info.IsEnabled) // device is enabled
                    count++; // count it

                if (num == count) nn = info.Name;
            }


            return nn;

        }
        public void ChannelSetDevice(byte device, string _name)
        {
            try
            {
                // if you want to change the output of the first stream to the second output
                // you might call this (even during playback)      
                //bool rez = Bass.BASS_ChannelSetDevice(_Stream, device);
                //int dev = Bass.BASS_GetDevice();
                //int num = Bass.BASS_GetDeviceCount();

                //BASS_DEVICEINFO info = new BASS_DEVICEINFO();
                //info.name = string.Empty;
                //info.driver = string.Empty;
                //info.flags = 0;
                //Bass.BASS_GetDeviceInfo(num, info);

                //if (!rez) System.Windows.MessageBox.Show(
                //    "Ошибка устройства вывода " + device.ToString() +
                //    "\nИмя " + _name+"/"+ info.name
                //    + "\ndriver= " + info.driver
                //    + "\nflags= " + info.flags
                //    + "\nТекущее " + dev.ToString()
                //    + "\nВсего устр. " + num.ToString()

                //    );

                stop();


                // Bass.Free();
                currentdevice = device;

                data._bass.create_stream(data.url, data.mode_radio, null);


                updPlay();


                //WinPOP.init_ok = false;

                //init();


            }
            catch (Exception ex) { System.Windows.MessageBox.Show("ошибка dev " + ex.ToString()); }
        }

        //private int _byteswritten = 0;
        //private byte[] _recbuffer = new byte[1048510]; // 1MB buffer
        public string Status;

        //private bool MyRecoring(int handle, IntPtr buffer, int length, IntPtr user)
        //{
        //    // just a dummy here...nothing is really written to disk...
        //    if (length > 0 && buffer != IntPtr.Zero)
        //    {
        //        // copy from managed to unmanaged memory
        //        // it is clever to NOT alloc the byte[] everytime here, since ALL callbacks should be really fast!
        //        // and if you would do a 'new byte[]' every time here...the GarbageCollector would never really clean up that memory here
        //        // even other sideeffects might occure, due to the fact, that BASS micht call this callback too fast and too often...
        //        Marshal.Copy(buffer, _recbuffer, 0, length);
        //        // write to file
        //        // NOT implemented her...;-)
        //        _byteswritten += length;
        //        Console.WriteLine("Bytes written = {0}", _byteswritten);
        //        if (_byteswritten < 800000)
        //            return true; // continue recording
        //        else
        //            return false;
        //    }
        //    return true;
        //}

        //private void MyDownloadProc(IntPtr buffer, int length, IntPtr user2)
        //{

        //    if (buffer != IntPtr.Zero && length == 0)
        //    {
        //        ///the buffer contains HTTP or ICY tags.
        //        //string txt = Marshal.PtrToStringAnsi(buffer);
        //        //this.Invoke(new UpdateMessageDelegate(UpdateMessageDisplay), new object[] { txt });
        //       // you might instead also use "this.BeginInvoke(...)", which would call the delegate asynchron!
        //    }
        //}

        ////private void MetaSync(int handle, int channel, int data, IntPtr user)
        ////{
        ////    try
        ////    {
        ////        // BASS_SYNC_META is triggered on meta changes of SHOUTcast streams
        ////        if (_tagInfo.UpdateFromMETA(Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META), false, true))
        ////        {
        ////            // this.Invoke(new UpdateTagDelegate(UpdateTagDisplay));
        ////            // new UpdateTagDelegate(UpdateTagDisplay);
        ////        }
        ////    }
        ////    catch 
        ////    { }// { MessageBox.Show(ex.ToString()); }
        ////}

        //public delegate void UpdateTagDelegate();
        //private void UpdateTagDisplay()
        //{
        //    //this.textBoxAlbum.Text = _tagInfo.album;
        //    //this.textBoxArtist.Text = _tagInfo.artist;
        //    //this.textBoxTitle.Text = _tagInfo.title;
        //    //this.textBoxComment.Text = _tagInfo.comment;
        //    //this.textBoxGenre.Text = _tagInfo.genre;
        //    //this.textBoxYear.Text = _tagInfo.year;
        //}

        //public delegate void UpdateStatusDelegate(string txt);
        //private void UpdateStatusDisplay(string txt)
        //{
        //    //this.statusBar1.Text = txt;
        //}

        //public delegate void UpdateMessageDelegate(string txt);
        //private void UpdateMessageDisplay(string txt)
        //{
        //    //this.textBox1.Text += "Tags: " + txt + Environment.NewLine;
        //}


        public void updPlay()
        {
            data._bass.SET_All_Param_VST();

            if (data.mode_radio)
            {
                data._bass.playStream();
            }
        }


    }
}
