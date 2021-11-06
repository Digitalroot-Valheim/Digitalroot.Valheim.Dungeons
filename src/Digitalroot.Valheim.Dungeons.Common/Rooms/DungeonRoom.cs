﻿using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonRoom : ITraceableLogging
  {
    public readonly string Name;
    protected readonly GameObject DungeonPrefab;
    public readonly TrapTriggerProxy RoomTrigger;
    public readonly ISpawnPool RoomSpawnPool;
    public readonly List<TrapSpawnerProxy> RoomSpawnPoints;

    // ReSharper disable once MemberCanBeProtected.Global
    public DungeonRoom([NotNull] string name, [NotNull] GameObject dungeonPrefab)
    {
      try
      {
        Name = name;
        DungeonPrefab = dungeonPrefab;
        RoomTrigger = new TrapTriggerProxy(dungeonPrefab, name);
        RoomSpawnPool = new TrapSpawnPoolProxy(dungeonPrefab, name);
        RoomSpawnPoints = RoomTrigger.GetSpawners();
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => $"Digitalroot.Valheim.Dungeons.Common.{nameof(DungeonRoom)} ({Name})";

    /// <inheritdoc />
    [UsedImplicitly]
    public bool EnableTrace { get; private set; }

    public void SetEnableTrace(bool value)
    {
      EnableTrace = value;
    }

    #endregion
  }

}