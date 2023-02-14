using System.Windows.Controls;
using System.Windows.Input;

namespace OstrichPomoadoro
{
    public enum TaskOpr
    { 
        N = 0,
        U = 1,
        M = 2,
    }
    public class TaskDialogModel
    { 
        public TaskOpr Oper { set; get; } = TaskOpr.N;
        public long ParentId { set; get; } = -1;
        public string? Name { set; get; }
        public int E { set; get; } = 1;

        public long Id { set; get; }
    }

    public partial class NewTaskDialog : Controls.OstrichDialog
    {
        public NewTaskDialog(TaskDialogModel model)
        {
            InitializeComponent();
            mModel = model;
            txtBox.Text = mModel.Name;
            numBox.Text = mModel.E.ToString();
        }
        TaskDialogModel mModel = null;


        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                numBox.Focus();
            } else if (e.Key == Key.Escape) {
                DialogResult = false;
                Close();
            }            
        }

        private void numBox_EnterDownEvent(object sender)
        {
            DialogResult = true;
            var mw = Owner is MainWindow ? (Owner as MainWindow) : null;
            if (mw != null) {
                mModel.Name = txtBox.Text;
                mModel.E = int.Parse(numBox.Text);
                mw.SaveTask(mModel);
            }
            Close();
        }

        private void numBox_EscDownEvent(object sender)
        {
            DialogResult = false;
            Close();
        }
    }
}
