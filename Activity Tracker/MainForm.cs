using System;
using System.Threading;
using System.Windows.Forms;
using Supabase;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using Activity_Tracker;

namespace ActivityTracker
{
    public partial class MainForm : Form
    {
        private readonly Client supabase;
        private bool isRunning = true;
        private string userId;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public MainForm(Client supabaseClient, string initialUserId)
        {
            InitializeComponent();
            supabase = supabaseClient;
            userId = initialUserId;
            txtUserId.Text = userId; // Display initial user ID
            StartTracking();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Activity Tracker";

            lblTitle = new Label
            {
                Text = "Activity Tracker",
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(260, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            lblStatus = new Label
            {
                Text = "Status: ",
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(260, 20)
            };

            lblLastActive = new Label
            {
                Text = "Last Active: ",
                Location = new System.Drawing.Point(10, 60),
                Size = new System.Drawing.Size(260, 20)
            };

            lblUserIdPrompt = new Label
            {
                Text = "User ID:",
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(50, 20)
            };

            txtUserId = new TextBox
            {
                Location = new System.Drawing.Point(60, 90),
                Size = new System.Drawing.Size(120, 20)
            };

            btnSetUserId = new Button
            {
                Text = "Set",
                Location = new System.Drawing.Point(190, 90),
                Size = new System.Drawing.Size(80, 25)
            };
            btnSetUserId.Click += BtnSetUserId_Click;

            btnStop = new Button
            {
                Text = "Stop App",
                Location = new System.Drawing.Point(10, 120),
                Size = new System.Drawing.Size(260, 30)
            };
            btnStop.Click += BtnStop_Click;

            this.Controls.AddRange(new Control[] { lblTitle, lblStatus, lblLastActive, lblUserIdPrompt, txtUserId, btnSetUserId, btnStop });
        }

        private Label lblTitle;
        private Label lblStatus;
        private Label lblLastActive;
        private Label lblUserIdPrompt;
        private TextBox txtUserId;
        private Button btnSetUserId;
        private Button btnStop;

        private async void StartTracking()
        {
            try
            {
                await UpdateStatusAsync(true);

                while (isRunning && !cts.Token.IsCancellationRequested)
                {
                    await UpdateLastActiveTimeAsync();
                    await DisplayCurrentStatusAsync();
                    await Task.Delay(60000, cts.Token); // Update every minute
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start Activity Tracker: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async Task UpdateStatusAsync(bool isActive)
        {
            try
            {
                var status = new ActivityStatus
                {
                    IsActive = isActive,
                    LastActive = DateTime.UtcNow,
                    UserId = userId
                };

                await supabase.From<ActivityStatus>().Upsert(status);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateLastActiveTimeAsync()
        {
            try
            {
                var status = new ActivityStatus
                {
                    IsActive = true,
                    LastActive = DateTime.UtcNow,
                    UserId = userId
                };

                await supabase.From<ActivityStatus>().Upsert(status);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating time: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DisplayCurrentStatusAsync()
        {
            try
            {
                var responseList = await supabase.From<ActivityStatus>()
                    .Filter("user_id", Postgrest.Constants.Operator.Equals, userId)
                    .Get();

                var response = responseList.Models.FirstOrDefault();

                if (response != null)
                {
                    lblStatus.Text = $"Status: {response.IsActive}";
                    lblLastActive.Text = $"Last Active: {response.LastActive}";
                }
                else
                {
                    lblStatus.Text = $"Status: Unknown";
                    lblLastActive.Text = $"Last Active: Unknown";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async  void BtnSetUserId_Click(object sender, EventArgs e)
        {
            string newUserId = txtUserId.Text.Trim();
            if (string.IsNullOrEmpty(newUserId))
            {
                MessageBox.Show("Please enter a valid User ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            userId = newUserId.ToLower();
            MessageBox.Show($"User ID set to: {userId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await UpdateLastActiveTimeAsync(); // updates the information also in database
            await DisplayCurrentStatusAsync(); // Refresh status with new user ID
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to stop the app?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                cts.Cancel();
                Task.Run(async () => await UpdateStatusAsync(false)).Wait();
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to stop the app?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            cts.Cancel();
            Task.Run(async () => await UpdateStatusAsync(false)).Wait();
        }
    }
}