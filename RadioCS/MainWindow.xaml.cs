using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Security.Cryptography.X509Certificates;
using NAudio.Wave;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Media;
using System.Net.Http;

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
                button.Content = "Start";

                return;
            }

            button.Content = "Stop";

            PlayStream("https://stream.r-a-d.io/main.mp3");

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

        private async void PlayStream(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();

            using (var fileStream = new FileStream("stream.mp3", FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            using var reader = new Mp3FileReader("stream.mp3");
            using var waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
        }
    }
}
