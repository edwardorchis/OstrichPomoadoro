using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OstrichPomoadoro.Controls
{
    public class OstrichCanvas : Canvas
    {
        static OstrichCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OstrichCanvas), new FrameworkPropertyMetadata(typeof(OstrichCanvas)));
        }
    }
}
