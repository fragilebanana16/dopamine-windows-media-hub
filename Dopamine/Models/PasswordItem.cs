using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Models
{
    public class PasswordItem : BindableBase
    {
        private string _id = Guid.NewGuid().ToString();
        public string Id { get => _id; set => SetProperty(ref _id, value); }

        private string _title = "";
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _category = "";
        public string Category { get => _category; set => SetProperty(ref _category, value); }

        private string _account = "";
        public string Account { get => _account; set => SetProperty(ref _account, value); }

        private string _password = "";
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _url = "";
        public string Url { get => _url; set => SetProperty(ref _url, value); }

        private string _remark = "";
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        private DateTime _updatedAt = DateTime.Now;
        public DateTime UpdatedAt { get => _updatedAt; set => SetProperty(ref _updatedAt, value); }

        private bool _isNew;
        /// <summary>标记是新建但还未保存的条目，用于列表上做"未保存"角标之类的视觉区分</summary>
        public bool IsNew { get => _isNew; set => SetProperty(ref _isNew, value); }

        public PasswordItem Clone()
        {
            return new PasswordItem
            {
                Id = this.Id,
                Title = this.Title,
                Category = this.Category,
                Account = this.Account,
                Password = this.Password,
                Url = this.Url,
                Remark = this.Remark,
                UpdatedAt = this.UpdatedAt,
                IsNew = this.IsNew
            };
        }

        public void CopyFrom(PasswordItem other)
        {
            Title = other.Title;
            Category = other.Category;
            Account = other.Account;
            Password = other.Password;
            Url = other.Url;
            Remark = other.Remark;
            UpdatedAt = other.UpdatedAt;
            IsNew = other.IsNew;
        }
    }
}
