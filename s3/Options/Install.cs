using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Diagnostics;

namespace s3.Options
{
    class Install : OptionWithParameter<string>
    {
        protected override bool ParameterIsCompulsory
        {
            get { return false; }
        }
        protected override void ParameterIsSet()
        {
        }

        public const string msiExtension = ".msi", iniExtension = ".ini";

        public Dictionary<string, Dictionary<string, bool>> ItemsToInstall = new Dictionary<string,Dictionary<string,bool>>();   // [base][ext] => downloaded

        public void SetFile(string filePath, bool downloaded)
        {
            string fileBase = Path.ChangeExtension(filePath, null).Replace('/', Path.DirectorySeparatorChar).ToLower();
            string fileExt = Path.GetExtension(filePath).ToLower();
            if (!ItemsToInstall.ContainsKey(fileBase)) ItemsToInstall.Add(fileBase, new Dictionary<string, bool>());
            if (!ItemsToInstall[fileBase].ContainsKey(fileExt)) ItemsToInstall[fileBase].Add(fileExt, false);
            ItemsToInstall[fileBase][fileExt] = downloaded;
        }
        public void InstallProducts(bool doInstall)
        {
            string msiArgs = Parameter;
            //Console.WriteLine("Installing {0} product{1}{2}...", ItemsToInstall.Count, 1 == ItemsToInstall.Count ? "" : "s", msiArgs == null ? "" : " with '" + msiArgs + "'");
            foreach (string fileBase in ItemsToInstall.Keys)
            {
                Dictionary<string, bool> extInfo = ItemsToInstall[fileBase];
                string msiPath = null;
                string msiExecKeys = msiExecKeysInstall;
                Windows.IniFile iniFile = Windows.IniFile.Read(fileBase + iniExtension);
                Windows.IniSection msiInfo = iniFile["$msi$"];
                Windows.IniSection msiProps = iniFile["$props$"];
                msiPath = fileBase + msiExtension;
                bool iniDownloaded = extInfo[iniExtension];
                bool msiDownloaded = extInfo[msiExtension];
                // got ini? read the props and decide if reinstall is needed
                if (extInfo.ContainsKey(iniExtension))
                {
                    // is it new? (assume props change in that case)
                    string iniPath = fileBase + iniExtension;
                }
                if (extInfo.ContainsKey(msiExtension))
                {
                    // if only props changed, we'd need to do full reinstall to set them...
                    if (iniDownloaded && !msiDownloaded) msiExecKeys = msiExecKeysReinstallAll;
                }
                else
                {
                    // there is no *.msi on remote
                    // should we remove existing local one too?
                    // try indirection
                }
                // got msi? Install it.
                if (!string.IsNullOrEmpty(msiPath))
                {
                    string cmdLineArgs = FormatCommand(msiExecKeys, msiPath, msiArgs, msiProps);
                    Console.WriteLine("{0} {1}", msiExec, cmdLineArgs);
                    if (doInstall)
                    {
                        Process process = Process.Start(msiExec, cmdLineArgs);
                        process.WaitForExit();
                        if (0 != process.ExitCode)
                        {
                            Console.WriteLine("{0} exited with {1}", msiExec, process.ExitCode);
                        }
                    }
                }
            }
        }
        const string msiExec = "msiexec.exe", msiExecKeysInstall = "/i", msiExecKeysReinstallAll = "/fva";
        public static string FormatCommand(string msiExecKeys, string path2msi, string msiExecArgs, IDictionary<string, string> props)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} \"{1}\"", msiExecKeys, path2msi);
            if (null != props)
            {
                foreach (string propName in props.Keys)
                {
                    sb.AppendFormat(" {0}=\"{1}\"", propName, props[propName]);
                }
            }
            // now apply extra args, so they may override ini props
            if (!string.IsNullOrEmpty(msiExecArgs)) sb.AppendFormat(" {0}", msiExecArgs);
            return sb.ToString();
        }
    }
}
