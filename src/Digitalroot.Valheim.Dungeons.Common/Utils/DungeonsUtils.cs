using Digitalroot.Valheim.Common;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Utils
{
  public static class DungeonsUtils
  {
    public static GameObject ConfigureAsTrash(GameObject prefab, ITraceableLogging logger)
    {
      Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      if (prefab == null) return null;
      // prefab.transform.localScale *= 2;
      // var character = prefab.GetComponent<Character>();
      // Log.Trace(StaticLogger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(3); // Need to do this at spawn time. 
      return prefab;
    }

    public static GameObject ConfigureAsBoss(GameObject prefab, ITraceableLogging logger)
    {
      Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      return prefab == null ? null : prefab;
      // prefab.transform.localScale *= 2;
    }
  }
}
