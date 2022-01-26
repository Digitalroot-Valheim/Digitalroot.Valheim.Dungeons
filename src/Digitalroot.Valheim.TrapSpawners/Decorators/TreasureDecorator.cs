using System;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Decorators
{
  public static class TreasureDecorator
  {
    [Obsolete("", true)]
    public static GameObject AsTreasure(this GameObject prefab, float scaleSize, int levelMin, int levelMax)
    {
      return prefab;
    }
  }
}
