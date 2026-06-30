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
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Dopamine.Views.FullPlayer.Video;
using Unosquare.FFME;
using Microsoft.Xaml.Behaviors;
using Dopamine.ViewModels.Control;
using DryIoc;
using System.Diagnostics;

namespace Dopamine.Controls
{
    /// <summary>
    /// Interaction logic for MoviePlayer.xaml
    /// </summary>
    public partial class MoviePlayer : UserControl
    {
        private Storyboard HideControllerAnimation => FindResource("HideControlOpacity") as Storyboard;

        private Storyboard ShowControllerAnimation => FindResource("ShowControlOpacity") as Storyboard;

        private DateTime LastMouseMoveTime;
        private Point LastMousePosition;
        private DispatcherTimer MouseMoveTimer;
        private bool IsControllerHideCompleted;

        public MoviePlayer()
        {
            InitializeComponent();
            Media.MediaFailed += Media_MediaFailed;
            Media.MediaOpened += Media_MediaOpened;


            Loaded += OnLoaded;
            InitializeInteraction();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MoviePlayerViewModel mpVm = (DataContext as MoviePlayerViewModel);
            if(mpVm != null)
            {
                mpVm.SetMediaElement(Media);
                mpVm.OnApplicationLoaded();
            }

            Debug.WriteLine($"MoviePlayer DataContext: {DataContext?.GetType().Name ?? "null"}");
        }
        #region Window Control and Input Event Handlers

        #endregion

        private void InitializeInteraction()
        {
 
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
