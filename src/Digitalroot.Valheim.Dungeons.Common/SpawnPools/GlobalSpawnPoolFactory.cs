using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Enums;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public static class GlobalSpawnPoolFactory
  {
    public static ISpawnPool CreateInstance(GlobalSpawnPoolNames globalSpawnPoolNames
                                            , [NotNull] GameObject dungeon
                                            , [NotNull] ITraceableLogging logger)
    {
      return globalSpawnPoolNames switch
      {
        GlobalSpawnPoolNames.MiniBoss => GlobalSpawnPoolProxy.CreateInstance(dungeon, logger, "GlobalMiniBossSpawnPool")
        , GlobalSpawnPoolNames.Destructible => GlobalSpawnPoolProxy.CreateInstance(dungeon, logger, "GlobalDestructibleSpawnPool")
        , GlobalSpawnPoolNames.Enemy => GlobalSpawnPoolProxy.CreateInstance(dungeon, logger, "GlobalEnemySpawnPool")
        , GlobalSpawnPoolNames.Treasure => GlobalSpawnPoolProxy.CreateInstance(dungeon, logger, "GlobalTreasureSpawnPool")
        , _ => throw new ArgumentOutOfRangeException(nameof(globalSpawnPoolNames), globalSpawnPoolNames, null)
      };
    }
  }
}
