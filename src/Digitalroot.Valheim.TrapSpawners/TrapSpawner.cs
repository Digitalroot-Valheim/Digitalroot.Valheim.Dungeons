using JetBrains.Annotations;
using System;
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
    [Header("Quantity")] [SerializeField, Tooltip("Min number of prefabs to spawn"), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConvertToConstant.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int m_quantityMin = 1;

    [SerializeField, Tooltip("Max number of prefabs to spawn"), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConvertToConstant.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int m_quantityMax = 1;

    [Header("Level")] [SerializeField, Tooltip("Min level to spawn prefabs at."), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public int m_levelMin = 1;

    [SerializeField, Tooltip("Max level to spawn prefabs at."), Range(1, 5), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public int m_levelMax = 1;

    // ReSharper disable once InconsistentNaming
    [Header("Scale Size")] [UsedImplicitly, SerializeField, Tooltip("Scale/Size to spawn prefabs at."), Range(0.01f, 5f), Delayed]
    public float m_scaleSize = 1;

    [Header("Local Spawn Pool")] [SerializeField, Tooltip("Force spawner to use local Spawn Pool")]
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
#pragma warning disable 649
    private bool m_ignoreSpawnPoolOverrides;
#pragma warning restore 649

    [SerializeField, Tooltip("Collection of all the prefabs that can spawn")] [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Unity Editor Field")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    private List<GameObject> m_spawnPoolPrefabs = new(0);

    public void AddToSpawnPool(GameObject prefab) => m_spawnPoolPrefabs.Add(prefab);

    public void ClearSpawnPool() => m_spawnPoolPrefabs.Clear();

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1)
    {
      try
      {
        // Debug.Log($"spawnPoolPrefabs = null : {spawnPoolPrefabs == null}");
        // Debug.Log($"spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
        if (m_ignoreSpawnPoolOverrides || spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] Using Local Trap Spawners Spawn Pool");
          spawnPoolPrefabs = m_spawnPoolPrefabs;
        }

        if (quantityMin == -1) quantityMin = m_quantityMin;
        if (quantityMax == -1) quantityMax = m_quantityMax;
        var quantity = quantityMin == quantityMax ? quantityMax : Random.Range(quantityMin, quantityMax + 1);

        for (var i = 0; i < quantity; i++)
        {
          var rnd = Random.Range(0, spawnPoolPrefabs.Count);
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] ({i + 1} of {quantity}), Using {rnd} from range 0 - {spawnPoolPrefabs.Count}, Spawn Pool size: {spawnPoolPrefabs.Count}");

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

          var go = Instantiate(spawnPoolPrefabs[rnd]);
          // var go = Instantiate(spawnPoolPrefabs[rnd], transform);
          go.transform.position = transform.position;

          switch (i)
          {
            case 1:
              go.transform.localPosition += Vector3.left * 2.5f * m_scaleSize;
              break;
            case 2:
              go.transform.localPosition += Vector3.right * 2.5f * m_scaleSize;
              break;
            case 3:
              go.transform.localPosition += Vector3.forward * 2.5f * m_scaleSize;
              break;
            case 4:
              go.transform.localPosition += Vector3.back * 2.5f * m_scaleSize;
              break;
          }

          if (levelMin == -1) levelMin = m_levelMin;
          if (levelMax == -1) levelMax = m_levelMax;
          var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
          // go.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);
          go.SendMessage("SetLevel", 3, SendMessageOptions.RequireReceiver);

          // var character = go.GetComponent("Character");
          // if (character != null)
          // {
          //   character.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);
          //   // Invoke("SetLevel", );
          //   // character?.SetLevel(4); // Need to do this at spawn time. 
          // }


          // Fix Scale
          // var localScale = go.transform.localScale;
          // localScale.y *= 59.6338481722f;
          go.transform.localPosition += Vector3.up * 0.025f;

          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}, Level: {level}");
        }
      }
      catch (Exception e)
      {
        Debug.LogError(e);
      }
    }

    public void SetIgnoreSpawnPoolOverrides(bool value) => m_ignoreSpawnPoolOverrides = value;

    public int SpawnPoolCount() => m_spawnPoolPrefabs.Count;

  }
}
