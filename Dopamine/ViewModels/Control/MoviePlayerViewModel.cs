using Dopamine.Commands;
using Dopamine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.FFME;
using Unosquare.FFME.Common;
using Windows.Media.Playlists;

namespace Dopamine.ViewModels.Control
{
    public class MoviePlayerViewModel: ViewModelBase
    {
        /// <summary>
        /// Provides access to application-wide commands.
        /// </summary>
        public PlayControlCommand Commands { get; }
        private string m_NotificationMessage = string.Empty;

        /// <summary>
        /// Gets or sets the notification message to be displayed.
        /// </summary>
        public string NotificationMessage
        {
            get
            {
                return m_NotificationMessage;
            }
            set
            {
                m_NotificationMessage = value;
                NotifyPropertyChanged(nameof(NotificationMessage));
            }
        }
        /// <summary>
        /// Provides access to the current media options.
        /// This is an unsupported usage of media options.
        /// </summary>
        public MediaOptions CurrentMediaOptions { get; set; }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        public ControllerViewModel Controller { get; }

        private MediaElement m_MediaElement;

        public void SetMediaElement(MediaElement mediaElement)
        {
            m_MediaElement = mediaElement;
        }

        /// <summary>
        /// Gets the media element hosted by the main window.
        /// </summary>
        public MediaElement MediaElement => m_MediaElement;

        private bool m_IsApplicationLoaded = App.IsInDesignMode;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is application loaded.
        /// </summary>
        public bool IsApplicationLoaded
        {
            get => m_IsApplicationLoaded;
            set => SetProperty(ref m_IsApplicationLoaded, value);
        }

        public MoviePlayerViewModel()
        {
            Commands = new PlayControlCommand(() => MediaElement);
            Controller = new ControllerViewModel(this);
        }


        /// <summary>
        /// Called when application has finished loading.
        /// </summary>
        internal void OnApplicationLoaded()
        {
            if (IsApplicationLoaded)
                return;

            //Playlist.OnApplicationLoaded();
            Controller.OnApplicationLoaded();

            var m = MediaElement;
            //m.WhenChanged(UpdateWindowTitle,
            //    nameof(m.IsOpen),
            //    nameof(m.IsOpening),
            //    nameof(m.MediaState),
            //    nameof(m.Source));

            m.MediaOpened += (s, e) =>
            {
                // Reset the Zoom
                Controller.MediaElementZoom = 1d;

                // Update the Controls
                //Playlist.IsInOpenMode = false;
                //IsPlaylistPanelOpen = false;
                //Playlist.OpenMediaSource = e.Info.MediaSource;
            };

            //IsPlaylistPanelOpen = true;
            IsApplicationLoaded = true;
        }
    }
}
