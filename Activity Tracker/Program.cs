using System;
using System.Threading;
using Supabase;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using Activity_Tracker;
using IWshRuntimeLibrary;
using System.Diagnostics;
using ActivityTracker;

class Program
{
    private static readonly string SUPABASE_URL = "https://wqwebhenkkyahqdcghra.supabase.co";
    private static readonly string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Indxd2ViaGVua2t5YWhxZGNnaHJhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDA0OTA0MDgsImV4cCI6MjA1NjA2NjQwOH0.HYZZbECOBEx_Qqck_XSKKCg2hNWKWnX4H9-nD1MDrHA";
    private static readonly Client supabase = new Client(SUPABASE_URL, SUPABASE_ANON_KEY);

    [STAThread]
    static async Task Main(string[] args)
    {
        try
        {
            await supabase.InitializeAsync();

            // Add to startup folder
            AddtoStartUp.AddToStartup();

            // Use a default user ID initially
            string initialUserId = Environment.UserName?.ToLower() ?? "default_user";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(supabase, initialUserId));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start Activity Tracker: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}