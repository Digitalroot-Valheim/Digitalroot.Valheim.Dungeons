using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Trigger", 30)]
  public class TrapTrigger : MonoBehaviour
  {
    [SerializeField] [HideInInspector] private bool _isTriggered;
    
    [SerializeField]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once CollectionNeverUpdated.Local
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<GameObject> m_trapSpawners = new List<GameObject>(0);

    [Header("Trigger Spawn Pool"), SerializeField]
    // ReSharper disable once InconsistentNaming
#pragma warning disable 649
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public bool m_useTriggerSpawnPool;
#pragma warning restore 649

    [SerializeField]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(0);

    [Header("Global Spawn Pool"), SerializeField]
    // ReSharper disable once InconsistentNaming
#pragma warning disable 649
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public bool m_useGlobalSpawnPool; // ToDo: Wrap this in a public toggle method. 
#pragma warning restore 649

    [SerializeField] 
    // ReSharper disable once InconsistentNaming
#pragma warning disable 649
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public GameObject m_globalSpawnPoolPrefab;
#pragma warning restore 649

    public void AddToSpawnPool(GameObject prefab)
    {
      m_spawnPoolPrefabs.Add(prefab);
    }

    public void ClearSpawnPool()
    {
      m_spawnPoolPrefabs.Clear();
    }

    [HideInInspector]
    // public Func<Collider, bool> ShouldTrigger = (collider => false );

    [UsedImplicitly]
    public void OnTriggerEnter(Collider other)
    {
      if (_isTriggered) return;
      Debug.Log($"** Trap Triggered **");
      Debug.Log($"** Triggered By: {other.name} **");

      if (!(other.name == "Player(Clone)" || other.name == "Player")) return;

      _isTriggered = true;
      if (m_trapSpawners.Count < 1) return;

      List<GameObject> spawnPool = null;
      switch (m_useGlobalSpawnPool)
      {
        // Global Pool
        case true:
          var globalSpawnPool = m_globalSpawnPoolPrefab.GetComponent<TrapSpawnPool>();
          spawnPool = globalSpawnPool.m_spawnPoolPrefabs;
          break;
        
        // Trigger Pool
        case false when m_useTriggerSpawnPool:
          spawnPool = m_spawnPoolPrefabs;
          break;
      }

      // ReSharper disable once IdentifierTypo
      foreach (var spawner in m_trapSpawners)
      {
        // ReSharper disable once IdentifierTypo
        var trapSpawner = spawner.GetComponent<TrapSpawner>();
        // ReSharper disable once CommentTypo
        trapSpawner?.DoSpawn(spawnPool); // null = use Spawners Pool
      }
    }

    public void SetGlobalSpawnPool(GameObject value, bool enable = true)
    {
      m_globalSpawnPoolPrefab = value;
      SetUseGlobalSpawnPool(enable);
    }

    public void SetIsTriggered(bool value)
    {
      _isTriggered = value;
    }

    public void SetUseGlobalSpawnPool(bool value)
    {
      m_useGlobalSpawnPool = value;
    }

    public void SetUseTriggerSpawnPool(bool value)
    {
      m_useTriggerSpawnPool = value;
    }

    public int SpawnPoolCount() => m_spawnPoolPrefabs.Count;

  }
}
