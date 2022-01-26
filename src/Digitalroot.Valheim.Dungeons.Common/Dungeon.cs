using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Enums;
using Digitalroot.Valheim.Dungeons.Common.Rooms;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class Dungeon
  {
    public readonly string Name;
    public readonly List<DungeonBossRoom> DungeonBossRooms = new();
    public readonly List<DungeonRoom> DungeonRooms = new();
    public readonly ISpawnPool GlobalDestructibleSpawnPool;
    public readonly ISpawnPool GlobalEnemySpawnPool;
    public readonly ISpawnPool GlobalMiniBossSpawnPool;
    public readonly ISpawnPool GlobalTreasureSpawnPool;
    
    public readonly GameObject DungeonPrefab;
    private readonly ITraceableLogging _logger;

    public Dungeon([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
    {
      try
      {
        _logger = logger;
        Log.Trace(logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Creating Dungeon ({name}) from {dungeonPrefab.name}");
        Name = name;
        DungeonPrefab = dungeonPrefab;
        GlobalDestructibleSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Destructible, DungeonPrefab, _logger);
        GlobalEnemySpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Enemy, DungeonPrefab, _logger);
        GlobalMiniBossSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.MiniBoss, DungeonPrefab, _logger);
        GlobalTreasureSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Treasure, DungeonPrefab, _logger);
      }
      catch (Exception e)
      {
        Log.Error(_logger, e);
      }
    }

    public void AddDungeonRoom([NotNull] string dungeonRoomName) => AddDungeonRoom(DungeonRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonRoom([NotNull] DungeonRoom dungeonRoom)
    {
      // Log.Trace(_logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Adding {dungeonRoom}");
      DungeonRooms.Add(dungeonRoom);
    }

    public void AddDungeonBossRoom([NotNull] string dungeonRoomName) => AddDungeonBossRoom(DungeonBossRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonBossRoom([NotNull] DungeonBossRoom dungeonRoom)
    {
      // Log.Trace(_logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Adding {dungeonRoom}");
      DungeonBossRooms.Add(dungeonRoom);
    }
  }
}
