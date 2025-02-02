﻿using static DownloadManager.BetterProgressBar;
using Timer = System.Windows.Forms.Timer;

namespace DownloadManager
{
    public class DownloadItem
    {
        CurrentDownloads downloadsForm = CurrentDownloads._instance;
        Timer timer = new Timer();
        DownloadProgress progress;
        GroupBox groupBox;
        bool isYtDownload = false;

        #region Predefined Controls
        BetterProgressBar progressBar = new BetterProgressBar()
        {
            Location = new Point(290, 43),
            Size = new Size(121, 23),
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1,
            MarqueeAnim = true,
            BackColor = Color.FromArgb(20, 20, 20)
        };

        Label title1 = new Label()
        {
            AutoSize = false,
            Size = new Size(83, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(6, 19),
            Text = "Downloading:"
        };
        Label fileNameLabel = new Label()
        {
            AutoSize = false,
            Size = new Size(192, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(92, 19),
            Text = "fileName"
        };
        Label title2 = new Label()
        {
            AutoSize = false,
            Size = new Size(88, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(6, 38),
            Text = "Download URL:"
        };
        Label fileUrlLabel = new Label()
        {
            AutoSize = false,
            Size = new Size(187, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(97, 38),
            Text = "fileUrl"
        };
        Label title3 = new Label()
        {
            AutoSize = false,
            Size = new Size(87, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(6, 57),
            Text = "Download Size:"
        };
        Label fileSizeLabel = new Label()
        {
            AutoSize = false,
            Size = new Size(188, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(96, 57),
            Text = "0 Bytes"
        };
        Label title4 = new Label()
        {
            AutoSize = false,
            Size = new Size(112, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(6, 77),
            Text = "Download Progress:"
        };
        Label fileProgressLabel = new Label()
        {
            AutoSize = false,
            Size = new Size(163, 15),
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Location = new Point(121, 77),
            Text = "0%"
        };
        #endregion

        public void Initialize(DownloadProgress progress)
        {
            this.progress = progress;

            this.groupBox = new GroupBox()
            {
                Anchor = AnchorStyles.Top,
                Size = new Size(428, 105),
                Location = new Point(12, CurrentDownloads.nextY),
                Text = ""
            };

            downloadsForm.panel1.Controls.Add(groupBox);

            groupBox.Controls.Add(progressBar);
            groupBox.Controls.Add(title1);
            groupBox.Controls.Add(fileNameLabel);
            groupBox.Controls.Add(title2);
            groupBox.Controls.Add(fileUrlLabel);
            groupBox.Controls.Add(title3);
            groupBox.Controls.Add(fileSizeLabel);
            groupBox.Controls.Add(title4);
            groupBox.Controls.Add(fileProgressLabel);

            timer.Tick += UpdateTimer_Tick;
            timer.Interval = 500;
            timer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (progress == null || progress.downloading == false || progress.cancelled == true)
            {
                // If the download is finished, dispose the item
                Dispose();
                timer.Stop();
                return;
            }
            else
            {
                // Update the labels
                fileNameLabel.Text = progress.fileName;
                fileUrlLabel.Text = progress.url;
                fileSizeLabel.Text = progress.fileSize + " Bytes";

                if (progress.downloadType == DownloadProgress.DownloadType.YoutubePlaylist)
                {
                    // Update the progress bar
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Minimum = progress.progressBar1.Minimum;
                    progressBar.Maximum = progress.progressBar1.Maximum;
                    progressBar.Value = progress.progressBar1.Value;

                    // Update the progress label
                    int percent = (int)(((double)progress.progressBar1.Value / (double)progress.progressBar1.Maximum) * 100);
                    fileProgressLabel.Text = $"{percent}%";
                }
                else if (progress.downloadType == DownloadProgress.DownloadType.YoutubeVideo)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    // Update the progress bar
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = (int)progress.percentageDone;

                    // Update the progress label
                    fileProgressLabel.Text = progress.percentageDone.ToString() + "%";
                }

                // Bring all labels to front
                fileNameLabel.BringToFront();
                fileUrlLabel.BringToFront();
                fileSizeLabel.BringToFront();
                fileProgressLabel.BringToFront();
            }

            if (progress.isPaused)
            {
                progressBar.State = ProgressBarState.Warning;
            }
            else
            {
                progressBar.State = ProgressBarState.Normal;
            }
        }

        public void Dispose()
        {
            downloadsForm.itemList.Remove(this);

            timer.Stop();
            timer.Dispose();

            downloadsForm.Controls.Remove(groupBox);
            groupBox.Dispose();
            progressBar.Dispose();
            title1.Dispose();
            fileNameLabel.Dispose();
            title2.Dispose();
            fileUrlLabel.Dispose();
            title3.Dispose();
            fileSizeLabel.Dispose();
            title4.Dispose();
            fileProgressLabel.Dispose();
        }

        public void DisposeNoRemove()
        {
            timer.Stop();
            timer.Dispose();

            downloadsForm.Controls.Remove(groupBox);
            groupBox.Dispose();
            progressBar.Dispose();
            title1.Dispose();
            fileNameLabel.Dispose();
            title2.Dispose();
            fileUrlLabel.Dispose();
            title3.Dispose();
            fileSizeLabel.Dispose();
            title4.Dispose();
            fileProgressLabel.Dispose();
        }
    }
}
