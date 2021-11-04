using Digitalroot.Valheim.Common;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class Dungeon : ITraceableLogging
  {
    // ReSharper disable once NotAccessedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly string Name;
    public readonly List<DungeonBossRoom> DungeonBossRooms = new List<DungeonBossRoom>();
    public readonly List<DungeonRoom> DungeonRooms = new List<DungeonRoom>();
    public readonly GlobalSpawnPool GlobalSpawnPool;
    public readonly GameObject DungeonPrefab;

    public Dungeon([NotNull] string name, [NotNull] GameObject dungeonPrefab)
    {
      try
      {
        Log.Trace(this, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Dungeon ({name}) from {dungeonPrefab.name}");
        Name = name;
        DungeonPrefab = dungeonPrefab;
        GlobalSpawnPool = new GlobalSpawnPool(DungeonPrefab);
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }

    public void AddDungeonRoom(string dungeonRoomName) => AddDungeonRoom(new DungeonRoom(dungeonRoomName, DungeonPrefab));
    public void AddDungeonRoom(DungeonRoom dungeonRoom)
    {
      DungeonRooms.Add(dungeonRoom);
    }

    public void AddDungeonBossRoom(string dungeonRoomName) => AddDungeonBossRoom(new DungeonBossRoom(dungeonRoomName, DungeonPrefab));
    public void AddDungeonBossRoom(DungeonBossRoom dungeonRoom)
    {
      DungeonBossRooms.Add(dungeonRoom);
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => $"Digitalroot.Valheim.Dungeons.Common.{nameof(Dungeon)}";

    /// <inheritdoc />
    [UsedImplicitly]
    public bool EnableTrace { get; private set; }

    public void SetEnableTrace(bool value)
    {
      EnableTrace = value;
      GlobalSpawnPool.SetEnableTrace(value);
      foreach (var dungeonBossRoom in DungeonBossRooms)
      {
        dungeonBossRoom.SetEnableTrace(value);
      }
    }

    #endregion
  }
}
