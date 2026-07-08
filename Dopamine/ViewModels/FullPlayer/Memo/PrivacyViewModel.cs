using Dopamine.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Data;

namespace Dopamine.ViewModels.FullPlayer.Memo
{
    class PrivacyViewModel : BindableBase
    {
        private ObservableCollection<PasswordItem> _allItems;
        public ICollectionView ItemsView { get; }

        // 当前选中的项（用于详情展示）
        private PasswordItem _selectedItem;
        public PasswordItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    IsEditMode = false;
                    // 切换选中时，深拷贝一份用于编辑，防止未保存直接修改列表
                    EditingItem = value != null ? CloneItem(value) : null;
                }
            }
        }

        // 当前正在编辑/新增的临时项
        private PasswordItem _editingItem;
        public PasswordItem EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }

        // 搜索文本
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ItemsView.Refresh();
            }
        }

        // 控制右侧是“只读/编辑”状态
        private bool _isEditMode;
        public bool IsEditMode { get => _isEditMode; set => SetProperty(ref _isEditMode, value); }

        // 核心交互命令
        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public PrivacyViewModel()
        {
            // 1. 初始化模拟后台数据 (查)
            _allItems = new ObservableCollection<PasswordItem>
        {
            new PasswordItem { Id = 1, Title = "微信", Category = "社交", Username = "wang_xiaoming", Password = "password123", Url = "weixin.qq.com", Notes = "主要社交账号，绑定手机 138****8888", LastUpdated = DateTime.Parse("2024-11-20") },
            new PasswordItem { Id = 2, Title = "招商银行", Category = "金融", Username = "6225 **** **** 8821", Password = "bankpassword", Url = "cmbchina.com", Notes = "工资卡", LastUpdated = DateTime.Parse("2024-10-15") },
            new PasswordItem { Id = 3, Title = "GitHub", Category = "工作", Username = "wangxm-dev", Password = "gitpassword", Url = "github.com", Notes = "个人开发账号", LastUpdated = DateTime.Parse("2025-01-02") }
        };

            ItemsView = CollectionViewSource.GetDefaultView(_allItems);
            ItemsView.Filter = FilterItems;

            // 2. 命令绑定
            AddCommand = new DelegateCommand(ExecuteAdd);
            EditCommand = new DelegateCommand(ExecuteEdit, () => SelectedItem != null).ObservesProperty(() => SelectedItem);
            SaveCommand = new DelegateCommand(ExecuteSave, () => EditingItem != null).ObservesProperty(() => EditingItem);
            DeleteCommand = new DelegateCommand(ExecuteDelete, () => SelectedItem != null).ObservesProperty(() => SelectedItem);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool FilterItems(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            var item = obj as PasswordItem;
            return item != null && (item.Title.Contains(SearchText) ||
                                   item.Username.Contains(SearchText));
        }

        // 【增】点击添加按钮
        private void ExecuteAdd()
        {
            SelectedItem = null;
            EditingItem = new PasswordItem { Id = 0, Category = "社交", LastUpdated = DateTime.Now };
            IsEditMode = true;
        }

        // 【改】进入编辑状态
        private void ExecuteEdit()
        {
            IsEditMode = true;
        }

        // 【改/增】保存逻辑
        private void ExecuteSave()
        {
            if (EditingItem == null) return;

            EditingItem.LastUpdated = DateTime.Now;

            if (EditingItem.Id == 0) // 新增
            {
                EditingItem.Id = _allItems.Any() ? _allItems.Max(i => i.Id) + 1 : 1;
                _allItems.Add(EditingItem);
                SelectedItem = EditingItem;
            }
            else // 修改更新
            {
                var original = _allItems.FirstOrDefault(i => i.Id == EditingItem.Id);
                if (original != null)
                {
                    original.Title = EditingItem.Title;
                    original.Category = EditingItem.Category;
                    original.Username = EditingItem.Username;
                    original.Password = EditingItem.Password;
                    original.Url = EditingItem.Url;
                    original.Notes = EditingItem.Notes;
                    original.LastUpdated = EditingItem.LastUpdated;
                }
            }
            IsEditMode = false;
            ItemsView.Refresh();
        }

        // 【删】删除逻辑
        private void ExecuteDelete()
        {
            if (SelectedItem == null) return;
            _allItems.Remove(SelectedItem);
            SelectedItem = _allItems.FirstOrDefault();
        }

        // 取消编辑
        private void ExecuteCancel()
        {
            IsEditMode = false;
            EditingItem = SelectedItem != null ? CloneItem(SelectedItem) : null;
        }

        private PasswordItem CloneItem(PasswordItem source)
        {
            return new PasswordItem
            {
                Id = source.Id,
                Title = source.Title,
                Category = source.Category,
                Url = source.Url,
                Username = source.Username,
                Password = source.Password,
                Notes = source.Notes,
                LastUpdated = source.LastUpdated
            };
        }
    }
}
