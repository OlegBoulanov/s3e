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
            string fileBase = Path.ChangeExtension(filePath, null).Replace('/', '\\').ToLower();
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
                Windows.MsiProperties msiProps = null;
                if (extInfo.ContainsKey(msiExtension))
                {
                    msiPath = fileBase + msiExtension;
                    bool msiDownloaded = extInfo[msiExtension];
                    if (extInfo.ContainsKey(iniExtension))
                    {
                        bool iniDownloaded = extInfo[iniExtension];
                        string iniPath = fileBase + iniExtension;
                        msiProps = new Windows.MsiProperties();
                        msiProps.Read(iniPath);
                        if (iniDownloaded && !msiDownloaded) msiExecKeys = msiExecKeysReinstallAll;
                    }
                    else
                    {
                        // should we remove existing local one too?
                    }
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
            if (!string.IsNullOrEmpty(msiExecArgs)) sb.AppendFormat(" {0}", msiExecArgs);
            if (null != props)
            {
                foreach (string propName in props.Keys)
                {
                    sb.AppendFormat(" {0}=\"{1}\"", propName, props[propName]);
                }
            }
            return sb.ToString();
        }
    }
}
