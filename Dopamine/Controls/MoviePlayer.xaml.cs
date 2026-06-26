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

namespace Dopamine.Controls
{
    /// <summary>
    /// Interaction logic for MoviePlayer.xaml
    /// </summary>
    public partial class MoviePlayer : UserControl
    {
        public MoviePlayer()
        {
            InitializeComponent();
            Media.MediaFailed += Media_MediaFailed;
            Media.MediaOpened += Media_MediaOpened;
        }

        private void Media_MediaFailed(object sender, Unosquare.FFME.Common.MediaFailedEventArgs e)
        {
            MessageBox.Show("Media Failed: " + e.ErrorException?.Message);
            System.Diagnostics.Debug.WriteLine("FFME Error: " + e.ErrorException);
        }

        private void Media_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            // 成功打开后可以在这里 Play()
            //Media.Play();
        }

        private async void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string filePath = "G:/test.mp4";

                // 1. 打开文件（异步）
                await Media.Open(new Uri(filePath));

                // 2. 开始播放
                Media.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("播放失败: " + ex.Message);
            }
        }
    }
}
