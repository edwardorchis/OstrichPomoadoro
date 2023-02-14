using System.Windows.Input;

namespace OstrichPomoadoro
{
    public partial class InterruptDialog : Controls.OstrichDialog
    {
        public InterruptDialog(bool bRest)
        {
            InitializeComponent();
            lblContext.Content = bRest ? "赶紧干活" : "休息一会儿吧";
        }

        private void btnContinue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void btnReset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
