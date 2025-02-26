﻿using System;
using System.Threading;
using Supabase;
using System.Threading.Tasks;
using Postgrest.Models;
using Postgrest.Attributes;
using Websocket.Client.Logging;

public class ActivityStatus : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("last_active")]
    public DateTime LastActive { get; set; }

    [Column("user_id")]
    public string UserId { get; set; }
}

class Program
{
    private static readonly string SUPABASE_URL = "https://wqwebhenkkyahqdcghra.supabase.co";
    private static readonly string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Indxd2ViaGVua2t5YWhxZGNnaHJhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDA0OTA0MDgsImV4cCI6MjA1NjA2NjQwOH0.HYZZbECOBEx_Qqck_XSKKCg2hNWKWnX4H9-nD1MDrHA";
    private static readonly Client supabase = new Client(SUPABASE_URL, SUPABASE_ANON_KEY);
    private static bool isRunning = true;
    private static string userId = string.IsNullOrEmpty(Environment.UserName) ? "default_user" : Environment.UserName;

    static async Task Main(string[] args)
    {
        await supabase.InitializeAsync();

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

        Console.WriteLine("Activity Tracker Started");
        Console.WriteLine("Press Ctrl+C to exit");

        await UpdateStatusAsync(true);

        while (isRunning)
        {
            await UpdateLastActiveTimeAsync();
            await DisplayCurrentStatusAsync();
            Thread.Sleep(60000);
        }
    }

    static async Task UpdateStatusAsync(bool isActive)
    {
        try
        {
            var status = new ActivityStatus
            {
                IsActive = isActive,
                LastActive = DateTime.UtcNow,
                UserId = userId
            };

            Console.WriteLine($"Attempting to upsert status: IsActive={status.IsActive}, LastActive={status.LastActive}, UserId={status.UserId}");
            var response = await supabase.From<ActivityStatus>().Upsert(status);
            Console.WriteLine("Status updated successfully. Response: " + (response != null ? response.ToString() : "Null response"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating status: {ex.Message} - {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }

    static async Task UpdateLastActiveTimeAsync()
    {
        try
        {
            var status = new ActivityStatus
            {
                IsActive = true,
                LastActive = DateTime.UtcNow,
                UserId = userId
            };

            Console.WriteLine($"Attempting to update last active time: IsActive={status.IsActive}, LastActive={status.LastActive}, UserId={status.UserId}");
            var response = await supabase.From<ActivityStatus>().Upsert(status);
            Console.WriteLine("Last active time updated successfully. Response: " + (response != null ? response.ToString() : "Null response"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating time: {ex.Message} - {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }

    static async Task DisplayCurrentStatusAsync()
    {
        try
        {
            var response = await supabase.From<ActivityStatus>()
                .Where(x => x.UserId == userId)
                .Single();

            Console.Clear();
            Console.WriteLine("Activity Status:");
            if (response != null)
            {
                Console.WriteLine($"IsActive: {response.IsActive}");
                Console.WriteLine($"LastActive: {response.LastActive}");
            }
            else
            {
                Console.WriteLine("No status found for user: {userId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error displaying status: {ex.Message} - {ex.StackTrace}");
        }
    }

    static void OnProcessExit(object sender, EventArgs e)
    {
        Task.Run(async () => await UpdateStatusAsync(false)).Wait();
        isRunning = false;
    }
}