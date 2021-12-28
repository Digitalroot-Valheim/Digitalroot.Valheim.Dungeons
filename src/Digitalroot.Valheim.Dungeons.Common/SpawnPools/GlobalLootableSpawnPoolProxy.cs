using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalLootableSpawnPoolProxy : TrapSpawnPoolProxy
  {
    private const string GlobalLootableSpawnPoolName = "GlobalLootableSpawnPool";

    /// <inheritdoc />
    private GlobalLootableSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalLootableSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalLootableSpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static GlobalLootableSpawnPoolProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalLootableSpawnPoolProxy(dungeon, logger);
    }
  }
}
