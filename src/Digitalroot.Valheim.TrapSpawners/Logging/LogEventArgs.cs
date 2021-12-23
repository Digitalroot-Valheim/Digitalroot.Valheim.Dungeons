using JetBrains.Annotations;
using System;

namespace Digitalroot.Valheim.TrapSpawners.Logging
{
  [UsedImplicitly]
  public class LogEventArgs : EventArgs
  {
    public string Message { get; set; }
    public LogLevel LogLevel { get; set; }
    public Exception Exception { get; set; }
  }
}
