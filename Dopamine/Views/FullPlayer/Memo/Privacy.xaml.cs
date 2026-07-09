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

namespace Dopamine.Views.FullPlayer.Memo
{
    /// <summary>
    /// Interaction logic for Privacy.xaml
    /// </summary>
    public partial class Privacy : UserControl
    {
        public Privacy()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                // 根据滚轮方向，向左或向右滚动
                if (e.Delta > 0)
                    scrollViewer.LineLeft();
                else
                    scrollViewer.LineRight();

                e.Handled = true; // 拦截事件，防止外层也跟着滚
            }
        }
    }
}
