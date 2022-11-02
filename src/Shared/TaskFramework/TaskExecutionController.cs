using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Meshmakers.Common.Shared.TaskFramework;

/// <summary>
///     Controls the task execution of a single task
/// </summary>
public class TaskExecutionController : INotifyPropertyChanged
{
    private readonly TaskEngine _engine;
    private readonly List<TaskBase> _lstAncestorTasks;
    private readonly TaskBase _task;
    private bool _hasError;
    private bool _isCompleted;
    private bool _isRunning;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="engine">Engine object</param>
    /// <param name="task">The task this object is responsible</param>
    internal TaskExecutionController(TaskEngine engine, TaskBase task)
    {
        _engine = engine;
        _task = task;
        CurrentWaitHandle = new ManualResetEvent(false);
        _lstAncestorTasks = new List<TaskBase>();
    }

    /// <summary>
    ///     Returns the exception object, when an exception has occured while execution of the task
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    ///     Returns true, when the execution of the task has been successfully succeeded
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
    ///     Returns true, when the execution of the task is currently running
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
    ///     Returns true, when the execution of the task has been successfully succeeded
    /// </summary>
    public bool HasError
    {
        get => _hasError;
        private set
        {
            if (_hasError != value)
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>
    ///     Returns the ResetEvent object
    /// </summary>
    public ManualResetEvent CurrentWaitHandle { get; }

    #region INotifyPropertyChanged Members

    /// <summary>
    ///     Thrown, when the value of a property changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    /// <summary>
    ///     Adds a wait handle
    /// </summary>
    /// <param name="task">Ancestor task</param>
    internal void AddAncestor(TaskBase task)
    {
        _lstAncestorTasks.Add(task);
    }

    /// <summary>
    ///     Runs the task
    /// </summary>
    internal void Run(object? _)
    {
        try
        {
            var lstWaitHandles = new List<WaitHandle>();
            var lstCtrl = new List<TaskExecutionController>();
            foreach (var task in _lstAncestorTasks)
            {
                var ctrl = _engine.GetExecutionController(task);
                lstWaitHandles.Add(ctrl.CurrentWaitHandle);
                lstCtrl.Add(ctrl);
            }

            // Wait for other tasks
            if (lstWaitHandles.Count > 0)
            {
                WaitHandle.WaitAll(lstWaitHandles.ToArray());
            }

            if (lstCtrl.Any(x => x.HasError))
            {
                throw new TaskException(Resource.EXCEPTION_0001);
            }

            // Check: Task is canceled?
            if (_engine.IsCanceling)
            {
                throw new TaskException(Resource.EXCEPTION_0002);
            }

            // Define the result object
            _task.SetResultObject(_engine.ResultObjects);

            IsRunning = true;
            _task.Run();

            // Check: Task is canceled?
            if (_engine.IsCanceling)
            {
                throw new TaskException(Resource.EXCEPTION_0002);
            }

            IsCompleted = true;
        }
        catch (Exception ex)
        {
            Exception = ex;
            HasError = true;
        }
        finally
        {
            IsRunning = false;
            CurrentWaitHandle.Set();
        }
    }

    #region Protected Member

    /// <summary>
    ///     Throws the PropertyChanged-Event
    /// </summary>
    /// <param name="propertyName">Event name</param>
    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion Protected Member
}
