using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class DungeonBossRoom : DungeonRoom, ITraceableLogging
  {
    private const string BossTriggerName = "Boss_Trigger";
    private const string BossSpawnPointName = "Boss_SpawnPoint";
    private readonly RoomSpawnPool _spawnPool;

    public DungeonBossRoom(string name, GameObject dungeon)
      : base(name, dungeon)
    {
      try
      {
        _spawnPool = new RoomSpawnPool(dungeon, name);
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }

    public TrapSpawnerProxy BossSpawnPoint => new TrapSpawnerProxy(Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{BossSpawnPointName}")?.gameObject.GetComponent<TrapSpawner>() ?? throw new ArgumentNullException($"{nameof(DungeonBossRoom)}.{nameof(TrapSpawnerProxy)}}}"));
    public TrapTrigger BossTrigger => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{BossTriggerName}")?.gameObject.GetComponent<TrapTrigger>();

    public void AddBoss(string prefabName, bool ignoreSpawnPoolOverrides = true)
    {
      try
      {
        var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
        if (prefab)
        {
          BossSpawnPoint.SetIgnoreSpawnPoolOverrides(ignoreSpawnPoolOverrides);
          BossSpawnPoint.AddBoss(prefab);
        }
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }

    public void AddEnemy(string prefabName)
    {
      try
      {
        var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
        if (prefab)
        {
          RoomSpawnPool.
          BossSpawnPoint.AddBoss(prefab);
        }
      }
      catch (Exception e)
      {
        Log.Error(this, e);
      }
    }


    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => $"Digitalroot.Valheim.Dungeons.Common.{nameof(DungeonBossRoom)} ({Name})";

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
