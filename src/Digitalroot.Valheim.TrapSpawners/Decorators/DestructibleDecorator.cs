using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Decorators
{
  public static class DestructibleDecorator
  {
    public static GameObject AsDestructible(this GameObject prefab, float scaleSize, int levelMin, int levelMax)
    {
      return prefab;
    }
  }
}
