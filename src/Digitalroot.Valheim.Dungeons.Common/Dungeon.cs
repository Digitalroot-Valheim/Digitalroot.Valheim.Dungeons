using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Rooms;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
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
    public readonly GlobalEnemySpawnPoolProxy GlobalEnemySpawnPoolProxy;
    public readonly GlobalLootableSpawnPoolProxy GlobalLootableSpawnPoolProxy;
    public readonly GameObject DungeonPrefab;
    private readonly ITraceableLogging _logger;

    public Dungeon([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
    {
      try
      {
        _logger = logger;
        Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Dungeon ({name}) from {dungeonPrefab.name}");
        Name = name;
        DungeonPrefab = dungeonPrefab;
        GlobalEnemySpawnPoolProxy = GlobalEnemySpawnPoolProxy.CreateInstance(DungeonPrefab, _logger);
        GlobalLootableSpawnPoolProxy = GlobalLootableSpawnPoolProxy.CreateInstance(DungeonPrefab, _logger);
      }
      catch (Exception e)
      {
        Log.Error(_logger, e);
      }
    }

    public void AddDungeonRoom([NotNull] string dungeonRoomName) => AddDungeonRoom(DungeonRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonRoom([NotNull] DungeonRoom dungeonRoom)
    {
      // Log.Trace(_logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Adding {dungeonRoom}");
      DungeonRooms.Add(dungeonRoom);
    }

    public void AddDungeonBossRoom([NotNull] string dungeonRoomName) => AddDungeonBossRoom(DungeonBossRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonBossRoom([NotNull] DungeonBossRoom dungeonRoom)
    {
      // Log.Trace(_logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Adding {dungeonRoom}");
      DungeonBossRooms.Add(dungeonRoom);
    }
  }
}
