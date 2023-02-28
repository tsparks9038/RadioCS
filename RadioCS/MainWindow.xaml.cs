using System;
using System.Windows;
using System.Threading;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace RadioCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (button.Content.ToString() == "Stop")
            {
                button.Content = "Play";

                return;
            }

            button.Content = "Stop";

            PlayAudio("https://relay0.r-a-d.io/main.mp3");
            new Thread(GetData).Start();
        }

        private void GetData()
        {
            SongInfo info = new();
            string[] nameArt;
            long curr, end;

            while (true)
            {
                nameArt = info.GetSongName().Split(" - ");

                curr = info.GetCurrent() - info.GetStartTime();
                end = info.GetEndTime() - info.GetStartTime();

                TimeSpan currT = TimeSpan.FromSeconds(curr);
                TimeSpan endT = TimeSpan.FromSeconds(end);

                string currStr = currT.ToString(@"mm\:ss");
                string endStr = endT.ToString(@"mm\:ss");

                this.Dispatcher.Invoke(() => 
                {
                    songName.Text = nameArt[1];
                    artistName.Text = nameArt[0];
                    currTime.Text = currStr;
                    endTime.Text = endStr;
                });
            }
        }

        //DO NOT DELETE, IT KIND OF WORKS!!!!!!!
        private void PlayAudio(string audioFilePath)
        {
            var url = "https://relay0.r-a-d.io/main.mp3";
            var wc = new WebClient();
            var stream = wc.OpenRead(url);

            var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
            ffmpegProcess.StartInfo.Arguments = "-i - -f wav -";
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardInput = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.CreateNoWindow = true;
            ffmpegProcess.Start();

            Task.Factory.StartNew(() =>
            {
                var buffer = new byte[4096];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ffmpegProcess.StandardInput.BaseStream.Write(buffer, 0, read);
                }

                ffmpegProcess.StandardInput.Close();
            });

            var waveFormat = new WaveFormat(44100, 16, 2);
            var waveOut = new WaveOutEvent();
            var bufferedWaveProvider = new BufferedWaveProvider(waveFormat);
            bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(30);
            waveOut.Init(bufferedWaveProvider);

            Task.Factory.StartNew(() =>
            {
                var buffer = new byte[4096];
                int read;
                while ((read = ffmpegProcess.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferedWaveProvider.AddSamples(buffer, 0, read);
                }

                waveOut.Stop();
                ffmpegProcess.Dispose();
            });

            waveOut.Play();
        }
    }
}
