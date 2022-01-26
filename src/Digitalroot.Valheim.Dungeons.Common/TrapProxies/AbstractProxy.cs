using Digitalroot.Valheim.Common;
// using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public abstract class AbstractProxy<TProxyType>
    // where TProxyType : AbstractProxy
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly TProxyType RealObject;
    protected readonly ITraceableLogging Logger;

    protected AbstractProxy([NotNull] TProxyType realObject, [NotNull] ITraceableLogging logger)
    {
      Logger = logger;
      RealObject = realObject;
      // RealObject.LogEvent += HandleLogEvent;
    }

    // ~AbstractProxy()
    // {
    //   RealObject.LogEvent -= HandleLogEvent;
    // }

    // ReSharper disable once MemberCanBeProtected.Global
    // public void HandleLogEvent([NotNull]object sender, [NotNull] LogEventArgs e)
    // {
    //   switch (e.LogLevel)
    //   {
    //     case LogLevel.Info:
    //       Log.Info(Logger, e);
    //       break;
    //
    //     case LogLevel.Debug:
    //       Log.Debug(Logger, e);
    //       break;
    //
    //     case LogLevel.Warning:
    //       Log.Warning(Logger, e);
    //       break;
    //
    //     case LogLevel.Error:
    //       Log.Error(Logger, e);
    //       break;
    //
    //     case LogLevel.Fatal:
    //       Log.Fatal(Logger, e);
    //       break;
    //
    //     case LogLevel.Trace:
    //       Log.Trace(Logger, e);
    //       break;
    //
    //     case LogLevel.Message:
    //       Log.Message(Logger, e);
    //       break;
    //
    //     default:
    //       throw new ArgumentOutOfRangeException();
    //   }
    // }
  }
}
