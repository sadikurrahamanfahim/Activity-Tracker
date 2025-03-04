# Activity Tracker

![Activity Tracker Logo](https://raw.githubusercontent.com/sadikurrahamanfahim/Activity-Tracker/refs/heads/main/ActivityTracker.png)

A simple console application built with C# and .NET that tracks your active status and last active time, storing the data in a Supabase online database. The app automatically starts when your computer boots up, sets your status to "active," and updates your last active time every minute. When you shut down your computer, it marks you as "inactive."

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [How It Works](#how-it-works)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [License](#license)

## Features
- Automatically tracks your active status when your computer starts.
- Updates your last active time every minute while the app is running.
- Marks you as inactive when your computer shuts down.
- Stores activity data in a Supabase online database, making it accessible across devices.
- Runs on startup without requiring manual configuration.
- Self-contained executable, so no additional software installation is needed.

## Prerequisites
- A Windows computer (the app is currently built for Windows only).
- An active internet connection to communicate with the Supabase database.
- No additional software or runtime installation is required, as the app is self-contained.

## Installation
1. **Download the App**:
   - Download the `ActivityTracker.exe` file from the [Releases](https://github.com/sadikurrahamanfahim/Activity-Tracker/blob/main/Activity%20Tracker.exe) page.

2. **Run the App**:
   - Double-click the `ActivityTracker.exe` file to launch the app.
   - The app will automatically add itself to your Windows Startup folder, so it runs every time your computer starts.

3. **Verify It’s Working**:
   - You should see a console window with messages like:
- Initializing Supabase client...
- Supabase client initialized successfully.
- Activity Tracker Started
- Press Ctrl+C to exit
- Activity Status:
- IsActive: True
- LastActive: 3/3/2025 9:56:38 AM

- The app will update your status and last active time every minute.

## Usage
- **Run the App**: Simply double-click `ActivityTracker.exe` to start tracking your activity.
- **Check Status**: The console window displays your current status (`IsActive`) and the last time you were active (`LastActive`).
- **Stop the App**: Press `Ctrl+C` in the console window to stop the app manually. On shutdown, the app automatically sets your status to "inactive."
- **Startup**: The app automatically starts when you log into your computer, thanks to the shortcut in the Windows Startup folder.

## How It Works
1. **On Startup**:
- The app launches automatically (via the Startup folder).
- It sets your status to `IsActive: True` in the Supabase `activity_status` table.
- It records the current time as your `LastActive` time.

2. **While Running**:
- Every minute, the app updates your `LastActive` time in the database.
- The console displays your current status and last active time.

3. **On Shutdown**:
- When you close the app or shut down your computer, the app updates your status to `IsActive: False`.

4. **Database**:
- The app uses Supabase to store your activity data in an `activity_status` table with columns `id`, `is_active`, `last_active`, and `user_id`.
- The `user_id` is based on your Windows username (e.g., `fahim`).

## Dependencies
The app is a self-contained .NET application, so all dependencies are bundled in the executable. You don’t need to install anything separately. The app uses the following libraries (already included):
- **supabase-csharp**: For interacting with the Supabase database.
- **IWshRuntimeLibrary**: For creating a shortcut in the Windows Startup folder.

## Contributing
Contributions are welcome! If you’d like to improve the app or add new features, follow these steps:

1. **Fork the Repository**:
- Fork this repository to your GitHub account.

2. **Clone the Repository**
3. **Make Changes:**
- Open the project in Visual Studio.
- Make your changes and test them.
4. **Submit a Pull Request:**
- Push your changes to your fork and submit a pull request to this repository.
