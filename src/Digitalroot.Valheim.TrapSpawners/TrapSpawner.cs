using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawner", 31), DisallowMultipleComponent]
  public class TrapSpawner : MonoBehaviour
  {

    [SerializeField, Tooltip("Min number of prefabs to spawn"), Range(1, 5), Delayed]
    private int m_min = 1;
    [SerializeField, Tooltip("Max number of prefabs to spawn"), Range(1, 5), Delayed]
    private int m_max = 1;

    [SerializeField, Tooltip("Collection of all the prefabs that can spawn")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(1);

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int min = 0, int max = 0)
    {
      // Debug.Log($"spawnPoolPrefabs = null : {spawnPoolPrefabs == null}");
      // Debug.Log($"spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
      if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
      {
        Debug.Log("[TrapSpawners] Using Local TrapSpawners Spawnpool");
        spawnPoolPrefabs = m_spawnPoolPrefabs;
      }

      if (min == 0) min = m_min;
      if (max == 0) max = m_max;
      var quantity = min == max ? max : Random.Range(min, max+1);
    
      for (var i = 0; i < quantity; i++)
      {
        var rnd = Random.Range(0, spawnPoolPrefabs.Count);
        // Debug.Log($"rnd : {rnd}");
        
        var go = Instantiate(spawnPoolPrefabs[rnd], transform.root);
        go.transform.localPosition = transform.localPosition;
        go.transform.localPosition += Vector3.up * 0.25f;

        switch (i)
        {
          case 1:
            go.transform.localPosition += Vector3.left * 2.5f;
            break;
          case 2:
            go.transform.localPosition += Vector3.right * 2.5f;
            break;
          case 3:
            go.transform.localPosition += Vector3.forward * 2.5f;
            break;
          case 4:
            go.transform.localPosition += Vector3.back * 2.5f;
            break;
        }

        Debug.Log($"[TrapSpawners] Spawning: {go.name} @ {go.transform.position}");
      }
    }
  }
}
