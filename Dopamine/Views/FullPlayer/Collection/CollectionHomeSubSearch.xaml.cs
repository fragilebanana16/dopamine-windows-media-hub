using Dopamine.ViewModels.FullPlayer.Collection;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using Digimezzo.Foundation.Core.Logging;
using Digimezzo.Foundation.WPF.Controls;
using Dopamine.Services.Entities;
using System.Collections;
using TagLib.Ape;
using Dopamine.Models;
using System.Collections.ObjectModel;

namespace Dopamine.Views.FullPlayer.Collection
{
    /// <summary>
    /// Interaction logic for CollectionHomeSubSearch.xaml
    /// </summary>
    public partial class CollectionHomeSubSearch : UserControl
    {
        private CollectionHomeSubSearchViewModel Vm => DataContext as CollectionHomeSubSearchViewModel;

        public CollectionHomeSubSearch()
        {
            InitializeComponent();
            ResultPanel.Visibility = Visibility.Collapsed;
            HomePanel.Visibility = Visibility.Visible;

            Loaded += OnLoaded;
        }

        private async void SearchItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            var item = (sender as Border)?.DataContext as SearchResultItem;
            if (item == null) return;
            var vm = DataContext as CollectionHomeSubSearchViewModel;
            vm?.PlayTrackCommand?.Execute(item);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Vm == null) return;
            Vm.PropertyChanged += OnVmPropertyChanged;
            Vm.NavigateToCategory += ScrollToCategory;

            // 分类下拉框选择后滚动定位
            CategoryComboBox.SelectionChanged += OnCategoryChanged;
        }

        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = CategoryComboBox.SelectedItem as ComboBoxItem;
            if (item == null) return;
            var key = item.Tag as string;
            if (string.IsNullOrEmpty(key)) return;  // 全部，不滚动
            ScrollToCategory(key);
        }

        private void OnVmPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Vm.IsResultMode))
                OnIsResultModeChanged(Vm.IsResultMode);
        }

        // ── 页面切换动画 ──────────────────────────────────────
        private void OnIsResultModeChanged(bool toResult)
        {
            if (toResult)
            {
                FadeOut(HomePanel, () =>
                {
                    HomePanel.Visibility = Visibility.Collapsed;
                    ResultPanel.Visibility = Visibility.Visible;
                    ResultPanel.Opacity = 0;
                    FadeIn(ResultPanel);
                });
            }
            else
            {
                FadeOut(ResultPanel, () =>
                {
                    ResultPanel.Visibility = Visibility.Collapsed;
                    HomePanel.Visibility = Visibility.Visible;
                    HomePanel.Opacity = 0;
                    FadeIn(HomePanel);
                });
            }
        }

        private static void FadeIn(UIElement el)
        {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            el.BeginAnimation(OpacityProperty, anim);
        }

        private static void FadeOut(UIElement el, Action onCompleted)
        {
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(160))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            anim.Completed += (s, args) => onCompleted();
            el.BeginAnimation(OpacityProperty, anim);
        }

        // ── 左侧导航滚动定位 ──────────────────────────────────
        private void ScrollToCategory(string categoryKey)
        {
            var itemsControl = FindVisualChild<ItemsControl>(ResultScroller);
            if (itemsControl == null) return;

            foreach (var item in itemsControl.Items)
            {
                var container = itemsControl.ItemContainerGenerator
                                            .ContainerFromItem(item) as FrameworkElement;
                if (container == null) continue;

                var border = FindVisualChild<Border>(container, b => b.Tag != null && b.Tag.ToString() == categoryKey);
                if (border == null) continue;

                var transform = border.TransformToAncestor(ResultScroller);
                var pos = transform.Transform(new Point(0, 0));
                ResultScroller.ScrollToVerticalOffset(ResultScroller.VerticalOffset + pos.Y - 16);
                break;
            }
        }

        // ── Visual 树查找 ─────────────────────────────────────
        private static T FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate = null)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && (predicate == null || predicate(t)))
                    return t;
                var result = FindVisualChild(child, predicate);
                if (result != null) return result;
            }
            return null;
        }
}
}
