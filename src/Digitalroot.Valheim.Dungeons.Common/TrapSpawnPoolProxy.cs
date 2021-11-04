using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class TrapSpawnPoolProxy : TrapSpawnPool, ISpawnPool
  {
    private readonly TrapSpawnPool _realTrapSpawnPool;

    // ReSharper disable twice IdentifierTypo
    public TrapSpawnPoolProxy([NotNull] TrapSpawnPool trapSpawnPool)
    {
      _realTrapSpawnPool = trapSpawnPool;
    }

    #region Implementation of ISpawnPool

    /// <inheritdoc />
    public void Clear() => _realTrapSpawnPool.Clear();

    /// <inheritdoc />
    public void AddEnemy(GameObject prefab) => _realTrapSpawnPool. _realTrapSpawner.AddToSpawnPool(DungeonsUtils.ConfigureAsTrash(prefab));

    /// <inheritdoc />
    public void AddEnemy(string prefabName) => AddEnemy(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddBoss(GameObject prefab) => _realTrapSpawner.AddToSpawnPool(DungeonsUtils.ConfigureAsBoss(prefab));

    /// <inheritdoc />
    public void AddBoss(string prefabName) => AddBoss(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddPrefab(GameObject prefab) => _realTrapSpawner.AddToSpawnPool(prefab);

    /// <inheritdoc />
    public void AddPrefab(string prefabName) => AddPrefab(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    #endregion
  }
}
