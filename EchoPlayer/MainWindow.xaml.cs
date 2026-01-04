using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool isUserDragging = false;
        private bool isPlaying = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            mediaPlayer.Volume = volumeSlider.Value;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!isUserDragging && mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Value = mediaPlayer.Position.TotalSeconds;
                currentTimeText.Text = FormatTime(mediaPlayer.Position);
            }
        }

        private string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media Files|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.mp3;*.wav;*.flac|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Source = new Uri(openFileDialog.FileName);
                noMediaText.Visibility = Visibility.Collapsed;
                playPauseButton.IsEnabled = true;
                mediaPlayer.Play();
                isPlaying = true;
                playPauseButton.Content = "⏸";
                timer.Start();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source == null)
                return;

            if (isPlaying)
            {
                mediaPlayer.Pause();
                playPauseButton.Content = "▶";
                timer.Stop();
            }
            else
            {
                mediaPlayer.Play();
                playPauseButton.Content = "⏸";
                timer.Start();
            }
            isPlaying = !isPlaying;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            playPauseButton.Content = "▶";
            isPlaying = false;
            timer.Stop();
            progressSlider.Value = 0;
            currentTimeText.Text = "00:00:00";
        }

        private void Rewind_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Position -= TimeSpan.FromSeconds(10);
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Position += TimeSpan.FromSeconds(10);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Volume = volumeSlider.Value;
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
                progressSlider.Maximum = duration.TotalSeconds;
                totalTimeText.Text = FormatTime(duration);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            playPauseButton.Content = "▶";
            isPlaying = false;
            timer.Stop();
            progressSlider.Value = 0;
        }

        private void ProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isUserDragging = true;
        }

        private void ProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isUserDragging = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUserDragging)
            {
                currentTimeText.Text = FormatTime(TimeSpan.FromSeconds(progressSlider.Value));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            mediaPlayer.Close();
            base.OnClosed(e);
        }
    }
}