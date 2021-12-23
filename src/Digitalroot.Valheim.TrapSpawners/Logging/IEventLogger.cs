using System;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Logging
{
  public interface IEventLogger
  {
    [HideInInspector]
    event EventHandler<LogEventArgs> LogEvent;
    void OnLogEvent(object sender, LogEventArgs logEventArgs);
  }
}
