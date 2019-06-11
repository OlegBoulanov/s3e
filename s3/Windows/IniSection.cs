
using System;
using System.Collections.Generic;
using System.Text;

namespace s3.Windows
{
    public class IniSection : Dictionary<string, string>
    {
        public static IniSection Read(string path, string section)
        {
            IniSection props = new IniSection();
            foreach (string propName in Windows.IniReader.KeyNames(path, section))
            {
                string propValue = Windows.IniReader.Value(path, section, propName);
                props[propName] = TrimValue(propValue);
            }
            return props;
        }
        static string TrimValue(string s)
        {
            int i = s.IndexOfAny(new char[] { ';' });
            if (0 <= i) s = s.Substring(0, i);
            return s.Trim();
        }
    }
}
