using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;

namespace IPTVman.Model
{
    class REG_FIND
    {
       public REG_FIND()
        {
        }
        private class RegFindValue
        {
            RegistryKey regKey;
            string mProps;
            string mVal;
            RegFindIn mWhereFound;

            public RegistryKey Key
            { get { return regKey; } }

            public string Property
            { get { return mProps; } }

            public string Value
            { get { return mVal; } }

            public RegFindIn WhereFound
            { get { return mWhereFound; } }

            public RegFindValue(RegistryKey key, string props, string val, RegFindIn where)
            {
                regKey = key;
                mProps = props;
                mVal = val;
                mWhereFound = where;
            }
        }

        enum RegFindIn
        {
            Property,
            Value
        }


        int ct = 0;
        private RegFindValue RegFind(RegistryKey key, string find)
        {
            if (key == null || string.IsNullOrEmpty(find))
                return null;

            if (ct > 30000) return null;//ВЫХОД ИЗ РЕКУРСИИ

            string[] props = key.GetValueNames();
            object value = null;

            if (props.Length != 0)
                foreach (string property in props)
                {
                    if (property.IndexOf(find, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        return new RegFindValue(key, property, null, RegFindIn.Property);
                    }

                    value = key.GetValue(property, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                    if (value is string && ((string)value).IndexOf(find, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        return new RegFindValue(key, property, (string)value, RegFindIn.Value);
                    }
                }

            string[] subkeys = key.GetSubKeyNames();
            RegFindValue retVal = null;

            if (subkeys.Length != 0)
            {
                foreach (string subkey in subkeys)
                {
                    try
                    {
                        ct++;
                        //РЕКУРСИЯ!!!
                        retVal = RegFind(key.OpenSubKey(subkey, RegistryKeyPermissionCheck.ReadSubTree), find);
                    }
                    catch 
                    {
                        // err msg, if need
                    }
                    if (retVal != null)
                    {
                        return retVal;
                    }
                }
            }


            key.Close();
            return null;
        }

        // Применение:
        // RegFindValue retVal = RegFind(/* где искать */, /* что искать */);

        public string FIND(string f)
        {
                      
            RegistryKey[] key = {Registry.ClassesRoot,Registry.CurrentConfig,
                                   Registry.LocalMachine, Registry.CurrentUser,Registry.Users};
            string output="";

            foreach (RegistryKey rk in key)
            {
                RegFindValue r= RegFind(rk, f);

                if (r != null)
                {
                    output += r.Key.Name + "=>" + r.Property.ToString() + "=>" + r.Value.ToString() + Environment.NewLine + "\n";
                }
               
            }
            return output;
        }

    }
}
