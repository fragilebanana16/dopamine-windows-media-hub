using Digimezzo.Foundation.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Digimezzo.Foundation.WPF.Controls;
using Dopamine.Core.Enums;
using Dopamine.Core.Prism;
using Dopamine.Views.FullPlayer.Collection;

namespace Dopamine.ViewModels.FullPlayer.Collection
{
    class CollectionHomeViewModel: BindableBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand LoadedCommand { get; set; }

        private HomeRegion? selectedPage;

        // string用来接收 CommandParameter
        public DelegateCommand<HomeRegion?> HomeNavigateCommand { get; }

        public HomeRegion? SelectedPage
        {
            get { return this.selectedPage; }
            set
            {
                SetProperty<HomeRegion?>(ref this.selectedPage, value);
                //SettingsClient.Set<int>("FullPlayer", "SelectedCollectionPage", (int)value);
                this.NagivateToSelectedPage(this.selectedPage);
            }
        }

        public CollectionHomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            HomeNavigateCommand = new DelegateCommand<HomeRegion?>(NagivateToSelectedPage);
            this.LoadedCommand = new DelegateCommand(() =>
            {
                if(this.SelectedPage == null)
                {
                    this.SelectedPage = HomeRegion.Overview;
                }
                this.NagivateToSelectedPage(this.SelectedPage);
            });
        }

        // 3. 执行导航
        private void NagivateToSelectedPage(HomeRegion? viewName)
        {
            if (!viewName.HasValue) 
            {
                return;
            }
            this.selectedPage = viewName;
            switch (this.selectedPage)
            {
                case HomeRegion.Overview:
                    this._regionManager.RequestNavigate(RegionNames.HomeMainContentRegion, typeof(CollectionHomeSubOverview).FullName);
                    break;
                case HomeRegion.Search:
                    this._regionManager.RequestNavigate(RegionNames.HomeMainContentRegion, typeof(CollectionHomeSubSearch).FullName);
                    break;

                default:
                    break;
            }
        }
    }
}
