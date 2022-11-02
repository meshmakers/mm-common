using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Meshmakers.Common.Shared.TaskFramework;

/// <summary>
///     Represents a task
/// </summary>
public abstract class TaskBase : INotifyPropertyChanged
{
    #region Constructor

    /// <summary>
    ///     Constructor
    /// </summary>
    protected TaskBase()
    {
        _maxValue = 100;
        _currentValue = 0;
        _hasWarnings = false;
        _isProgressAvailable = false;
    }

    #endregion Constructor

    #region INotifyPropertyChanged Member

    /// <summary>
    ///     Thrown, when the value of a property changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

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

    #region Declarations

    private int _maxValue;
    private int _currentValue;
    private bool _hasWarnings;
    private string? _warningMessage;
    private string? _statusMessage;
    private bool _isProgressAvailable;

    #endregion Declarations

    #region Public Member

    /// <summary>
    ///     Gets or Sets a Flag, that indicates the availability of a progress
    /// </summary>
    public bool IsProgressAvailable
    {
        get => _isProgressAvailable;
        set
        {
            if (_isProgressAvailable != value)
            {
                _isProgressAvailable = value;
                OnPropertyChanged(nameof(IsProgressAvailable));
            }
        }
    }

    /// <summary>
    ///     Returns true, when the task is subjected for termination
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public bool CancelPending { get; private set; }

    /// <summary>
    ///     Returns the result object list, where other tasks can post their results
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, object>? ResultObjects { get; private set; }

    /// <summary>
    ///     Gets or Sets the maximum value of the progress
    /// </summary>
    public int MaxValue
    {
        get => _maxValue;
        set
        {
            if (_maxValue != value)
            {
                _maxValue = value;
                OnPropertyChanged(nameof(MaxValue));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets the current value of the progress
    /// </summary>
    public int CurrentValue
    {
        get => _currentValue;
        set
        {
            if (_currentValue != value)
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets the warning flag of the task
    /// </summary>
    public bool HasWarnings
    {
        get => _hasWarnings;
        protected set
        {
            if (_hasWarnings != value)
            {
                _hasWarnings = value;
                OnPropertyChanged(nameof(HasWarnings));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets the warning message of the task
    /// </summary>
    public string? WarningMessage
    {
        get => _warningMessage;
        protected set
        {
            if (_warningMessage != value)
            {
                _warningMessage = value;
                OnPropertyChanged(nameof(WarningMessage));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets the status message
    /// </summary>
    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
    }

    /// <summary>
    ///     Runs the task
    /// </summary>
    public abstract void Run();

    #endregion Public Member

    #region Internal Member

    internal void SetTermination()
    {
        CancelPending = true;
    }

    internal void SetResultObject(Dictionary<string, object> resultObjects)
    {
        ResultObjects = resultObjects;
    }

    #endregion Internal Member
}
