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
    public readonly GlobalSpawnPool GlobalSpawnPool;
    public readonly GameObject DungeonPrefab;
    private readonly ITraceableLogging _logger;

    public Dungeon([NotNull] string name, [NotNull] GameObject dungeonPrefab, ITraceableLogging logger)
    {
      try
      {
        _logger = logger;
        Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Dungeon ({name}) from {dungeonPrefab.name}");
        Name = name;
        DungeonPrefab = dungeonPrefab;
        GlobalSpawnPool = GlobalSpawnPool.CreateInstance(DungeonPrefab, _logger);
      }
      catch (Exception e)
      {
        Log.Error(_logger, e);
      }
    }

    public void AddDungeonRoom(string dungeonRoomName) => AddDungeonRoom(DungeonRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonRoom(DungeonRoom dungeonRoom)
    {
      Log.Trace(_logger, $"Adding {dungeonRoom}");
      DungeonRooms.Add(dungeonRoom);
    }

    public void AddDungeonBossRoom(string dungeonRoomName) => AddDungeonBossRoom(DungeonBossRoom.CreateInstance(dungeonRoomName, DungeonPrefab, _logger));

    public void AddDungeonBossRoom(DungeonBossRoom dungeonRoom)
    {
      Log.Trace(_logger, $"Adding {dungeonRoom}");
      DungeonBossRooms.Add(dungeonRoom);
    }
  }
}
