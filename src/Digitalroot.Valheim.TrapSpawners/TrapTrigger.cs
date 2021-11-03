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
    private List<GameObject> m_trapSpawners = new List<GameObject>(0);

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

    [UsedImplicitly]
    public void OnTriggerEnter(Collider other)
    {
      if (_isTriggered) return;
      _isTriggered = true;
      if (m_trapSpawners.Count < 1) return;

      List<GameObject> spawnPool = null;
      if (m_useGlobalSpawnPool) // Global Pool
      {
        var globalSpawnPool = m_globalSpawnPoolPrefab.GetComponent<TrapSpawnPool>();
        spawnPool = globalSpawnPool.m_spawnPoolPrefabs;
      }

      if (!m_useGlobalSpawnPool && m_useTriggerSpawnPool) // Trigger Pool
      {
        spawnPool = m_spawnPoolPrefabs;
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
  }
}
