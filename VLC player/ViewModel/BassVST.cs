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
using Un4seen.Bass.AddOn.Vst;
using System.Threading.Tasks;
using ManagedBass.Vst;

/// <summary>
/// //////////Assembly: Bass.Net (in Bass.Net.dll) Version: 2.4.13.3
/// </summary>

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


    public partial class  AudioBass
    {
        public List<WORKVST> work_list_vst = new List<WORKVST>();

     

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
            // var handle = Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_ChannelSetDSP(_Stream, path,
            //                     BASSVSTDsp.BASS_VST_DEFAULT, 1);

            var handle =  ManagedBass.Vst.BassVst.ChannelSetDSP(_chan, path, BassVstDsp.Default, 1);

            //if (handle < 1)
            //{
            //    System.Windows.MessageBox.Show("Ошибка подключения " + get_nameVTS(path));
            //    return false;
            //}
            work_list_vst.Add(new WORKVST(handle, path));
            Trace.WriteLine("ENABLE header=" + handle.ToString() + " " + path);
            return true;
        }

        int getHAndle(string s)
        {
            foreach (var v in work_list_vst)
            {
                Trace.WriteLine("work-header = " + v.handle.ToString());
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
            //Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_ChannelRemoveDSP(_Stream, getHAndle(del));
            ManagedBass.Vst.BassVst.ChannelRemoveDSP(_chan, getHAndle(del));

           remove_workLIST(del, ref work_list_vst);
            remove_VST(del, ref data.workVST);
            data.UpdateLIST();
        }

        public void DISABLE_ALL_VST()
        {
            List<string> lstVST = new List<string>();

            foreach (var dsp in data.workVST)
            {
                lstVST.Add(dsp);
            }

            foreach (var dsp in lstVST)
            {
                VST_DISABLE(get_nameVTS(dsp));
            }
            lstVST.Clear();
            data.pathVST.Clear();
        }

        void remove_workLIST(string s, ref List<WORKVST> col)
        {
            try
            {
                WORKVST findobj = null;
                s = s.Trim();

                foreach (var obj in col)
                {
                    if (new Regex(s).Match(obj.path.ToString().Trim()).Success)
                    {
                        findobj = obj;
                    }
                }
                if (findobj != null) { col.Remove(findobj); }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("Ошибка удаления " + ex.Message); }
        }

        public void OPEN_VST(string s)
        {
            foreach (var w in work_list_vst)
            {
                if (w.wnd != null)
                    if (w.wnd.IsDisposed)
                    {
                        // Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_EmbedEditor(w.handle, IntPtr.Zero);

                        ManagedBass.Vst.BassVst.EmbedEditor(w.handle, IntPtr.Zero);
                    }
            }
            string path = get_path_VST(s);
            int _hdl = getHAndle(get_nameVTS(path));
            if (_hdl < 0) { System.Windows.MessageBox.Show("Ошибка открытия " + s); return; }

            // show the embedded editor
            BASS_VST_INFO vstInfo = new BASS_VST_INFO();
           // BassVstInfo vstInfo = new BassVstInfo();

           if (Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_GetInfo(_hdl, vstInfo) && vstInfo.hasEditor)
            //if (ManagedBass.Vst.BassVst.GetInfo(_hdl, out vstInfo) && vstInfo.HasEditor)        
            {
                int id = getID(_hdl);
                if (work_list_vst[id].wnd != null)
                    if (!work_list_vst[id].wnd.IsDisposed)
                        return;

                work_list_vst[id].wnd = new Form();


                //work_list_vst[id].wnd.Width = vstInfo.EditorWidth  + 4;
                //work_list_vst[id].wnd.Height = vstInfo.EditorHeight + 34;
                //work_list_vst[id].wnd.Closing += new CancelEventHandler(windsp_Closing);
                //work_list_vst[id].wnd.Text = data.name + " " + vstInfo.EffectName;


                work_list_vst[id].wnd.Width = vstInfo.editorWidth  + 4;
                work_list_vst[id].wnd.Height = vstInfo.editorHeight + 34;
                work_list_vst[id].wnd.Closing += new CancelEventHandler(windsp_Closing);
                work_list_vst[id].wnd.Text = data.name + " " + vstInfo.effectName;


                work_list_vst[id].wnd.Show();


                Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_EmbedEditor(_hdl, work_list_vst[id].wnd.Handle);
              //  ManagedBass.Vst.BassVst.EmbedEditor(_hdl, work_list_vst[id].wnd.Handle);
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

        public bool path_is_ok(string s)
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



        /// <summary>
        /// ЗАбирает все параметры из активных VST
        /// </summary>
        public void Get_All_Param_VST()
        {
            data.listPARAM.Clear();
            foreach (var s in work_list_vst)
            {
                if (s.handle > 0)
                {
                    var rezult = VST_GET_PARAM(s.handle);
          
                    data.listPARAM.Add(rezult);
                }
            }
        }

        /// <summary>
        /// Пишет параметры во все активные VST
        /// </summary>
        public void SET_All_Param_VST()
        {
            enableVST();
            if (work_list_vst.Count == 0) { return; }
            int index = 0;
            try
            {
                foreach (var mylist in data.listPARAM)
                {
                    if (VST_SET_PARAM(work_list_vst[index].handle, mylist))
                        System.Windows.MessageBox.Show("Ошибка установки параметров VST МОДУЛЯ");

                    index++;
                }
            }
            catch
            {

            }
        }


   
        List<float> VST_GET_PARAM(int vstHandle)
        {
            List<float> rezparam = new List<float>(); 

            try
            {
                int vstParams = Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_GetParamCount(vstHandle);
                //int vstParams = ManagedBass.Vst.BassVst.GetParamCount(vstHandle);

               // create a paramInfo object
               BASS_VST_PARAM_INFO paramInfo = new BASS_VST_PARAM_INFO();

               // BassVstParamInfo paramInfo = new BassVstParamInfo();

                for (int i = 0; i < vstParams; i++)
                {
                    // get the info about the parameter
                     float paramValue = Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_GetParam(vstHandle, i);
                   // float paramValue = ManagedBass.Vst.BassVst.GetParam(vstHandle, i);

                    // and get further info about the parameter
                    Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_GetParamInfo(vstHandle, i, paramInfo);
                   // ManagedBass.Vst.BassVst.GetParamInfo(vstHandle, i, out paramInfo);

                    rezparam.Add(paramValue);
                   // Trace.WriteLine("GETparam: " + paramInfo.ToString() + " = " + paramValue.ToString());
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("vst get "+ex.Message); }


            return rezparam;
        }

        bool VST_SET_PARAM(int vstHandle, List<float> data)
        {
            bool error = false;
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    Trace.WriteLine("handle=" + vstHandle.ToString() + "  " +
                        i.ToString() + " SETparam " + data[i].ToString());

                   var s = Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_SetParam(vstHandle, i, data[i]);
                  // var s  = ManagedBass.Vst.BassVst.SetParam(vstHandle, i, data[i]);
                    if (!s) error = true;
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            return error;
        }

        void VST_GET_PARAM_count()
        {


        }

        /// <summary>
        /// Retrieving a program name other than the current one enforces to change the current program inernally
        /// </summary>
        /// <param name="vstHandle"></param>
        /// <returns></returns>
        //List<string> VST_GET_AFFECT(int vstHandle)
        //{
        //    List<string> spis = null;
        //   // BASS_VST_INFO vstInfo = new BASS_VST_INFO();
        //    BassVstInfo vstInfo = new BassVstInfo();


        //    //if (Un4seen.Bass.AddOn.Vst.BassVst.BASS_VST_GetInfo(vstHandle, vstInfo))
        //    if (ManagedBass.Vst.BassVst.GetInfo(vstHandle, out vstInfo))
        //    {
        //        if (vstInfo.AEffect != IntPtr.Zero)
        //        {
        //           // BASS_VST_AEFFECT aeffect = BASS_VST_AEFFECT.FromIntPtr(vstInfo.aeffect);
        //            // list all available programs
        //            for (int i = 0; i < aeffect.numPrograms; i++)
        //                spis.Add(aeffect.GetProgramName(i));
        //        }

        //    }
        //    return spis;
        //}


        //запуск предустановленных VST
        void enableVST()
        {
            foreach (var dsp in data.workVST)
            {
                Trace.WriteLine("START VST " + dsp);
                if (!VST_ENABLE(dsp)) System.Windows.MessageBox.Show("Ошибка запуска VST " +
                                                                            get_nameVTS(dsp));
            }
        }




    }



}
