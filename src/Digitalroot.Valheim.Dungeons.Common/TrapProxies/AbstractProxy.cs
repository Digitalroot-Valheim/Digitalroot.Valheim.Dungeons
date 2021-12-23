using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public abstract class AbstractProxy<TProxyType> where TProxyType : IEventLogger
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly TProxyType RealObject;
    protected readonly ITraceableLogging _logger;

    protected AbstractProxy([NotNull] TProxyType realObject, [NotNull] ITraceableLogging logger)
    {
      _logger = logger;
      RealObject = realObject;
      RealObject.LogEvent += HandleLogEvent;
    }

    ~AbstractProxy()
    {
      RealObject.LogEvent -= HandleLogEvent;
    }

    // ReSharper disable once MemberCanBeProtected.Global
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
