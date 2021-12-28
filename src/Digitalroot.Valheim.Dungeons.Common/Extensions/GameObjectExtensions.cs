using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Extensions
{
  public static class GameObjectExtensions
  {
    public static GameObject EnemyDecorator(this GameObject prefab)
    {
      // Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      // Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      // prefab.transform.localScale *= 2;
      // var character = prefab.GetComponent<Character>();
      // Log.Trace(StaticLogger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(3); // Need to do this at spawn time. 
      return prefab;
    }

    public static GameObject BossDecorator(this GameObject prefab)
    {
      // Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      // Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      // prefab.transform.localScale *= 2;


      return prefab;
    }
  }
}
