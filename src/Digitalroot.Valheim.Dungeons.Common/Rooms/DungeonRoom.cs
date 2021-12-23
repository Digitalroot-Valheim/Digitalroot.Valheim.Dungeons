using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonRoom
  {
    public readonly string Name;
    private readonly GameObject _dungeonPrefab;
    public readonly TrapTriggerProxy RoomTrigger;
    public readonly IEnumerable<TrapSpawnerProxy> RoomSpawnPoints;
    private protected readonly ITraceableLogging Logger;

    // ReSharper disable once MemberCanBeProtected.Global
    private protected DungeonRoom([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
    {
      try
      {
        Logger = logger;
        Log.Trace(logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Dungeon Room ({name}) from {dungeonPrefab.name}");
        Name = name;
        _dungeonPrefab = dungeonPrefab;
        RoomTrigger = TrapTriggerProxy.CreateInstance(_dungeonPrefab, Name, Logger);
        RoomSpawnPoints = RoomTrigger?.Spawners;
      }
      catch (Exception e)
      {
        Log.Error(Logger, e);
      }
    }

    public static DungeonRoom CreateInstance([NotNull] string name, [NotNull] GameObject dungeonPrefab, [NotNull] ITraceableLogging logger)
    {
      return new DungeonRoom(name, dungeonPrefab, logger);
    }
  }
}
