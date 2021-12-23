using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners.Logging;
using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public abstract class AbstractProxy<TProxyType> where TProxyType : IEventLogger
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly TProxyType RealObject;

    protected AbstractProxy(TProxyType realObject)
    {
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
          Log.Info(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Debug:
          Log.Debug(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Warning:
          Log.Warning(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Error:
          Log.Error(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Fatal:
          Log.Fatal(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Trace:
          Log.Trace(DungeonsUtils.StaticLogger, e);
          break;

        case LogLevel.Message:
          Log.Message(DungeonsUtils.StaticLogger, e);
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
