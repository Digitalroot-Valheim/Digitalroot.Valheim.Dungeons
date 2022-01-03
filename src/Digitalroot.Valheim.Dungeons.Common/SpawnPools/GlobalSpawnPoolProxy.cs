using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public class GlobalSpawnPoolProxy : TrapSpawnPoolProxy
  {
    private GlobalSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger, [NotNull] string globalSpawnPoolName)
      : base(dungeon.transform.Find("GlobalSpawnPools").Find(globalSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>() ?? throw new NullReferenceException($"{globalSpawnPoolName} was not found."), logger)
    {
    }

    public static ISpawnPool CreateInstance([NotNull] GameObject dungeon, [NotNull] ITraceableLogging logger, [NotNull] string globalSpawnPoolName)
    {
      return new GlobalSpawnPoolProxy(dungeon, logger, globalSpawnPoolName);
    }
  }
}
