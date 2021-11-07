using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public class TrapTriggerProxy : AbstractProxy<TrapTrigger>, ISpawnPool
  {
    private readonly StaticSourceLogger _logger = new StaticSourceLogger(true);
    private const string RoomTriggerName = "Room_Trigger";
    private static string GetPath(string roomName, string roomTriggerName) => $"Interior/Dungeon/Rooms/{roomName}/Traps/{roomTriggerName}";

    public TrapTriggerProxy(TrapTrigger realObject)
      : base(realObject)
    {

    }

    public TrapTriggerProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] string roomTriggerName = RoomTriggerName)
      : base(dungeon.transform.Find(GetPath(roomName, roomTriggerName))
            ?.gameObject?.GetComponent<TrapTrigger>()
             // ?? throw new NullReferenceException($"{nameof(TrapTriggerProxy)} '{GetPath(roomName, roomTriggerName)}' not found.")
             )
    {
      // RealObject.ShouldTrigger = ShouldTrigger;
    }

    private bool ShouldTrigger(Collider other)
    {
      var character = other.gameObject.GetComponent<Character>();
      Log.Trace(_logger, $"character == null : {character == null}");
      Log.Trace(_logger, $"character?.IsPlayer() : {character?.IsPlayer()}");
      return character != null && character.IsPlayer();
    }

    #region Implementation of ISpawnPool

    /// <inheritdoc />
    /// <inheritdoc />
    public void Clear() => RealObject.ClearSpawnPool();

    /// <inheritdoc />
    public void AddEnemy(GameObject prefab) => RealObject.AddToSpawnPool(DungeonsUtils.ConfigureAsTrash(prefab));

    /// <inheritdoc />
    public void AddEnemy(string prefabName) => AddEnemy(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddBoss(GameObject prefab) => RealObject.AddToSpawnPool(DungeonsUtils.ConfigureAsBoss(prefab));

    /// <inheritdoc />
    public void AddBoss(string prefabName) => AddBoss(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddPrefab(GameObject prefab) => RealObject.AddToSpawnPool(prefab);

    /// <inheritdoc />
    public void AddPrefab(string prefabName) => AddPrefab(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public int SpawnPoolCount() => RealObject.SpawnPoolCount();

    #endregion

    public void SetUseTriggerSpawnPool(bool value) => RealObject.SetUseTriggerSpawnPool(value);
    public void SetIsTriggered(bool value) => RealObject.SetIsTriggered(value);
    public void SetUseGlobalSpawnPool(bool value) => RealObject.SetUseGlobalSpawnPool(value);
    public void SetGlobalSpawnPool(GameObject value, bool enable = true) => RealObject.SetGlobalSpawnPool(value, enable);
    public List<TrapSpawnerProxy> GetSpawners() => RealObject.m_trapSpawners.Select(trapSpawner => new TrapSpawnerProxy(trapSpawner.GetComponent<TrapSpawner>())).ToList();
  }
}
