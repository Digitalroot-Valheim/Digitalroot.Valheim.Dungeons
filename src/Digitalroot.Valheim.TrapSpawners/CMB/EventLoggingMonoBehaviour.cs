using Digitalroot.Unity3d.Log;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  public abstract class EventLoggingMonoBehaviour : MonoBehaviour
  {
    #region Logging

    
    private static TraceLogger _logger;

    [UsedImplicitly]
    private void Awake()
    {
      if (_logger != null) return;
      lock (typeof(TraceLogger))
      {
        _logger ??= new TraceLogger(Common.Utils.Namespace, true);
      }
    }

    // /// <inheritdoc />
    // public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    // {
    //   try
    //   {
    //     Logger.Trace(logEventArgs.Message);
    //     LogEvent?.Invoke(sender, logEventArgs);
    //   }
    //   catch (Exception e)
    //   {
    //     LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
    //   }
    // }

    private protected static void LogError(Exception e)
    {
      LogTrace(e.Message);
      LogTrace(e.StackTrace);
    }

    private protected static void LogTrace(string msg) => _logger?.Trace(msg);

    #endregion
  }
}
