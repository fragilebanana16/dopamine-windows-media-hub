using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopamine.Models
{
    public class SearchResultItem
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public object Tag { get; set; }  // 存原始对象，可以是Track、Album等任何类型
    }

    public class SearchResultGroup
    {
        public string CategoryKey { get; set; } = string.Empty;  // 对应左侧导航 CommandParameter
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public ObservableCollection<SearchResultItem> Items { get; set; }
    }
}
