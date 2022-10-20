using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Meshmakers.Common.Shared.TaskFramework;

/// <summary>
///     Manages tasks
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class TaskEngine : INotifyPropertyChanged
{
    private readonly Dictionary<TaskBase, TaskExecutionController> _taskList;
    private bool _hasErrors;
    private bool _hasWarnings;
    private bool _isCanceled;
    private bool _isCanceling;
    private bool _isCompleted;
    private bool _isRunning;

    #region Constructor

    /// <summary>
    ///     Constructor
    /// </summary>
    public TaskEngine()
    {
        _taskList = new Dictionary<TaskBase, TaskExecutionController>();
        ResultObjects = new Dictionary<string, object>();
    }

    #endregion

    #region Internal Member

    /// <summary>
    ///     Returns the Result object, where tasks can post objects for other tasks
    /// </summary>
    internal Dictionary<string, object> ResultObjects { get; }

    #endregion Internal Member


    #region INotifyPropertyChanged Members

    /// <summary>
    ///     Thrown, when the value of a property changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    /// <summary>
    ///     Thrown, when all tasks of the engine are completed
    /// </summary>
    public event EventHandler? EngineCompleted;

    #region Private Member

    private void th_runTasks()
    {
        IsRunning = true;
        var lstHandles = new List<WaitHandle>();
        foreach (var executionController in _taskList.Values)
        {
            ThreadPool.QueueUserWorkItem(executionController.Run, null);
            lstHandles.Add(executionController.CurrentWaitHandle);
        }

        WaitHandle.WaitAll(lstHandles.ToArray());

        if (IsCanceling)
            IsCanceled = true;

        HasWarnings = _taskList.Keys.Any(x => x.HasWarnings);
        HasErrors = _taskList.Values.Any(x => x.HasError);

        IsRunning = false;
        IsCompleted = true;
        TasksWaitHandle?.Set();

        EngineCompleted?.Invoke(this, EventArgs.Empty);
    }

    #endregion Private Member

    #region Protected Member

    /// <summary>
    ///     Throws the PropertyChanged-Event
    /// </summary>
    /// <param name="propertyName">Event name</param>
    [NotifyPropertyChangedInvocator]
    // ReSharper disable once MemberCanBePrivate.Global
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion Protected Member

    #region Public Member

    /// <summary>
    ///     Returns true, when one or more tasks defined warnings
    /// </summary>
    public bool HasWarnings
    {
        get => _hasWarnings;
        set
        {
            if (_hasWarnings != value)
            {
                _hasWarnings = value;
                OnPropertyChanged(nameof(HasWarnings));
            }
        }
    }

    /// <summary>
    ///     Returns true, when one ore more tasks thrown errors
    /// </summary>
    public bool HasErrors
    {
        get => _hasErrors;
        set
        {
            if (_hasErrors != value)
            {
                _hasErrors = value;
                OnPropertyChanged(nameof(HasErrors));
            }
        }
    }

    /// <summary>
    ///     Returns true, while errors are pending for canceling
    /// </summary>
    public bool IsCanceling
    {
        get => _isCanceling;
        private set
        {
            if (_isCanceling != value)
            {
                _isCanceling = value;
                OnPropertyChanged(nameof(IsCanceling));
            }
        }
    }

    /// <summary>
    ///     Returns true, when the execution of tasks has been canceled
    /// </summary>
    public bool IsCanceled
    {
        get => _isCanceled;
        private set
        {
            if (_isCanceled != value)
            {
                _isCanceled = value;
                OnPropertyChanged(nameof(IsCanceled));
            }
        }
    }

    /// <summary>
    ///     Returns true, when all tasks have completed (normal or in error state)
    /// </summary>
    public bool IsCompleted
    {
        get => _isCompleted;
        private set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }
    }

    /// <summary>
    ///     Returns true, when currently task are running
    /// </summary>
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }
    }

    /// <summary>
    ///     Returns the wait handle of all tasks
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public ManualResetEvent? TasksWaitHandle { get; private set; }

    /// <summary>
    ///     Adds a task to the task list
    /// </summary>
    /// <param name="task"></param>
    public void AddTask(TaskBase task)
    {
        var executionController = new TaskExecutionController(this, task);
        _taskList.Add(task, executionController);
    }

    /// <summary>
    ///     Defines a task as the ancestor of an other task
    /// </summary>
    /// <param name="ancestorTask">The task which is the ancestor the other task depends on</param>
    /// <param name="dependentTask">The task, which can only be executed when the other task is completed</param>
    public void AddAncestorOfTask(TaskBase ancestorTask, TaskBase dependentTask)
    {
        if (!_taskList.ContainsKey(ancestorTask))
            throw new InvalidOperationException("Ancestor task hasn't been added to the task list");
        if (!_taskList.ContainsKey(dependentTask))
            throw new InvalidOperationException("Dependent task hasn't been added to the task list");

        _taskList[dependentTask].AddAncestor(ancestorTask);
    }

    /// <summary>
    ///     Runs the tasks
    /// </summary>
    /// <returns>The wait handle of the task</returns>
    public void RunTasks()
    {
        TasksWaitHandle = new ManualResetEvent(false);
        var threadStart = new ThreadStart(th_runTasks);

        var th = new Thread(threadStart)
        {
            Name = "TasksRunner"
        };
        th.Start();
    }

    /// <summary>
    ///     Cancels the execution of all tasks
    /// </summary>
    public void CancelTasks()
    {
        IsCanceling = true;

        foreach (var task in _taskList.Keys) task.SetTermination();
    }

    /// <summary>
    ///     Returns a list of task error
    /// </summary>
    /// <returns>String, which describes the task error(s)</returns>
    public string? GetTaskErrors()
    {
        var list = _taskList.Values.Where(x => x.HasError);
        var taskExecutionControllers = list.ToList();
        if (taskExecutionControllers.Any())
        {
            var message = "";
            foreach (var executionController in taskExecutionControllers)
            {
                if (executionController.Exception == null)
                    continue;

                message += executionController.Exception.ToString();
                message += Environment.NewLine;
                message += "------------------------------------------------";
                message += Environment.NewLine;
            }

            return message;
        }

        return null;
    }

    /// <summary>
    ///     Returns the task list
    /// </summary>
    public ReadOnlyCollection<TaskBase> Tasks => new(_taskList.Keys.ToList());

    /// <summary>
    ///     Returns the list of task execution controller
    /// </summary>
    public ReadOnlyCollection<TaskExecutionController> TaskExecutionController =>
        new(_taskList.Values.ToList());

    /// <summary>
    ///     Returns the execution controller of the task
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public TaskExecutionController GetExecutionController(TaskBase task)
    {
        return _taskList[task];
    }

    #endregion Public Member
}