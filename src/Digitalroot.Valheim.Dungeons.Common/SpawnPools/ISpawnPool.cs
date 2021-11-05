using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  public interface ISpawnPool
  {
    void Clear();

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefab"></param>
    void AddEnemy(GameObject prefab);

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    void AddEnemy(string prefabName);

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefab"></param>
    void AddBoss(GameObject prefab);

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    void AddBoss(string prefabName);

    /// <summary>
    /// Adds a prefab without configuration.
    /// </summary>
    /// <param name="prefab"></param>
    void AddPrefab(GameObject prefab);

    /// <summary>
    /// Adds a prefab without configuration.
    /// </summary>
    /// <param name="prefabName"></param>
    void AddPrefab(string prefabName);

    int SpawnPoolCount();
  }
}
