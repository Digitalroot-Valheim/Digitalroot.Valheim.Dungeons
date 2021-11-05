using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  [UsedImplicitly]
  public class TrapSpawnerProxy : AbstractProxy<TrapSpawner>, ISpawnPool
  {
    private const string RoomSpawnPointName = "Room_SpawnPoint";
    private static string GetPath(string roomName, string roomSpawnPointName) => $"Interior/Dungeon/Rooms/{roomName}/Traps/{roomSpawnPointName}";

    public TrapSpawnerProxy([NotNull] TrapSpawner trapSpawner)
      : base(trapSpawner)
    {
    }

    public TrapSpawnerProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] string roomSpawnPointName = RoomSpawnPointName)
      : base(dungeon.transform.Find(GetPath(roomName, roomSpawnPointName))
               ?.gameObject?.GetComponent<TrapSpawner>()
             ?? throw new NullReferenceException($"{nameof(TrapSpawnerProxy)} '{GetPath(roomName, roomSpawnPointName)}' not found."))
    {
    }

    #region Implementation of ISpawnPool

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

    public void SetIgnoreSpawnPoolOverrides(bool value) => RealObject.SetIgnoreSpawnPoolOverrides(value);
  }
}
