using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners.Logging;
using System;

namespace Digitalroot.Valheim.Dungeons.Common.Logging
{
  public class EventLogHandler
  {
    private readonly ITraceableLogging _logger;

    public EventLogHandler(ITraceableLogging logger)
    {
      _logger = logger;
    }

    public void HandleLogEvent(object sender, LogEventArgs e)
    {
      if (sender == null) return;

      switch (e.LogLevel)
      {
        case LogLevel.Info:
          Log.Info(_logger, e);
          break;

        case LogLevel.Debug:
          Log.Debug(_logger, e);
          break;

        case LogLevel.Warning:
          Log.Warning(_logger, e);
          break;

        case LogLevel.Error:
          Log.Error(_logger, e);
          break;

        case LogLevel.Fatal:
          Log.Fatal(_logger, e);
          break;

        case LogLevel.Trace:
          Log.Trace(_logger, e);
          break;

        case LogLevel.Message:
          Log.Message(_logger, e);
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
