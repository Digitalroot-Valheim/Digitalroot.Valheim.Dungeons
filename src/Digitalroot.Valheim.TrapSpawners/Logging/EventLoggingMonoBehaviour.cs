using System;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Logging
{
  public abstract class EventLoggingMonoBehaviour : MonoBehaviour, IEventLogger
  {
    #region Logging

    [HideInInspector]
    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log($"[REMOVE] {logEventArgs.Message}"); // Todo: Remove
        LogEvent?.Invoke(sender, logEventArgs);
      }
      catch (Exception e)
      {
        LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
      }
    }

    private void OnLogEvent(LogEventArgs logEventArgs) => OnLogEvent(this, logEventArgs);
    private protected void LogError(Exception e) => OnLogEvent(new LogEventArgs { Message = e.Message, Exception = e, LogLevel = LogLevel.Error });
    private protected void LogTrace(string msg) => Log(msg, LogLevel.Trace);
    private void Log(string msg, LogLevel logLevel) => OnLogEvent(new LogEventArgs { Message = msg, LogLevel = logLevel });

    #endregion
  }
}
