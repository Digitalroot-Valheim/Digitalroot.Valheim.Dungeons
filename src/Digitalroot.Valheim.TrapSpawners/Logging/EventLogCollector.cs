using JetBrains.Annotations;
using System;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Logging
{
  [AddComponentMenu("Traps/Event Log Collector", 34), DisallowMultipleComponent]
  [UsedImplicitly]
  public class EventLogCollector : MonoBehaviour, IEventLogger
  {
    public void Start()
    {
      foreach (var trapSpawnPool in GetComponentsInChildren<TrapSpawnPool>())
      {
        trapSpawnPool.LogEvent += OnLogEvent;
      }

      foreach (var trapTrigger in GetComponentsInChildren<TrapTrigger>())
      {
        trapTrigger.LogEvent += OnLogEvent;
      }
    }

    public void OnDestroy()
    {
      foreach (var trapSpawnPool in GetComponentsInChildren<TrapSpawnPool>())
      {
        trapSpawnPool.LogEvent -= OnLogEvent;
      }

      foreach (var trapTrigger in GetComponentsInChildren<TrapTrigger>())
      {
        trapTrigger.LogEvent -= OnLogEvent;
      }
    }

    public void RegisterLogEventSource(IEventLogger eventLogger)
    {
      ZLog.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}].{MethodBase.GetCurrentMethod().Name}()");
      eventLogger.LogEvent += OnLogEvent;
    }

    public void UnRegisterLogEventSource(IEventLogger eventLogger)
    {
      ZLog.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}].{MethodBase.GetCurrentMethod().Name}()");
      eventLogger.LogEvent -= OnLogEvent;
    }

    #region Implementation of IEventLogger

    /// <inheritdoc />
    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log(logEventArgs.Message);
        LogEvent?.Invoke(sender, logEventArgs);
      }
      catch (Exception e)
      {
        LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
      }
    }

    #endregion
  }
}
