using Digitalroot.Valheim.Common;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  public class SpawnPool
  {
    protected readonly GameObject Dungeon;

    // ReSharper disable once MemberCanBeProtected.Global
    public SpawnPool(GameObject dungeon)
    {
      Dungeon = dungeon;
    }

    protected GameObject ConfigureAsTrash(GameObject prefab)
    {
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      if (prefab == null) return null;
      // prefab.transform.localScale *= 2;
      var character = prefab.GetComponent<Character>();
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(3); // Need to do this at spawn time. 
      return prefab;
    }

    protected GameObject ConfigureAsBoss(GameObject prefab)
    {
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      if (prefab == null) return null;
      prefab.transform.localScale *= 2;
      var character = prefab.GetComponent<Character>();
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(4); // Need to do this at spawn time. 
      return prefab;
    }

  }
}
