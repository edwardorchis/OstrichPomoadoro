using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace OstrichPomoadoro
{
    public class TaskListItem : INotifyPropertyChanged
    {
        public TaskListItem(string name, int e)
        {
            _id = DateTime.Now.ToUniversalTime().Ticks;
            _name = name;
            _e = e;
        }

        private long _id = -1;
        public long Id { get { return _id; } }
        public long Parent { set; get; } = -1;

        private string? _name;
        private int _e = 0;
        private int _p = 0;
        private int _u = 0;
        private int _i = 0;

        public string? Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        /// <summary>
        /// 预估番茄数
        /// </summary>
        public int E  { get { return _e; } set {  _e = value; OnPropertyChanged("E"); } }
        /// <summary>
        /// 实际番茄数
        /// </summary>
        public int P  { get { return _p; } set {  _p = value; OnPropertyChanged("P"); } }
        /// <summary>
        /// 非计划任务
        /// </summary>
        public int U  { get { return _u; } set {  _u = value; OnPropertyChanged("U"); } }
        /// <summary>
        /// 中断数
        /// </summary>
        public int I  { get { return _i; } set {  _i = value; OnPropertyChanged("I"); } }

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Logger.GetLogger("opr_").Info(String.Format("{0} {1}", Id.ToString(), propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public static TaskListItem? Parse(string line)
        {
            try {
                var lst = line.Split(",");
                var task = new TaskListItem(lst[2], int.Parse(lst[3]));
                task._id = long.Parse(lst[0]);
                task.Parent = long.Parse(lst[1]);
                task._p = int.Parse(lst[4]);
                task._u = int.Parse(lst[5]);
                task._i = int.Parse(lst[6]);

                return task;
            }
            catch (Exception ex) {
                Logger.GetLogger().Error(ex.ToString());                
            }
            return null;
        }
        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6}", Id, Parent, Name, E, P, U, I );
        }
    }


    public class TaskList
    {
        private ObservableCollection<TaskListItem> taskLists = new ObservableCollection<TaskListItem>();
        public ObservableCollection<TaskListItem> TaskLists {
            get {
                return this.taskLists;
            }
        }

        public TaskList() {           
            var path = GetDbPath();
            var fileName = path + "\\cur";
            if (!File.Exists(fileName)) {
                return;
            }
            StreamReader sr = new StreamReader(fileName);
            try {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    var t = TaskListItem.Parse(line);
                    if (t != null) {
                        taskLists.Add(t);
                    }
                }
            }
            catch (Exception ex) {
                Logger.GetLogger().Error(ex.ToString());
            }
            finally {
                sr.Close();
            }
        }

        public void AddTask(TaskListItem task) {
            taskLists.Add(task);
            SaveCurrent();
        }

        public void ModTask(long id, string name, int e) {
            for (int i = 0; i < taskLists.Count; i++) {
                if (taskLists[i].Id == id) {
                    taskLists[i].Name = name;
                    taskLists[i].E = e;
                    break;
                }
            }
            SaveCurrent();
        }

        public void AddSubTask(long id, TaskListItem subTask)
        {
            for (int i = 0; i < taskLists.Count; i++) {
                if (taskLists[i].Id == id) {
                    subTask.Name = "*" + subTask.Name;
                    subTask.Parent = id;
                    taskLists.Insert(i + 1, subTask);                    
                }
            }
            SaveCurrent();
        }

        public void RmvTask(TaskListItem task) {
            taskLists.Remove(task);
            SaveCurrent();
            SaveHistory(task);
        }

        string GetDbPath() {
            string dbPath = ".db";
            if (!Directory.Exists(dbPath)) {
                Directory.CreateDirectory(dbPath);
            }
            return dbPath;
        }

        public void SaveCurrent()
        {
            var path = GetDbPath();
            var fileName = path + "\\cur";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, false)) {
                for (int i = 0; i < taskLists.Count; i++) {
                    file.WriteLine(taskLists[i].ToString());
                }  
            }
        }
        public void SaveHistory(TaskListItem task)
        {
            var path = GetDbPath();
            var fileName = path + "\\history";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true)) {
                file.WriteLine(task.ToString());
            }
        }   
    }

    public class Logger
    {
        static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>(); 
        public static Logger GetLogger(string filePrefix = "")
        {
            if (!loggers.ContainsKey(filePrefix)) {
                loggers.Add(filePrefix, new Logger(filePrefix));
                
            }
            return loggers[filePrefix];
        }
        private string _logPath = ".db";
        private string _filePrefix = "";
        public Logger(string prefix)
        {
            _filePrefix = prefix;
            if (!Directory.Exists(_logPath)) {
                Directory.CreateDirectory(_logPath);
            }
            
        }

        void writelog(string level, string log)
        {
            var fileName = _logPath + "\\" + _filePrefix + DateTime.Now.ToString("yyyyMM") + ".log";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true)) {
                file.WriteLine(level + "\t" + DateTime.Now.ToString("dd HH:mm:ss.fff") + "\t" + log);
            }
        }

        public void Error(string log) {
            writelog("E", log);
        }
        public void Info(string log)
        {
            writelog("I", log);
        }
        public void Debug(string log)
        {
            writelog("D", log);
        }
    }
}
