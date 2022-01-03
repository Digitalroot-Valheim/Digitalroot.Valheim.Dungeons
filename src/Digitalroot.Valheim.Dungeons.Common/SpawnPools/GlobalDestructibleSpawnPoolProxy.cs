using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  [Obsolete]
  public class GlobalDestructibleSpawnPoolProxy : TrapSpawnPoolProxy
  {
    private const string GlobalDestructibleSpawnPoolName = "GlobalDestructibleSpawnPool";

    /// <inheritdoc />
    private GlobalDestructibleSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalDestructibleSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalLootableSpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static GlobalDestructibleSpawnPoolProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalDestructibleSpawnPoolProxy(dungeon, logger);
    }
  }
}
