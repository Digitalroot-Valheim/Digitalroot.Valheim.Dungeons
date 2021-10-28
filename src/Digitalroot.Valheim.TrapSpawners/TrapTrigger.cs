using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Traps/Trigger", 30)]
public class TrapTrigger : MonoBehaviour
{
  [SerializeField][HideInInspector] private bool _isTriggered;
  [SerializeField] private List<GameObject> m_trapSpawners = new List<GameObject>(0);

  [Header("Trigger Spawn Pool"), SerializeField]
  private bool m_useTriggerSpawnPool;

  [SerializeField] private List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(0);

  [Header("Global Spawn Pool"), SerializeField]
  private bool m_useGlobalSpawnPool;
  [SerializeField] private GameObject m_globalSpawnPoolPrefab;

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

    foreach (var spawner in m_trapSpawners)
    {
      var trapSpawner = spawner.GetComponent<TrapSpawner>();
      trapSpawner?.DoSpawn(spawnPool); // null = use Spawners Pool
    }
  }
}
