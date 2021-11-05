using JetBrains.Annotations;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public abstract class AbstractProxy<TProxyType>
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly TProxyType RealObject;

    protected AbstractProxy(TProxyType realObject)
    {
      RealObject = realObject;
    }
  }
}
