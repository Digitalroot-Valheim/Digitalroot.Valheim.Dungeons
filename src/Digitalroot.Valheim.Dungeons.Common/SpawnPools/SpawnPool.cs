using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.SpawnPools
{
  // public abstract class SpawnPool : ITraceableLogging, ISpawnPool
  // {
  //   // ReSharper disable once InconsistentNaming
  //   protected TrapSpawnPool _spawnPool;
  //
  //   private static GameObject ConfigureAsTrash(GameObject prefab) => DungeonsUtils.ConfigureAsTrash(prefab);
  //
  //   private static GameObject ConfigureAsBoss(GameObject prefab) => DungeonsUtils.ConfigureAsBoss(prefab);
  //
  //   public void Clear() => _spawnPool.Clear();
  //
  //   /// <summary>
  //   /// Adds a prefab and configures it as trash.
  //   /// </summary>
  //   /// <param name="prefab"></param>
  //   public void AddEnemy(GameObject prefab) => _spawnPool.m_spawnPoolPrefabs.Add(ConfigureAsTrash(prefab));
  //
  //   /// <summary>
  //   /// Adds a prefab and configures it as trash.
  //   /// </summary>
  //   /// <param name="prefabName"></param>
  //   public void AddEnemy(string prefabName) => _spawnPool.m_spawnPoolPrefabs.Add(ConfigureAsTrash(PrefabManager.Cache.GetPrefab<GameObject>(prefabName)));
  //
  //   /// <summary>
  //   /// Adds a prefab and configures it as trash.
  //   /// </summary>
  //   /// <param name="prefab"></param>
  //   public void AddBoss(GameObject prefab) => _spawnPool.m_spawnPoolPrefabs.Add(ConfigureAsBoss(prefab));
  //
  //   /// <summary>
  //   /// Adds a prefab and configures it as trash.
  //   /// </summary>
  //   /// <param name="prefabName"></param>
  //   public void AddBoss(string prefabName) => _spawnPool.m_spawnPoolPrefabs.Add(ConfigureAsBoss(PrefabManager.Cache.GetPrefab<GameObject>(prefabName)));
  //
  //   /// <summary>
  //   /// Adds a prefab without trash configuration.
  //   /// </summary>
  //   /// <param name="prefab"></param>
  //   public void AddPrefab(GameObject prefab) => _spawnPool.m_spawnPoolPrefabs.Add(prefab);
  //
  //   /// <summary>
  //   /// Adds a prefab and configures it as trash.
  //   /// </summary>
  //   /// <param name="prefabName"></param>
  //   public void AddPrefab(string prefabName) => _spawnPool.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));
  //
  //   #region Implementation of ITraceableLogging
  //
  //   /// <inheritdoc />
  //   public string Source => $"Digitalroot.Valheim.Dungeons.Common.{nameof(SpawnPool)}";
  //
  //   /// <inheritdoc />
  //   [UsedImplicitly]
  //   public bool EnableTrace { get; private set; }
  //
  //   public void SetEnableTrace(bool value)
  //   {
  //     EnableTrace = value;
  //   }
  //
  //   #endregion
  // }
}
