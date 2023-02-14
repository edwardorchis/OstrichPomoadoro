using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OstrichPomoadoro.Controls
{  public enum WindowMode
    {
        Normal,
        Clock
    }
    public class OstrichWindow : Window
    {
        static OstrichWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OstrichWindow), new FrameworkPropertyMetadata(typeof(OstrichWindow)));
        }

        public WindowMode WindowMode
        {
            get { return (WindowMode)GetValue(WindowModeProperty); }
            set { 
                SetValue(WindowModeProperty, value);
                if (value == WindowMode.Normal) {
                    this.Height = 500;
                    this.Topmost = false;
                } else if (value == WindowMode.Clock) {
                    this.Height = 175;                 
                    this.Topmost = true;
                }
            }
        }

        public static readonly DependencyProperty WindowModeProperty =
            DependencyProperty.Register("WindowMode", typeof(WindowMode), typeof(OstrichWindow), new PropertyMetadata(WindowMode.Normal));

        public delegate void btnClickEvent(OstrichButton sender);
        public event btnClickEvent OstrichButtonClickEvent;

        public void OnOstrichButtonClickEvent(object sender)
        {
            OstrichButtonClickEvent?.Invoke(sender as OstrichButton);
        }
    }
}
