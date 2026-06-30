using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.FFME.Common;

namespace Dopamine.ViewModels.Control
{
    /// <summary>
    /// A base class for Root VM-attached view models.
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public abstract class AttachedViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedViewModel"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        protected AttachedViewModel(MoviePlayerViewModel root)
        {
            Root = root;
        }

        /// <summary>
        /// Gets the root VM this object belongs to.
        /// </summary>
        public MoviePlayerViewModel Root { get; }

        /// <summary>
        /// Called by the root ViewModel when the application is loaded and fully available.
        /// </summary>
        internal virtual void OnApplicationLoaded()
        {
            // placeholder
        }
    }
}
