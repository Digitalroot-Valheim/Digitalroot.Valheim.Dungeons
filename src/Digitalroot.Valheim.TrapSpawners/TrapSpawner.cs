using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawner", 31), DisallowMultipleComponent]
  [UsedImplicitly]
  // ReSharper disable once IdentifierTypo
  public class TrapSpawner : MonoBehaviour
  {
    [SerializeField, Tooltip("Min number of prefabs to spawn"), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    public int m_min = 1;
    [SerializeField, Tooltip("Max number of prefabs to spawn"), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    public int m_max = 1;

    [SerializeField, Tooltip("Force spawn point to use local Spawn Pool")]
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public bool m_ignoreSpawnPoolOverrides;

    [SerializeField, Tooltip("Collection of all the prefabs that can spawn")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(1);

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int min = 0, int max = 0)
    {
      // Debug.Log($"spawnPoolPrefabs = null : {spawnPoolPrefabs == null}");
      // Debug.Log($"spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
      if (m_ignoreSpawnPoolOverrides || spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
      {
        Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] Using Local Trap Spawners Spawn Pool");
        spawnPoolPrefabs = m_spawnPoolPrefabs;
      }

      if (min == 0) min = m_min;
      if (max == 0) max = m_max;
      var quantity = min == max ? max : Random.Range(min, max+1);
    
      for (var i = 0; i < quantity; i++)
      {
        var rnd = Random.Range(0, spawnPoolPrefabs.Count);
        Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] ({i} of {quantity}), Using {rnd} from range {min} - {max}, Spawn Pool size: {spawnPoolPrefabs.Count}");

        if (spawnPoolPrefabs.Count == 0)
        {
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] spawnPoolPrefabs is empty - Skipping Spawn");
          continue;
        }
        
        if (rnd >= spawnPoolPrefabs.Count)
        {
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] spawnPoolPrefabs[{rnd}] index is out of range. Using spawnPoolPrefabs[0].");
          rnd = 0;
        }
        var go = Instantiate(spawnPoolPrefabs[rnd], transform.root);
        go.transform.position = transform.position;
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

        // Fix Scale
        // go.transform.localScale

        Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}");
        
      }
    }
  }
}
