using Digitalroot.Valheim.TrapSpawners;
using Jotunn.Managers;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  public class GlobalSpawnPool : SpawnPool
  {
    private const string GlobalSpawnPoolName = "GlobalSpawnPool";

    /// <inheritdoc />
    public GlobalSpawnPool(GameObject dungeon) 
      : base(dungeon)
    {
    }

    public TrapSpawnPool SpawnPool => Dungeon?.transform.Find(GlobalSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>();

    public void Clear() => SpawnPool.m_spawnPoolPrefabs.Clear();

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefab"></param>
    public void AddEnemy(GameObject prefab) => SpawnPool.m_spawnPoolPrefabs.Add(ConfigureAsTrash(prefab));

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    public void AddEnemy(string prefabName) => SpawnPool.m_spawnPoolPrefabs.Add(ConfigureAsTrash(PrefabManager.Cache.GetPrefab<GameObject>(prefabName)));

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefab"></param>
    public void AddBoss(GameObject prefab) => SpawnPool.m_spawnPoolPrefabs.Add(ConfigureAsBoss(prefab));

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    public void AddBoss(string prefabName) => SpawnPool.m_spawnPoolPrefabs.Add(ConfigureAsBoss(PrefabManager.Cache.GetPrefab<GameObject>(prefabName)));


    /// <summary>
    /// Adds a prefab without trash configuration.
    /// </summary>
    /// <param name="prefab"></param>
    public void AddPrefab(GameObject prefab) => SpawnPool.m_spawnPoolPrefabs.Add(prefab);

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    public void AddPrefab(string prefabName) => SpawnPool.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

  }
}
