using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalSpawnPool : TrapSpawnPoolProxy
  {
    private const string GlobalSpawnPoolName = "GlobalSpawnPool";

    /// <inheritdoc />
    public GlobalSpawnPool(GameObject dungeon)
      : base(dungeon?.transform.Find(GlobalSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalSpawnPool.TrapSpawnPool was not found."))
    {
      
    }
  }
}
