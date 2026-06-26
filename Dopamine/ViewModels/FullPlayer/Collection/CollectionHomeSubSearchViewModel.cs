using Digimezzo.Foundation.Core.IO;
using Digimezzo.Foundation.Core.Logging;
using Dopamine.Data.Entities;
using Dopamine.Data.Repositories;
using Dopamine.Models;
using Dopamine.Services.Entities;
using Dopamine.Services.Extensions;
using Dopamine.Services.Playback;
using Dopamine.Services.Search;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dopamine.ViewModels.FullPlayer.Collection
{
    class CollectionHomeSubSearchViewModel : BindableBase
    {

        private string searchText;
        private ITrackRepository trackRepository;
        private IPlaybackService playbackService;

        // ── 状态 ──────────────────────────────────────────────
        private bool _isResultMode;
        public bool IsResultMode
        {
            get => _isResultMode;
            set => SetProperty(ref _isResultMode, value);
        }

        private bool _isHistoryVisible;
        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set => SetProperty(ref _isHistoryVisible, value);
        }

        private bool _showHistory = true;
        public bool ShowHistory
        {
            get => _showHistory;
            set
            {
                SetProperty(ref _showHistory, value);
                RaisePropertyChanged(nameof(ShowHistoryHint)); // 反向属性
            }
        }
        // Inverse，XAML 直接绑这个，不需要 Converter
        public bool ShowHistoryHint => !_showHistory;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                RaisePropertyChanged(nameof(HasText));
                if (!string.IsNullOrWhiteSpace(value) && ShowHistory && SearchHistory.Count > 0)
                    IsHistoryVisible = true;
                else
                    IsHistoryVisible = false;
            }
        }

        public bool HasText => !string.IsNullOrEmpty(_searchText);

        // ── 数据 ──────────────────────────────────────────────
        public ObservableCollection<string> SearchHistory { get; } = new ObservableCollection<string>()
        {
            "周杰伦",
            "星际穿越",
            "项目计划书",
            "Python教程",
            "晚霞摄影"
        };

        public ObservableCollection<SearchResultGroup> ResultGroups { get; } = new ObservableCollection<SearchResultGroup>();

        // ── Commands ──────────────────────────────────────────
        public ICommand SearchCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SelectHistoryCommand { get; }
        public ICommand DeleteHistoryItemCommand { get; }
        public ICommand ToggleHistoryCommand { get; }
        public ICommand ClearCommand { get; }

        public ICommand PlayTrackCommand { get; }
        public ICommand ViewInExplorerCommand { get; }

        // 左侧导航：通知 code-behind 滚动，用事件传出 CategoryKey
        public event Action<string> NavigateToCategory;
        public ICommand NavToCategoryCommand { get; }

        private IContainerProvider container;

        public CollectionHomeSubSearchViewModel(ITrackRepository trackRepository, IPlaybackService playbackService, IContainerProvider container)
        {
            this.container = container;
            SearchCommand = new DelegateCommand(ExecuteSearch, () => HasText)
                                           .ObservesProperty(() => SearchText);
            BackCommand = new DelegateCommand(ExecuteBack);
            SelectHistoryCommand = new DelegateCommand<string>(SelectHistory);
            DeleteHistoryItemCommand = new DelegateCommand<string>(item => SearchHistory.Remove(item));
            ToggleHistoryCommand = new DelegateCommand(() => { ShowHistory = !ShowHistory; IsHistoryVisible = false; });
            ClearCommand = new DelegateCommand(() => SearchText = string.Empty);
            NavToCategoryCommand = new DelegateCommand<string>(key => NavigateToCategory?.Invoke(key));
            PlayTrackCommand = new DelegateCommand<SearchResultItem>(ExecutePlayTrack);

            this.ViewInExplorerCommand = new DelegateCommand<SearchResultItem>(ViewInExplorer);

            this.playbackService = playbackService;
            this.trackRepository = trackRepository;
        }

        private void ViewInExplorer(SearchResultItem item)
        {
            if (item == null) return;

            try
                {
                if (item.Tag is Track track)
                {
                    Actions.TryViewInExplorer(track.Path);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("Could not view track in Windows Explorer. Exception: {0}", ex.Message);
            }
        }

        private async void ExecutePlayTrack(SearchResultItem item)
        {
            if(item == null) return;
            if (item.Tag is Track track)
            {
                var trackViewModel = this.container.ResolveTrackViewModel(track);
                // todo 队列更新
                await this.playbackService.PlaySelectedAsync(trackViewModel);
            }
        }


        // ── 搜索 ──────────────────────────────────────────────
        private async void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return;
            IsHistoryVisible = false;

            // 历史去重置顶
            if (SearchHistory.Contains(SearchText))
                SearchHistory.Remove(SearchText);
            SearchHistory.Insert(0, SearchText);

            var tracks = await trackRepository.SearchTracksAsync(SearchText);

            BuildMockResults(SearchText, tracks);
            IsResultMode = true;
        }

        private void SelectHistory(string item)
        {
            SearchText = item;
            ExecuteSearch();
        }

        private void ExecuteBack()
        {
            IsResultMode = false;
            IsHistoryVisible = false;
        }

        // ── Mock 数据 ─────────────────────────────────────────
        private void BuildMockResults(string q, List<Track> tracks)
        {
            ResultGroups.Clear();

            // tracks
            if(tracks != null && tracks.Count > 0)
            {
                var trackItems = tracks.Select(track => new SearchResultItem
                {
                    Title = !string.IsNullOrEmpty(track.FileName) ? track.FileName : "未知曲目",
                    Subtitle = $"{track.Artists ?? "未知艺术家"} · {track.AlbumTitle ?? "未知专辑"}",
                    Icon = Convert.ToString("\uEC4F"), // music icon
                    Tag = track,
                }).ToList();

                ResultGroups.Add(new SearchResultGroup
                {
                    CategoryKey = "Music",
                    CategoryName = "音乐",
                    CategoryIcon = Convert.ToString("\uEC4F"),
                    Items = new ObservableCollection<SearchResultItem>(trackItems)
                });
            }
            

            ResultGroups.Add(new SearchResultGroup
            {
                CategoryKey = "Video",
                CategoryName = "影视",
                CategoryIcon = "▶",
                Items = new ObservableCollection<SearchResultItem>
                {
                    new SearchResultItem() { Title = $"{q}：纪录片",    Subtitle = "2023 · 1h 42min · 豆瓣 8.9", Icon = "🎬" },
                    new SearchResultItem() { Title = $"{q}的故事 全集", Subtitle = "剧集 · 全 24 集",              Icon = "📺" },
                }
            });

            ResultGroups.Add(new SearchResultGroup
            {
                CategoryKey = "Note",
                CategoryName = "笔记",
                CategoryIcon = "✎",
                Items = new ObservableCollection<SearchResultItem>
                {
                    new SearchResultItem() { Title = $"关于「{q}」的读书笔记", Subtitle = "上次修改：2 天前", Icon = "📝" },
                    new SearchResultItem() { Title = $"{q} 项目会议记录",      Subtitle = "上次修改：1 周前", Icon = "📋" },
                }
            });

            ResultGroups.Add(new SearchResultGroup
            {
                CategoryKey = "File",
                CategoryName = "文件",
                CategoryIcon = "◻",
                Items = new ObservableCollection<SearchResultItem>
                {
                    new SearchResultItem() { Title = $"{q}_final.pdf",   Subtitle = "2.3 MB · 桌面",  Icon = "📄" },
                    new SearchResultItem() { Title = $"{q}_资料包.zip",  Subtitle = "15.6 MB",         Icon = "🗜" },
                }
            });

            ResultGroups.Add(new SearchResultGroup
            {
                CategoryKey = "Contact",
                CategoryName = "联系人",
                CategoryIcon = "👤",
                Items = new ObservableCollection<SearchResultItem>
                {
                    new SearchResultItem() { Title = $"{q}（同事）",   Subtitle = "手机：138****8888", Icon = "👤" },
                    new SearchResultItem() { Title = $"{q} 粉丝群",    Subtitle = "微信群 · 128 人",   Icon = "💬" },
                }
            });
        }
    }
}