using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonBossRoom : DungeonRoom
  {
    private const string BossTriggerName = "Boss_Trigger";
    private const string BossSpawnPointName = "Boss_SpawnPoint";
    public readonly TrapTriggerProxy RoomBossTrigger;
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly TrapSpawnerProxy RoomBossSpawnPoint;

    public DungeonBossRoom(string name, GameObject dungeonPrefab)
      : base(name, dungeonPrefab)
    {
      try
      {
        RoomBossTrigger = new TrapTriggerProxy(dungeonPrefab, name, BossTriggerName);
        RoomBossSpawnPoint = new TrapSpawnerProxy(dungeonPrefab, name, BossSpawnPointName);
        RoomBossSpawnPoint.SetIgnoreSpawnPoolOverrides(true);
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }
  }
}
