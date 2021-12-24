using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalDecoSpawnPool : TrapSpawnPoolProxy
  {
    private const string GlobalDecoSpawnPoolName = "GlobalDecoSpawnPool";

    /// <inheritdoc />
    private GlobalDecoSpawnPool([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
      : base(dungeon.transform.Find(GlobalDecoSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException("GlobalDecoSpawnPool.TrapSpawnPool was not found."), logger)
    {
    }

    public static GlobalDecoSpawnPool CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger)
    {
      return new GlobalDecoSpawnPool(dungeon, logger);
    }
  }
}
