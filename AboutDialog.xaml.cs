using System.Windows;
using System.Windows.Input;

namespace OstrichPomoadoro
{
    public partial class AboutDialog : Controls.OstrichDialog
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Escape)) {
                Close();
            }
        }

        private void OstrichDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            textBox.Focus();
        }
    }
}
