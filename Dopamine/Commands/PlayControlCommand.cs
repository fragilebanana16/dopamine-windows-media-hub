using Dopamine.Utils;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unosquare.FFME;
using Windows.Storage.Streams;

namespace Dopamine.Commands
{
    /// <summary>
    /// Represents the Application-Wide Commands.
    /// </summary>
    public class PlayControlCommand
    {
        #region Private State

        private readonly WindowStatus PreviousWindowStatus = new WindowStatus();

        private DelegateCommand<string> m_OpenCommand;
        private DelegateCommand<string> m_PauseCommand;
        private DelegateCommand<string> m_PlayCommand;
        private DelegateCommand<string> m_StopCommand;
        private DelegateCommand<string> m_CloseCommand;
        private DelegateCommand<string> m_ToggleFullscreenCommand;
        private DelegateCommand<string> m_RemovePlaylistItemCommand;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommands"/> class.
        /// </summary>

        private readonly Func<MediaElement> _getMedia;

        public PlayControlCommand(Func<MediaElement> getMedia)
        {
            _getMedia = getMedia;
        }
        #endregion

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand<string> OpenCommand => m_OpenCommand ??
             (m_OpenCommand = new DelegateCommand<string>(async uriString =>
             {
                 try
                 {
                     if (string.IsNullOrWhiteSpace(uriString))
                         return;
                     var m = _getMedia();
                     var target = new Uri(uriString);
                     if (target.ToString().StartsWith(Utils.FileInputStream.Scheme, StringComparison.OrdinalIgnoreCase))
                         await m.Open(new Utils.FileInputStream(target.LocalPath));
                     else
                         await m.Open(target);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show(
                         Application.Current.MainWindow,
                         $"Media Failed: {ex.GetType()}\r\n{ex.Message}",
                         $"{nameof(MediaElement)} Error",
                         MessageBoxButton.OK,
                         MessageBoxImage.Error,
                         MessageBoxResult.OK);
                 }
             }));

        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public DelegateCommand<string> CloseCommand => m_CloseCommand ??
            (m_CloseCommand = new DelegateCommand<string>(async o =>
            {
                var m = _getMedia();
                await m.Close();
                // or, you can totally dispose manually:
                // App.ViewModel.MediaElement.Dispose();
            }));

        /// <summary>
        /// Gets the pause command.
        /// </summary>
        /// <value>
        /// The pause command.
        /// </value>
        public DelegateCommand<string> PauseCommand => m_PauseCommand ??
            (m_PauseCommand = new DelegateCommand<string>(async o =>
            {
                var m = _getMedia();
                await m.Pause();
            }));

        /// <summary>
        /// Gets the play command.
        /// </summary>
        /// <value>
        /// The play command.
        /// </value>
        public DelegateCommand<string> PlayCommand => m_PlayCommand ??
            (m_PlayCommand = new DelegateCommand<string>(async o =>
            {
                // await Current.MediaElement.Seek(TimeSpan.Zero)
                var m = _getMedia();
                await m.Play();
            }));

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public DelegateCommand<string> StopCommand => m_StopCommand ??
            (m_StopCommand = new DelegateCommand<string>(async o =>
            {
                var m = _getMedia();
                await m.Stop();
            }));

        /// <summary>
        /// Gets the toggle fullscreen command.
        /// </summary>
        /// <value>
        /// The toggle fullscreen command.
        /// </value>
        public DelegateCommand<string> ToggleFullscreenCommand => m_ToggleFullscreenCommand ??
            (m_ToggleFullscreenCommand = new DelegateCommand<string>(o =>
            {
                var mainWindow = Application.Current.MainWindow;

                // If we are already in fullscreen, go back to normal
                if (mainWindow.WindowStyle == WindowStyle.None)
                {
                    PreviousWindowStatus.Apply(mainWindow);
                    WindowStatus.EnableDisplayTimeout();
                }
                else
                {
                    PreviousWindowStatus.Capture(mainWindow);
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.ResizeMode = ResizeMode.NoResize;
                    mainWindow.Topmost = true;
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.WindowState = WindowState.Maximized;
                    WindowStatus.DisableDisplayTimeout();
                }
            }));

        /// <summary>
        /// Gets the remove playlist item command.
        /// </summary>
        /// <value>
        /// The remove playlist item command.
        /// </value>
        public DelegateCommand<string> RemovePlaylistItemCommand => m_RemovePlaylistItemCommand ??
            (m_RemovePlaylistItemCommand = new DelegateCommand<string>(arg =>
            {
                //if (arg is CustomPlaylistEntry entry)
                //{
                //    if (Uri.TryCreate(entry.MediaSource, UriKind.RelativeOrAbsolute, out var mediaSource))
                //    {
                //        App.ViewModel.Playlist.Entries.RemoveEntryByMediaSource(mediaSource);
                //        App.ViewModel.Playlist.Entries.SaveEntries();
                //    }
                //}
            }));
    }
}
