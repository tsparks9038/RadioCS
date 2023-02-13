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
using LibVLCSharp;
using LibVLCSharp.Shared;

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

            new Thread(PlayAudio).Start();
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

        private void PlayAudio()
        {
            // URL of the MP3 file
            string url = "https://relay0.r-a-d.io/main.mp3";

            // Buffer size in bytes
            int bufferSize = 4096;

            // Number of bytes to read from the MP3 file
            int chunkSize = 1024 * 1024; // 1 MB

            // Create a request to the URL
            WebRequest request = WebRequest.Create(url);

            // Get the response from the request
            WebResponse response = request.GetResponse();

            // Get the stream from the response
            Stream stream = response.GetResponseStream();

            // Create a buffer
            byte[] buffer = new byte[bufferSize];

            // Create a memory stream to store the MP3 data
            using (var memoryStream = new MemoryStream())
            {
                // Read the first chunk of the MP3 file
                int bytesRead = 0;
                int totalBytesRead = 0;
                while (totalBytesRead < chunkSize && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }

                // Reset the position of the memory stream
                memoryStream.Position = 0;

                // Create a wave stream for the MP3 data
                using (var waveStream = new Mp3FileReader(memoryStream))
                {
                    // Create a wave output device
                    using (var waveOut = new WaveOutEvent())
                    {
                        // Set the wave stream as the source for the wave output device
                        waveOut.Init(waveStream);

                        // Play the MP3 data
                        waveOut.Play();

                        // Wait for the playback to complete
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }

            // Close the response stream
            stream.Close();

            // Wait for user input
            Console.ReadLine();
        }
    }
}
