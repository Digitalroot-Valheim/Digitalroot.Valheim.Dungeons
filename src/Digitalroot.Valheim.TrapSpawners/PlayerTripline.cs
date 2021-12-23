using JetBrains.Annotations;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners
{
  public class PlayerTripline : MonoBehaviour
  {
    [UsedImplicitly]
    public void OnTriggerEnter(Collider other)
    {
      if (other.name is not ("Player(Clone)" or "Player")) return;
      var trigger = GetComponentInParent<TrapTrigger>();
      if (trigger._isTriggered) return;
      trigger.OnTriggered(other);
    }
  }
}
