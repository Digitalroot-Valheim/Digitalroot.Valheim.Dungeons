using Digitalroot.Valheim.TrapSpawners.CMB;
using JetBrains.Annotations;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Trip Wire", 33), DisallowMultipleComponent]
  public class PlayerTripWire : EventLoggingMonoBehaviour
  {
    [UsedImplicitly]
    public void OnTriggerEnter(Collider other)
    {
      if (Utils.GetPrefabName(other.gameObject) is not nameof(Player)) return;
      GetComponentInParent<TrapTrigger>()?.OnTriggered(other);
    }
  }
}
