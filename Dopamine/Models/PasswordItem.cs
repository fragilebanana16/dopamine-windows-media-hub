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
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _category; // 社交, 金融, 工作, 购物, 其他
        public string Category { get => _category; set => SetProperty(ref _category, value); }

        private string _url;
        public string Url { get => _url; set => SetProperty(ref _url, value); }

        private string _username;
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _notes;
        public string Notes { get => _notes; set => SetProperty(ref _notes, value); }

        private DateTime _lastUpdated;
        public DateTime LastUpdated { get => _lastUpdated; set => SetProperty(ref _lastUpdated, value); }
    }
}
