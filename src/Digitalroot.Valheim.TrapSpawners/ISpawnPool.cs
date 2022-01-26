using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners
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
    void AddMiniBoss(GameObject prefab);

    /// <summary>
    /// Adds a prefab and configures it as trash.
    /// </summary>
    /// <param name="prefabName"></param>
    void AddMiniBoss(string prefabName);

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

    int Count { get; }
  }
}
