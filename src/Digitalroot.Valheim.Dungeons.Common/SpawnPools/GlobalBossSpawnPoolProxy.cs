using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  [Obsolete]
  public class GlobalBossSpawnPoolProxy : TrapSpawnPoolProxy
  {
    private const string GlobalBossSpawnPoolName = "GlobalBossSpawnPool";

    /// <inheritdoc />
    private GlobalBossSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalBossSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalEnemySpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static TrapSpawnPoolProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalBossSpawnPoolProxy(dungeon, logger);
    }
  }
}
