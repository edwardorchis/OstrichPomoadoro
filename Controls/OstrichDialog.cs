using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OstrichPomoadoro.Controls
{      
    public class OstrichDialog : Window
    {
        static OstrichDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OstrichDialog), new FrameworkPropertyMetadata(typeof(OstrichDialog)));
        }
    }
}
