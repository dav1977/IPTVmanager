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
using Un4seen.Bass.AddOn.Vst;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Linq;

namespace IPTVman.ViewModel
{
    public class WORKVST
    {
        public int handle;
        public string path;
        public Form wnd = null;

        public WORKVST(int handle, string path)
        {
            this.handle = handle;
            this.path = path;
        }
    }
    // NOTE: Needs 'bass.dll' - copy it to your output directory first!
    //       needs 'basswma.dll' - copy it to your output directory first!
    public class AudioBass
    {
        // PINNED
        private string _myUserAgent = "RADIO42";
        [FixedAddressValueType()]
        public IntPtr _myUserAgentPtr;
        //string tags;
        private int _Stream = 0;
        private string _url = String.Empty;
        private DOWNLOADPROC myStreamCreateURL;
        private TAG_INFO _tagInfo;
        private SYNCPROC mySync;
        //private RECORDPROC myRecProc;
        private int _wmaPlugIn = 0;
        bool isWMA = false;
        public List<WORKVST> work_list_vst = new List<WORKVST>();
       
        public void init()
		{
            if (WinPOP.init_ok) return;
            _myUserAgentPtr = Marshal.StringToHGlobalAnsi(_myUserAgent);

            //BassNet.Registration("your email", "your regkey");

            // check the version..
            if (Utils.HighWord(Bass.BASS_GetVersion()) != Bass.BASSVERSION)
            {
                System.Windows.MessageBox.Show( "Wrong Bass Version!");
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
                    System.Windows.MessageBox.Show("Ошибка Bass_Init " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()) );
				}
				// we alraedy create the user callback methods...
				myStreamCreateURL = new DOWNLOADPROC(MyDownloadProc);
                WinPOP.init_ok = true;
            }
			else
                System.Windows.MessageBox.Show( "Bass_Init error!" );
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
            BASS_DEVICEINFO info = new BASS_DEVICEINFO();
            info.name = string.Empty;
            Bass.BASS_GetDeviceInfo(num, info);
            return info.name;
        }

        public void ChannelSetDevice(byte device, string _name)
        {
            try
            {
                // if you want to change the output of the first stream to the second output
                // you might call this (even during playback)      
                bool rez = Bass.BASS_ChannelSetDevice(_Stream, device);
                int dev = Bass.BASS_GetDevice();
                int num = Bass.BASS_GetDeviceCount();

                BASS_DEVICEINFO info = new BASS_DEVICEINFO();
                info.name = string.Empty;
                info.driver = string.Empty;
                info.flags = 0;
                Bass.BASS_GetDeviceInfo(num, info);

                if (!rez) System.Windows.MessageBox.Show(
                    "Ошибка устройства вывода " + device.ToString() +
                    "\nИмя " + _name+"/"+ info.name
                    + "\ndriver= " + info.driver
                    + "\nflags= " + info.flags
                    + "\nТекущее " + dev.ToString()
                    + "\nВсего устр. " + num.ToString()

                    );
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("ошибка "+ex.Message); }
        }


        public string get_path_VST(string s)
        {
            foreach (var p in work_list_vst)
            {
                if (new Regex(s).Match(p.path).Success) { return p.path.Trim(); }
            }
            return "";
        }

        public bool VSTisENABLED(string s)
        {
            if (work_list_vst.Count == 0) return false;
            foreach (var p in work_list_vst)
                      if (get_nameVTS(p.path) == s) { return true; }
            return false;
        }

        public bool VST_ENABLE(string path)
        {
            var handle = BassVst.BASS_VST_ChannelSetDSP(_Stream, path,
                                BASSVSTDsp.BASS_VST_DEFAULT, 1);
            if (handle < 1)
            { System.Windows.MessageBox.Show("Ошибка подключения "+get_nameVTS(path));
                return false; }
            work_list_vst.Add(new WORKVST(handle, path));
            Trace.WriteLine("add hd="+handle.ToString());
            return true;
        }


        int getHAndle(string s)
        {
            foreach (var v in work_list_vst)
            {
                Trace.WriteLine("listwork = "+v.handle.ToString());
                if (get_nameVTS(v.path.ToString()) == s) { return v.handle; }
            }
            return -1;
        }
        
        /// <summary>
        /// возврат индекса по handler
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        int getID(int h)
        {
            int i = 0;
            foreach (var v in work_list_vst)
            {
                if (v.handle == h) { return i; }
                i++;
            }
            return -1;
        }

        public void VST_DISABLE(string del)
        {
            BassVst.BASS_VST_ChannelRemoveDSP(_Stream, getHAndle(del));

            remove_workLIST(del, ref work_list_vst);
            remove_VST(del, ref data.workVST);
            data.UpdateLIST();
        }

        void remove_workLIST(string s, ref List<WORKVST> col)
        {
            WORKVST findobj= null;
            s = s.Trim();

            foreach (var obj in col)
            {
                if (new Regex(s).Match(obj.path.ToString().Trim()).Success)
                {
                    findobj = obj;
                }
            }
            if (findobj != null) { Trace.WriteLine("remove hd=" + findobj.handle.ToString()); col.Remove(findobj); }
        }

        public void OPEN_VST(string s)
        {
            foreach (var w in work_list_vst)
            {
                if (w.wnd != null)
                    if (w.wnd.IsDisposed)
                    { 
                        BassVst.BASS_VST_EmbedEditor(w.handle, IntPtr.Zero);
                    }
            }
            string path = get_path_VST(s);
            int _hdl = getHAndle(get_nameVTS(path));
            if (_hdl < 0) { System.Windows.MessageBox.Show("Ошибка открытия "+s); return; }

            // show the embedded editor
            BASS_VST_INFO vstInfo = new BASS_VST_INFO();
            if (BassVst.BASS_VST_GetInfo(_hdl, vstInfo) && vstInfo.hasEditor)
            {
                int id=getID(_hdl);
                if (work_list_vst[id].wnd != null)
                    if(!work_list_vst[id].wnd.IsDisposed)
                                                       return;

                work_list_vst[id].wnd = new Form();
                work_list_vst[id].wnd.Width = vstInfo.editorWidth + 4;
                work_list_vst[id].wnd.Height = vstInfo.editorHeight + 34;
                work_list_vst[id].wnd.Closing += new CancelEventHandler(windsp_Closing);
                work_list_vst[id].wnd.Text = data.name+" "+vstInfo.effectName;
                work_list_vst[id].wnd.Show();
                BassVst.BASS_VST_EmbedEditor(_hdl, work_list_vst[id].wnd.Handle);

            }
        }

        private void windsp_Closing(object sender, CancelEventArgs e)
        {
            
        }

        string _current_pathVST = "no select";
        public string current_pathVST
        {
            get { return _current_pathVST; }
            set
            {
                _current_pathVST = value;
                data.UpdateSettings();
            }
        }


        public void addVST(string s)
        {
            data.pathVST.Add(s);
            current_pathVST = s;
            data.UpdateLIST();

        }

        public  bool path_is_ok(string s)
        {
            foreach (var obj in data.pathVST)
            {
                if (s.Trim() == obj)
                {
                    return true;
                }
            }
            return false;
        }

        //public void remove_dataPATH(string s)
        //{
        //    remove_VST(s, ref data.pathVST);
        //}
        public void remove_VST(string s, ref ObservableCollection<string> col)
        {
            string findobj = "";
            s = s.Trim();

            foreach (var obj in col)
            {
                if (new Regex(s).Match(obj.ToString().Trim()).Success)
                {
                    findobj = obj.ToString();
                }
            }
            if (findobj == "") return;

            var v = col.FirstOrDefault(p => p == findobj);
            if (v != null) col.Remove(v);

        }

        public string get_nameVTS(string text)
        {
            string[] words = text.Split(new char[] { '\\' });
            return words[words.Length - 1];
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
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
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

            //Trace.WriteLine(str + "&&" + ans + "=>" + tt);
            return ans.Trim();
        }


      
        public string get_tags(string nm, ref string bitr)
        {
            if (_Stream == 0) return "";
            try
            {              
                init_tag();

                Thread.Sleep(500);

                if (BassTags.BASS_TAG_GetFromURL(_Stream, _tagInfo))
                {
                    bitr = _tagInfo.bitrate.ToString();

                    string artist = StrEncod(_tagInfo.artist);
                    string title = StrEncod(_tagInfo.title);
                    title = title.Replace("- 0:00","");
                    string rez = artist + " - " + title;

                    if (artist == "" || artist == " " || artist == ";") rez = title;
                    if (title == "" || title == " " || title == ";" || title == "0:00" || title == "0") rez = artist;

                    if (artist == "" && title == "") rez = "no artist";

                    if (rez == " ") rez = "none";
                    return (rez);
                }

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
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
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

                //запуск предустановленных VST
                foreach (var dsp in data.workVST)
                {
                    if (!VST_ENABLE(dsp)) System.Windows.MessageBox.Show("Ошибка запуска VST " +
                                                                                get_nameVTS(dsp));
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString()); }
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

        private void MyDownloadProc(IntPtr buffer, int length, IntPtr user2)
        {

            if (buffer != IntPtr.Zero && length == 0)
            {
                ///the buffer contains HTTP or ICY tags.
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
            catch 
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
