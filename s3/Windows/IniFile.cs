
using System;
using System.Collections.Generic;
using System.Text;

namespace s3.Windows
{
    public class IniFile : Dictionary<string, IniSection>
    {
        public static IniFile Read(string path)
        {
            IniFile file = new IniFile();
            foreach (string sectionName in IniReader.SectionNames(path))
            {
                file[sectionName] = IniSection.Read(path, sectionName);
            }
            return file;
        }
    }
}
