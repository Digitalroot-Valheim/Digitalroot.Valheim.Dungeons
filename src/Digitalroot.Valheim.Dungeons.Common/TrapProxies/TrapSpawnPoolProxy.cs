using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public class TrapSpawnPoolProxy : AbstractProxy<TrapSpawnPool>, ISpawnPool
  {
    private const string RoomSpawnPoolName = "Room_SpawnPool";

    private static string GetPath(string roomName, string roomSpawnPoolName) => $"Interior/Dungeon/Rooms/{roomName}/Traps/{roomSpawnPoolName}";

    // ReSharper disable once MemberCanBeProtected.Global
    public TrapSpawnPoolProxy([NotNull] TrapSpawnPool trapSpawnPool)
      : base(trapSpawnPool)
    {
    }

    public TrapSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] string roomSpawnPoolName = RoomSpawnPoolName)
      : base(dungeon.transform.Find(GetPath(roomName, roomSpawnPoolName))
               ?.gameObject?.GetComponent<TrapSpawnPool>()
             ?? throw new NullReferenceException($"{nameof(TrapSpawnPoolProxy)} '{GetPath(roomName, roomSpawnPoolName)}' not found."))
    {
    }


    #region Implementation of ISpawnPool

    /// <inheritdoc />
    public void Clear() => RealObject.Clear();

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
    public int SpawnPoolCount() => RealObject.m_spawnPoolPrefabs.Count;

    #endregion
  }
}
