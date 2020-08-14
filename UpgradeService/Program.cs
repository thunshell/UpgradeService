using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UpgradeService
{
    class Program
    {
        string dataFile = "upgradeservice.log";
        string target;
        string source;
        bool autoCallback;
        string sourceParameter;
        string targetParemeter;
        bool isOverride = true;

        static void Main(string[] args)
        {
            Program app = new Program();
            string rootpath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Path.Combine(rootpath, app.dataFile);
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-s": app.source = args[++i]; break;
                        case "-t": app.target = args[++i]; break;
                        case "-o": app.isOverride = true; break;
                        case "-a": app.autoCallback = true; break;
                        case "-sp": app.sourceParameter = args[++i]; break;
                        case "-tp": app.targetParemeter = args[++i]; break;
                        case "-h": app.ConsoleHelp(); return;
                    }
                }
            }
            catch
            {
                app.ConsoleHelp();
                return;
            }
            try
            {
                if (!File.Exists(dataPath)) return;
                string[] lines = File.ReadAllLines(dataPath);
                if (lines.Length % 2.0 > 0)
                {
                    Console.WriteLine("File mismatch.");
                    return;
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    string oldfile = lines[i];
                    if (Extension.IsNulOrWhiteSpace(oldfile))
                        continue;
                    string newfile = lines[++i];
                        if(Extension.IsNulOrWhiteSpace(newfile))
                        continue;

                    if (File.Exists(oldfile))
                            File.Delete(oldfile);
                    File.Copy(newfile, oldfile, app.isOverride);
                    File.Delete(newfile);
                    if (app.autoCallback && !Extension.IsNulOrWhiteSpace(app.source))
                        System.Diagnostics.Process.Start(app.source, string.Format("-upgradeservice {0}/{1}", (lines.Length - i) / 2, lines.Length / 2) + app.sourceParameter);
                }
                if (!Extension.IsNulOrWhiteSpace(app.target))
                    System.Diagnostics.Process.Start(app.target, app.targetParemeter);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            if (File.Exists(dataPath)) File.Delete(dataPath);
        }

        void ConsoleHelp()
        {
            Console.WriteLine("-h      Show helper");
            Console.WriteLine("-a      Should call source application when upgraded a file. Example: upgradeservice.exe -a");
            Console.WriteLine("-o      Should override file. Example: upgradeservice.exe -o");
            Console.WriteLine("-s      The source application. Example: upgradeservice.exe -s \"c://a/b.exe\"");
            Console.WriteLine("-sp     Source application's paremeters. Example: upgradeservice.exe -sp \"paremeter1 paremeter2...\"");
            Console.WriteLine("-t      The target application. Example: upgradeservice.exe -s \"c://a/c.exe\"");
            Console.WriteLine("-tp     Target application's paremeters. Example: upgradeservice.exe -tp \"paremeter1 paremeter2...\"");
        }
    }

    public static class Extension
    {
        public static bool IsNulOrWhiteSpace(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }
    }
}
