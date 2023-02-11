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

            new Thread(getData).Start();
        }

        private void getData()
        {
            SongInfo info = new();
            long end, curr, timeLeft;
            string[] nameArt;

            while (true)
            {
                nameArt = info.GetSongName().Split(" - ");

                this.Dispatcher.Invoke(() =>
                {
                    songName.Text = nameArt[1];
                    artistName.Text = nameArt[0];
                });

                end = info.GetEndTime();
                curr = info.GetCurrent();

                timeLeft = end - curr;

                DateTime defaultUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                DateTime timeUnix = defaultUnix.AddSeconds(timeLeft).ToLocalTime();

                DateTimeOffset timeMill = timeUnix;

                int timeInt = (int)timeMill.ToUnixTimeMilliseconds();

                Thread.Sleep(timeInt + 2000);
            }
        }
    }
}
