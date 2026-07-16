using Dopamine.Data.Entities;
using Prism.Mvvm;
using System;

namespace Dopamine.Services.Entities
{
    public class PasswordEntryViewModel : BindableBase
    {
        private readonly PasswordEntry _entry;   // 持有实体引用

        public PasswordEntryViewModel(PasswordEntry entry)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public long PasswordID => _entry.PasswordID;

        public string Title
        {
            get => _entry.Title;
            set
            {
                if (_entry.Title != value)
                {
                    _entry.Title = value;
                    RaisePropertyChanged(); // 自动通知属性名 "Title" 发生变化
                    UpdateTimestamp();
                }
            }
        }

        public string Account
        {
            get => _entry.Account;
            set
            {
                if (_entry.Account != value)
                {
                    _entry.Account = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                }
            }
        }

        // 实际密码值的存取
        public string Password
        {
            get => _entry.Password;
            set
            {
                if (_entry.Password != value)
                {
                    _entry.Password = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                    // 实际密码变了，显示用的 "DisplayPassword" 也要跟着刷新
                    RaisePropertyChanged(nameof(DisplayPassword));
                }
            }
        }

        // 密码在 VM 中做显示控制（明文/密文切换）
        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                // 这里 _showPassword 是 VM 里的私有字段，可以安全地使用 ref
                if (SetProperty(ref _showPassword, value))
                {
                    // 当 ShowPassword 变化时，必须通知 UI 重新读取 DisplayPassword
                    RaisePropertyChanged(nameof(DisplayPassword));
                }
            }
        }

        // 绑定到界面上显示的属性
        public string DisplayPassword => ShowPassword ? _entry.Password : "••••••••";

        public string Url
        {
            get => _entry.Url;
            set
            {
                if (_entry.Url != value)
                {
                    _entry.Url = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                }
            }
        }

        public string Type
        {
            get => _entry.Type;
            set
            {
                if (_entry.Type != value)
                {
                    _entry.Type = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                }
            }
        }

        public string Tags
        {
            get => _entry.Tags;
            set
            {
                if (_entry.Tags != value)
                {
                    _entry.Tags = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                }
            }
        }

        public string Notes
        {
            get => _entry.Notes;
            set
            {
                if (_entry.Notes != value)
                {
                    _entry.Notes = value;
                    RaisePropertyChanged();
                    UpdateTimestamp();
                }
            }
        }

        public string CreatedAt => _entry.CreatedAt;
        public string UpdatedAt => _entry.UpdatedAt;

        // 辅助方法
        private void UpdateTimestamp()
        {
            _entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            RaisePropertyChanged(nameof(UpdatedAt));
        }

        // 如果需要完整实体（保存时用）
        public PasswordEntry ToEntity() => _entry;

        // 深拷贝
        public PasswordEntryViewModel DeepCopy()
        {
            var newEntry = new PasswordEntry
            {
                PasswordID = this.PasswordID,
                Title = this.Title,
                Account = this.Account,
                Password = _entry.Password,
                Url = this.Url,
                Type = this.Type,
                Tags = this.Tags,
                Notes = this.Notes,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
            return new PasswordEntryViewModel(newEntry);
        }
    }
}