using Dopamine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.FFME;

namespace Dopamine.ViewModels.Control
{
    public class MoviePlayerViewModel
    {
        /// <summary>
        /// Provides access to application-wide commands.
        /// </summary>
        public PlayControlCommand Commands { get; }

        public MoviePlayerViewModel()
        {
            Commands = new PlayControlCommand(() => MediaElement);
        }

        private MediaElement m_MediaElement;

        public void SetMediaElement(MediaElement mediaElement)
        {
            m_MediaElement = mediaElement;
        }

        /// <summary>
        /// Gets the media element hosted by the main window.
        /// </summary>
        public MediaElement MediaElement => m_MediaElement;
    }
}
