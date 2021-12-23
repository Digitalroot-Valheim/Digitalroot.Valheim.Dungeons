using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalSpawnPool : TrapSpawnPoolProxy
  {
    private const string GlobalSpawnPoolName = "GlobalSpawnPool";

    /// <inheritdoc />
    private GlobalSpawnPool([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalSpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static GlobalSpawnPool CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalSpawnPool(dungeon, logger);
    }
  }
}
