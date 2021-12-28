using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalEnemySpawnPoolProxy : TrapSpawnPoolProxy
  {
    private const string GlobalEnemySpawnPoolName = "GlobalEnemySpawnPool";

    /// <inheritdoc />
    private GlobalEnemySpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalEnemySpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalEnemySpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static GlobalEnemySpawnPoolProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalEnemySpawnPoolProxy(dungeon, logger);
    }
  }
}
