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
        public ObservableCollection<PasswordItem> Accounts { get; } = new ObservableCollection<PasswordItem>();
        public List<string> Categories { get; set; }
        private ICollectionView _accountsView;
        public ICollectionView AccountsView => _accountsView;

        private PasswordItem _selectedAccount;
        /// <summary>列表中当前选中的条目（未编辑前的"原始态"引用）</summary>
        public PasswordItem SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                // 切换选中项前，如果正处于编辑态且未保存，提示或自动放弃编辑
                if (IsEditing && _selectedAccount != null && !ReferenceEquals(value, _selectedAccount))
                {
                    CancelEditInternal();
                }

                if (SetProperty(ref _selectedAccount, value))
                {
                    RaisePropertyChanged(nameof(HasSelection));
                    LoadDetailFromSelected();
                    RaiseCommandsCanExecuteChanged();
                }
            }
        }

        public bool HasSelection => SelectedAccount != null;

        // ---------------- 右侧详情表单的可编辑字段（编辑态草稿，与列表原始数据分离） ----------------

        private PasswordItem _draft;
        /// <summary>正在编辑/查看的草稿对象，UI 绑定到这个对象上的字段</summary>
        public PasswordItem Draft
        {
            get => _draft;
            set => SetProperty(ref _draft, value);
        }

        private bool _isEditing;
        /// <summary>是否处于编辑态：true=表单可写，false=只读展示</summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    RaiseCommandsCanExecuteChanged();
                }
            }
        }

        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _accountsView.Refresh();
                }
            }
        }

        // ---------------- Commands ----------------

        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelEditCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand ToggleShowPasswordCommand { get; }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    OnPropertyChanged(nameof(SelectedCategory));
                    AccountsView?.Refresh();
                }
            }
        }
        public PrivacyViewModel()
        {
            // 初始化测试分类
            Categories = new List<string> { "全部", "社交", "金融", "图书音像", "户外运动", "食品生鲜", "服装鞋帽" };
            // 模拟数据
            Accounts.Add(new PasswordItem
            {
                Title = "微信",
                Category = "社交",
                Account = "wang_xiaoming",
                Password = "Wx@123456",
                Url = "qq.com",
                Remark = "常用账号",
                UpdatedAt = new DateTime(2024, 11, 20, 14, 23, 1)
            });
            Accounts.Add(new PasswordItem
            {
                Title = "招商银行",
                Category = "金融",
                Account = "6225********8821",
                Password = "Bank#9527",
                Url = "cmbchina.com",
                Remark = "",
                UpdatedAt = new DateTime(2024, 10, 2, 9, 0, 0)
            });

            _accountsView = CollectionViewSource.GetDefaultView(Accounts);
            _accountsView.Filter = FilterAccounts;

            AddCommand = new DelegateCommand(ExecuteAdd);
            EditCommand = new DelegateCommand(ExecuteEdit, CanEdit);
            SaveCommand = new DelegateCommand(ExecuteSave, CanSave);
            CancelEditCommand = new DelegateCommand(ExecuteCancelEdit, CanCancelEdit);
            DeleteCommand = new DelegateCommand(ExecuteDelete, CanDelete);
            ToggleShowPasswordCommand = new DelegateCommand(() => ShowPassword = !ShowPassword);

            if (Accounts.Count > 0)
            {
                SelectedAccount = Accounts[0];
            }
        }

        private bool FilterAccounts(object obj)
        {
            if (obj is PasswordItem item)
            {
                // 1. 验证【标签分类】条件 (若为"全部"或为空，则该条件直接通关)
                bool matchesCategory = string.IsNullOrEmpty(SelectedCategory) ||
                                       SelectedCategory == "全部" ||
                                       item.Category == SelectedCategory;

                // 2. 验证【搜索框文字】条件 (若为空，则该条件直接通关)
                bool matchesSearchText = string.IsNullOrWhiteSpace(SearchText) ||
                                         (item.Title?.Contains(SearchText) ?? false) ||
                                         (item.Account?.Contains(SearchText) ?? false);

                // 3. 必须同时满足 标签 且 满足 搜索文字 才能显示
                return matchesCategory && matchesSearchText;
            }

            return false;
        }

        private void LoadDetailFromSelected()
        {
            ShowPassword = false;
            if (SelectedAccount == null)
            {
                Draft = null;
                IsEditing = false;
                return;
            }

            // 深拷贝到草稿，避免只读展示阶段直接改到列表数据源
            Draft = SelectedAccount.Clone();

            // 如果是刚添加的新条目，直接进入编辑态；否则只读展示
            IsEditing = SelectedAccount.IsNew;
        }

        // ---------------- 添加 ----------------

        private void ExecuteAdd()
        {
            var newItem = new PasswordItem
            {
                Title = "新账号",
                Category = "未分类",
                UpdatedAt = DateTime.Now,
                IsNew = true
            };

            Accounts.Insert(0, newItem);
            _accountsView.Refresh();

            // 选中新条目 -> 触发 LoadDetailFromSelected -> 因为 IsNew=true 自动进入编辑态
            SelectedAccount = newItem;
        }

        // ---------------- 编辑 ----------------

        private bool CanEdit() => HasSelection && !IsEditing;

        private void ExecuteEdit()
        {
            if (SelectedAccount == null) return;
            Draft = SelectedAccount.Clone();
            IsEditing = true;
        }

        // ---------------- 保存 ----------------

        private bool CanSave() => IsEditing && Draft != null && !string.IsNullOrWhiteSpace(Draft.Title);

        private void ExecuteSave()
        {
            if (SelectedAccount == null || Draft == null) return;

            Draft.UpdatedAt = DateTime.Now;
            Draft.IsNew = false;

            // 把草稿写回列表中的真实对象，列表项因为属性是 BindableBase 会自动刷新显示
            SelectedAccount.CopyFrom(Draft);

            IsEditing = false;
            _accountsView.Refresh();
            RaiseCommandsCanExecuteChanged();
        }

        // ---------------- 取消编辑 ----------------

        private bool CanCancelEdit() => IsEditing;

        private void ExecuteCancelEdit() => CancelEditInternal();

        private void CancelEditInternal()
        {
            if (SelectedAccount == null) return;

            if (SelectedAccount.IsNew)
            {
                // 新建但取消 -> 直接从列表移除这条未保存记录
                var toRemove = SelectedAccount;
                int idx = Accounts.IndexOf(toRemove);
                Accounts.Remove(toRemove);

                IsEditing = false;
                _selectedAccount = null; // 避免再次触发 CancelEditInternal
                RaisePropertyChanged(nameof(SelectedAccount));

                if (Accounts.Count > 0)
                {
                    SelectedAccount = Accounts[Math.Min(idx, Accounts.Count - 1)];
                }
                else
                {
                    Draft = null;
                    RaisePropertyChanged(nameof(HasSelection));
                }
            }
            else
            {
                // 已有记录，取消 -> 草稿还原为原始数据，回到只读态
                Draft = SelectedAccount.Clone();
                IsEditing = false;
            }

            RaiseCommandsCanExecuteChanged();
        }

        // ---------------- 删除 ----------------

        private bool CanDelete() => HasSelection;

        private void ExecuteDelete()
        {
            if (SelectedAccount == null) return;

            int idx = Accounts.IndexOf(SelectedAccount);
            Accounts.Remove(SelectedAccount);

            _selectedAccount = null;
            RaisePropertyChanged(nameof(SelectedAccount));

            if (Accounts.Count > 0)
            {
                SelectedAccount = Accounts[Math.Min(idx, Accounts.Count - 1)];
            }
            else
            {
                Draft = null;
                IsEditing = false;
                RaisePropertyChanged(nameof(HasSelection));
            }

            _accountsView.Refresh();
        }

        private void RaiseCommandsCanExecuteChanged()
        {
            EditCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }
}
