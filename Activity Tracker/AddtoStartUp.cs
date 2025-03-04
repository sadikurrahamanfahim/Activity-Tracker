using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activity_Tracker
{
    internal class AddtoStartUp
    {
        public static void AddToStartup()
        {
            try
            {
                string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolder, "ActivityTracker.lnk");
                string appPath = Process.GetCurrentProcess().MainModule.FileName;

                if (!System.IO.File.Exists(shortcutPath))
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = appPath;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(appPath);
                    shortcut.Description = "Activity Tracker Shortcut";
                    shortcut.Save();
                    Console.WriteLine("Added to startup successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add to startup: {ex.Message}");
            }
        }
    }
}
