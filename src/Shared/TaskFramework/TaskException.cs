using System;
using System.Runtime.Serialization;

namespace Meshmakers.Common.Shared.TaskFramework;

/// <summary>
///     Represents an task execution error
/// </summary>
[Serializable]
public class TaskException : Exception
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public TaskException()
    {
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message"></param>
    public TaskException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public TaskException(string message, Exception inner) : base(message, inner)
    {
    }
}
