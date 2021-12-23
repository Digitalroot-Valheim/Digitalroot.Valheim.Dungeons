using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using System;
using System.Linq;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonBossRoom : DungeonRoom
  {
    private const string BossTriggerName = "Boss_Trigger";
    public readonly TrapTriggerProxy RoomBossTrigger;
    // public readonly ISpawnPool RoomBossSpawnPool;
    public readonly TrapSpawnerProxy RoomBossSpawnPoint;

    public DungeonBossRoom(string name, GameObject dungeonPrefab)
      : base(name, dungeonPrefab)
    {
      try
      {
        RoomBossTrigger = new TrapTriggerProxy(dungeonPrefab, name, BossTriggerName);
        RoomBossSpawnPoint = RoomBossTrigger.Spawners.FirstOrDefault();
        // RoomBossSpawnPoint?.SetIgnoreSpawnPoolOverrides(true);
        // RoomBossSpawnPool = RoomBossSpawnPoint?.SpawnPool;
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }
  }
}
