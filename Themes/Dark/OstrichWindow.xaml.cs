using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using OstrichPomoadoro.Controls;

namespace OstrichPomoadoro.Themes
{
    public partial class OstrichWindow
    {
        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Window.GetWindow(sender as FrameworkElement).DragMove();
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as FrameworkElement).Close();
        }
        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            var vm = Window.GetWindow(sender as FrameworkElement);
            if (!vm.ShowInTaskbar) {
                vm.ShowInTaskbar = true;
            }
            vm.WindowState = WindowState.Minimized;
        }

        private void BtnState_Click(object sender, RoutedEventArgs e)
        {
            var mw = Window.GetWindow(sender as FrameworkElement) is Controls.OstrichWindow ? 
                Window.GetWindow(sender as FrameworkElement) as Controls.OstrichWindow : null;
            if (mw != null) {
                mw.WindowMode = (mw.WindowMode == Controls.WindowMode.Normal) ? WindowMode.Clock : WindowMode.Normal;
            }
        }

        private void OstrichButton_Click(object sender, RoutedEventArgs e)
        {
            var mw = Window.GetWindow(sender as FrameworkElement) is Controls.OstrichWindow ?
                Window.GetWindow(sender as FrameworkElement) as Controls.OstrichWindow : null;
            if (mw != null) {
                mw.OnOstrichButtonClickEvent(sender);
            }
            
        }
    }
}
