using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners.Enums;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonBossRoom : DungeonRoom
  {
    // private const string BossTriggerName = "BossTrigger";
    // public readonly TrapTriggerProxy RoomBossTrigger;
    public readonly IEnumerable<TrapSpawnerProxy> RoomBossSpawnPoint;

    private DungeonBossRoom([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
      : base(name, dungeonPrefab, logger)
    {
      try
      {
        // RoomBossTrigger = TrapTriggerProxy.CreateInstance(dungeonPrefab, name, _logger, BossTriggerName);
        RoomBossSpawnPoint = RoomTrigger?.Spawners?.Where(s => s.SpawnerType == SpawnerType.Boss);
      }
      catch (Exception e)
      {
        Log.Error(Logger, e);
      }
    }

    public new static DungeonBossRoom CreateInstance([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
    {
      return new DungeonBossRoom(name, dungeonPrefab, logger);
    }
  }
}
