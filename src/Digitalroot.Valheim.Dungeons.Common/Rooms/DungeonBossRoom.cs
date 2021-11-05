using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.Common.TrapProxies;
using Digitalroot.Valheim.TrapSpawners;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.Rooms
{
  public class DungeonBossRoom : DungeonRoom
  {
    private const string BossTriggerName = "Boss_Trigger";
    private const string BossSpawnPointName = "Boss_SpawnPoint";
    public readonly TrapTriggerProxy RoomBossTrigger;
    public readonly TrapSpawnPoolProxy RoomBossSpawnPool;
    public readonly List<TrapSpawnerProxy> RoomBossSpawnPoints;
    public TrapSpawnerProxy FirstBossSpawnPoint => RoomBossSpawnPoints.FirstOrDefault();

    public DungeonBossRoom(string name, GameObject dungeon)
      : base(name, dungeon)
    {
      try
      {
        RoomBossTrigger = new TrapTriggerProxy(dungeon, name, BossTriggerName);
        RoomBossSpawnPoints = RoomBossTrigger.GetSpawners();
        // RoomBossSpawnPool = FirstBossSpawnPoint.;
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }

    // public TrapSpawnerProxy BossSpawnPoint => new TrapSpawnerProxy(Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{BossSpawnPointName}")?.gameObject.GetComponent<TrapSpawner>() ?? throw new ArgumentNullException($"{nameof(DungeonBossRoom)}.{nameof(TrapSpawnerProxy)}}}"));
    // public TrapTrigger BossTrigger => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{BossTriggerName}")?.gameObject.GetComponent<TrapTrigger>();

    //public void AddBoss(string prefabName, bool ignoreSpawnPoolOverrides = true)
    //{
    //  try
    //  {
    //    var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
    //    if (prefab)
    //    {
    //      BossSpawnPoint.SetIgnoreSpawnPoolOverrides(ignoreSpawnPoolOverrides);
    //      BossSpawnPoint.AddBoss(prefab);
    //    }
    //  }
    //  catch (Exception e)
    //  {
    //    Log.Error(this, e);
    //  }
    //}

    //public void AddEnemy(string prefabName)
    //{
    //  try
    //  {
    //    var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
    //    if (prefab)
    //    {
    //      RoomSpawnPool.
    //      BossSpawnPoint.AddBoss(prefab);
    //    }
    //  }
    //  catch (Exception e)
    //  {
    //    Log.Error(this, e);
    //  }
    //}
  }
}
