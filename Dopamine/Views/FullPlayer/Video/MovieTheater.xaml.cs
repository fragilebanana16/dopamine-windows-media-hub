using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Controls.Maps;

namespace Dopamine.Views.FullPlayer.Video
{
    /// <summary>
    /// Interaction logic for VideoHome.xaml
    /// </summary>
    public partial class MovieTheater : UserControl
    {
        public ObservableCollection<VideoModel> Videos { get; set; }

        public MovieTheater()
        {
            InitializeComponent();
            LoadMockData();

            // 绑定两个列表的数据源
            VideoListBox.ItemsSource = Videos;
            SideVideoListBox.ItemsSource = Videos;
        }

        private void LoadMockData()
        {
            Videos = new ObservableCollection<VideoModel>
            {
                new VideoModel { Title = "Iceland Northern Lights Timelapse", Duration = "4:32", Size = "1.2 GB", Format = "MP4", ModifiedDate = "Mar 15, 2024", Resolution = "4K", ThumbnailUrl = "pack://application:,,,/Images/iceland.jpg" },
                new VideoModel { Title = "Tokyo Street Night Walk", Duration = "12:45", Size = "2.8 GB", Format = "MKV", ModifiedDate = "Feb 28, 2024", Resolution = "4K", ThumbnailUrl = "pack://application:,,,/Images/tokyo.jpg" },
                new VideoModel { Title = "Mountain Hiking Documentary", Duration = "38:20", Size = "5.4 GB", Format = "MP4", ModifiedDate = "Jan 10, 2024", Resolution = "4K", ThumbnailUrl = "pack://application:,,,/Images/mountain.jpg" }
                // ... 依此添加其它卡片数据
            };
        }

        // 核心：点击主页面视频，切到播放页
        private void VideoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoListBox.SelectedItem is VideoModel selectedVideo)
            {
                SwitchToPlayerPage(selectedVideo);
                VideoListBox.SelectedItem = null; // 清空选择状态
            }
        }

        // 核心：在播放页点击右侧“下一个”
        private void SideVideoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SideVideoListBox.SelectedItem is VideoModel selectedVideo)
            {
                SwitchToPlayerPage(selectedVideo);
            }
        }

        private void SwitchToPlayerPage(VideoModel video)
        {
            // 1. 界面轮换
            BrowserPage.Visibility = Visibility.Collapsed;
            PlayerPage.Visibility = Visibility.Visible;

            // 2. 联动更新右侧面板文字
            PlayingTitle.Text = video.Title;
            DetailTitle.Text = video.Title;
            DetailDuration.Text = video.Duration;
            DetailSize.Text = video.Size;
            DetailFormat.Text = video.Format;
            DetailDate.Text = video.ModifiedDate;

            // 3. 播放视频（这里绑定本地绝对路径，如 video.FullPath）
            // MainPlayer.Source = new Uri(video.FullPath);
            // MainPlayer.Play();
        }

        // 核心：从播放页退回到浏览界面
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // 界面切回
            PlayerPage.Visibility = Visibility.Collapsed;
            BrowserPage.Visibility = Visibility.Visible;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            // 简单的播放/暂停逻辑切换
        }
    }

    public class VideoModel
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public string Size { get; set; }
        public string Format { get; set; }
        public string ModifiedDate { get; set; }
        public string Resolution { get; set; }
        public string ThumbnailUrl { get; set; } // 缩略图路径
        public string FullPath { get; set; }     // 本地视频真实路径
    }
}
