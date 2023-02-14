using OstrichPomoadoro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Threading;

namespace OstrichPomoadoro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Controls.OstrichWindow
    {
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            InitializeComponent();
            InitData();
            InitTimer();            
        }
        private DispatcherTimer mDataTimer = null; //定时器
        DateTime mStartTime;
        DateTime mEndTime;
        int mTickInterval = 25;
        int mRestInterval = 5;
        bool mResting = false;

        TaskList vw = new TaskList();

        private void InitTimer()
        {
            if (mDataTimer == null) {
                mDataTimer = new DispatcherTimer();
                mDataTimer.Tick += new EventHandler(DataTimer_Tick);
                mDataTimer.Interval = TimeSpan.FromSeconds(1);
            }
        }
        private void DataTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = mEndTime.Subtract(DateTime.Now).Duration();
            lblTicker.Content = String.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);            
            if (mEndTime <= DateTime.Now) {
                StopTimer();
                if (!mResting) {
                    var item = (gridTaskList.SelectedItem as TaskListItem);
                    if (item != null) {
                        item.P += 1;
                    }
                }
                InterruptDialog dlg = new InterruptDialog(mResting);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mResting = !(bool)dlg.ShowDialog();                
                StartTimer();                
            }
        }
        public void StartTimer()
        {
            if (mDataTimer != null && mDataTimer.IsEnabled == false) {
                mDataTimer.Start();
                mStartTime = DateTime.Now;
                mEndTime = DateTime.Now.AddMinutes(mResting ? mRestInterval : mTickInterval);
                //mEndTime = DateTime.Now.AddSeconds(mResting ? mRestInterval : mTickInterval);
                lblTicker.Content = String.Format("{0:00}:{1:00}", mTickInterval, 0);
                var item = (gridTaskList.SelectedItem as TaskListItem);
                if (item != null) {
                    this.Title = String.Format("{0}({1}/{2})", item.Name, item.P + 1, item.E);
                }
                WindowMode = WindowMode.Clock;
            }
            btnStart.Content = "中断";
        }
        public void StopTimer()
        {
            if (mDataTimer != null && mDataTimer.IsEnabled == true) {
                mDataTimer.Stop();
                lblTicker.Content = String.Format("{0:00}:{1:00}", mTickInterval, 0);
            }
            btnStart.Content = "开始";
        }
    
        private void InitData()
        {
            lblTicker.Content = String.Format("{0:00}:{1:00}", mTickInterval, 0);
            gridTaskList.DataContext = vw;
            if (vw.TaskLists.Count > 0) {
                gridTaskList.SelectedIndex = 0;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender is OstrichButton ? (sender as OstrichButton) : null;
            if (btn == null) {
                return;
            }
            string? txt = btn.Content.ToString();
            if (txt == "开始") {
                StartTimer();
            } else if (txt == "中断") {
                StopTimer();
                if (!mResting) {
                    var item = (gridTaskList.SelectedItem as TaskListItem);
                    if (item != null) {
                        item.I += 1;
                    }
                }
            }
        }
        private void Task_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender is OstrichButton ? (sender as OstrichButton) : null;
            if (btn == null) {
                return;
            }
            this.WindowMode = WindowMode.Normal;
            string? btnName = btn.Name.ToString();
            switch (btnName) {
                case "E": { // Plan
                        TaskDialogModel m = new TaskDialogModel();
                        NewTaskDialog dlg = new NewTaskDialog(m);
                        dlg.Owner = this;
                        dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        dlg.ShowDialog();
                        break;
                    }
                case "U": { // Unplan                       
                        var item = (gridTaskList.SelectedItem as TaskListItem);
                        if (item == null) {
                            break;
                        }
                        TaskDialogModel m = new TaskDialogModel();
                        m.Oper = TaskOpr.U;
                        m.ParentId = item.Id;
                        NewTaskDialog dlg = new NewTaskDialog(m);
                        dlg.Owner = this;
                        dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        dlg.ShowDialog();
                        break;
                    }
                case "D": { // Done
                        var item = (gridTaskList.SelectedItem as TaskListItem);
                        if (item != null) {
                            vw.RmvTask(item);
                        }
                        break;
                    }
            }
        }
        public void SaveTask(TaskDialogModel task)
        {           
            switch(task.Oper) {
                case TaskOpr.U:
                    var item = new TaskListItem(task.Name, task.E) {
                        Parent = task.ParentId,
                    };
                    vw.AddSubTask(task.ParentId, item);
                    break;
                case TaskOpr.N:
                    vw.AddTask(new TaskListItem(task.Name, task.E));
                    break;
                case TaskOpr.M:
                    vw.ModTask(task.Id, task.Name, task.E);
                    break;
            }
        }

        private void OstrichWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vw.SaveCurrent();
        }

        private void OstrichWindow_OstrichButtonClickEvent(OstrichButton sender)
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.Owner = this;
            aboutDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutDialog.ShowDialog();
        }

        private void gridTaskList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (gridTaskList.SelectedItem as TaskListItem);
            if (item == null) {
                return;
            }
            TaskDialogModel m = new TaskDialogModel();
            m.Name = item.Name;
            m.Id = item.Id;
            m.E = item.E;
            m.Oper = TaskOpr.M;
            NewTaskDialog dlg = new NewTaskDialog(m);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowDialog();
            e.Handled = true;
        }
    }
}
