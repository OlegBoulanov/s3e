
using System;
using System.Collections.Generic;
using System.Text;

namespace s3.Windows
{
    public class MsiProperties : Dictionary<string, string>
    {
        const string sectionProps = "$props$";
        public int Read(string path)
        {
            foreach (string propName in Windows.IniReader.KeyNames(path, sectionProps))
            {
                string propValue = Windows.IniReader.Value(path, sectionProps, propName);
                this[propName] = TrimValue(propValue);
            }
            return Count;
        }
        string TrimValue(string s)
        {
            int i = s.IndexOfAny(new char[] { ';' });
            if (0 <= i) s = s.Substring(0, i);
            return s.Trim();
        }
    }
}
