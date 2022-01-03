using JetBrains.Annotations;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Destroy Child ZDOs", 34)]
  [UsedImplicitly, DisallowMultipleComponent]
  public class DestroyChildZDOs : MonoBehaviour
  {
    public void OnDestroy()
    {
      var zNetViews = gameObject.GetComponentsInChildren<ZNetView>();
      if (zNetViews == null) return;
      if (zNetViews.Length == 0) return;
      
      foreach (var zNetView in zNetViews)
      {
        zNetView?.Destroy();
      }
    }
  }
}
