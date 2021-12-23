using Digitalroot.Valheim.TrapSpawners.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawn Pool", 32)]
  // ReSharper disable once ClassNeverInstantiated.Global
  public class TrapSpawnPool : MonoBehaviour, IEventLogger, ISpawnPool
  {
    [SerializeField, Tooltip("Collection of all the prefabs that can spawn")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<GameObject> m_spawnPoolPrefabs = new(0);

    #region Implementation of IEventLogger

    /// <inheritdoc />
    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log(logEventArgs.Message);
        LogEvent?.Invoke(this, logEventArgs);
      }
      catch (Exception e)
      {
        LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
      }
    }

    #endregion

    #region Implementation of ISpawnPool

    public void Clear() => m_spawnPoolPrefabs.Clear();

    public void AddEnemy(GameObject prefab) => m_spawnPoolPrefabs.Add(prefab);

    public void AddEnemy(string prefabName) => AddEnemy(ZNetScene.instance.GetPrefab(prefabName.GetStableHashCode()));

    public void AddBoss(GameObject prefab) => m_spawnPoolPrefabs.Add(prefab);

    public void AddBoss(string prefabName) => AddBoss(ZNetScene.instance.GetPrefab(prefabName.GetStableHashCode()));

    public void AddPrefab(GameObject prefab) => m_spawnPoolPrefabs.Add(prefab);

    public void AddPrefab(string prefabName) => AddPrefab(ZNetScene.instance.GetPrefab(prefabName.GetStableHashCode()));

    public int Count => m_spawnPoolPrefabs.Count;

    #endregion
  }
}
