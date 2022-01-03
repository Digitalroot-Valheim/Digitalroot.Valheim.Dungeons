using Digitalroot.Valheim.TrapSpawners.Decorators;
using Digitalroot.Valheim.TrapSpawners.Enums;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  /// <summary>
  /// Needs the following saved to ZDOs
  /// - humanoid.m_health
  /// - humanoid.m_name
  /// - humanoid.m_faction
  /// - humanoid.m_boss
  /// - humanoid.m_jumpForce
  /// - humanoid.m_jumpForceForward
  /// - monsterAI.SetDespawnInDay(false);
  /// - monsterAI.SetPatrolPoint(prefab.transform.parent.transform.position);
  /// - monsterAI.m_jumpInterval
  /// - monsterAI.m_fleeIfLowHealth
  /// - monsterAI.m_pathAgentType
  /// - drop.m_levelMultiplier
  /// - drop.m_amountMax
  /// - AddComponent<AutoJumpLedge>()
  /// - level
  /// - m_scaleSize
  /// - Boss Auras
  /// </summary>
  [AddComponentMenu("Traps/Spawner", 31)]
  [UsedImplicitly, Serializable, DisallowMultipleComponent]
  public class TrapSpawner : EventLoggingMonoBehaviour
  {
    #region Unity Props

    /// <summary>
    /// Type of spawner.
    /// </summary>
    [Header("Configure")]
    [SerializeField, Tooltip("Hide the marker on spawn.")]
    private bool m_hideMarker = true;

    public SpawnerType m_spawnerType = SpawnerType.Enemy;

    /// <summary>
    /// Local Spawn Pool
    /// </summary>
    [SerializeField]
    private TrapSpawnPool m_spawnPool;

    /// <summary>
    /// Min number of prefabs to spawn
    /// </summary>
    [HideInInspector]
    public int m_quantityMin = 1;

    /// <summary>
    /// Max number of prefabs to spawn
    /// </summary>
    [HideInInspector]
    public int m_quantityMax = 1;

    /// <summary>
    /// Min level to spawn prefabs at.
    /// </summary>
    [HideInInspector]
    public int m_levelMin = 1;

    /// <summary>
    /// Max level to spawn prefabs at.
    /// </summary>
    [HideInInspector]
    public int m_levelMax = 1;

    /// <summary>
    /// Scale/Size to spawn prefabs at.
    /// </summary>
    [HideInInspector]
    public float m_scaleSize = 1;

    public ISpawnPool SpawnPool => m_spawnPool;

    private ZNetView m_nview;

    private IEnumerator _coroutine;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
      m_nview = GetComponent<ZNetView>();
    }

    public void Start()
    {
      DisableMarkerMesh();
      // SendMessageUpwards();  
    }

    #endregion

    private void DisableMarkerMesh()
    {
      if (!m_hideMarker) return;
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    #region Spawn

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1)
    {
      try
      {
        //LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs == null : {spawnPoolPrefabs == null}");
        //LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Using {m_spawnPool.name} Spawn Pool.");
          // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs == null : {m_spawnPool.m_spawnPoolPrefabs == null}");
          // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs.Count : {m_spawnPool.m_spawnPoolPrefabs?.Count}");
          spawnPoolPrefabs = m_spawnPool.m_spawnPoolPrefabs;
        }

        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] {m_spawnPool.name} is null or empty");
          return;
        }

        var quantity = GetQuantity(quantityMin, quantityMax);

        for (var i = 0; i < quantity; i++)
        {
          var rnd = Random.Range(0, spawnPoolPrefabs.Count);
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ({i + 1} of {quantity}), Using {rnd} from range 0 - {spawnPoolPrefabs.Count}, {m_spawnPool.name} size: {spawnPoolPrefabs.Count}");

          if (spawnPoolPrefabs.Count == 0)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] {m_spawnPool.name} is empty - Skipping Spawn");
            continue;
          }

          if (rnd >= spawnPoolPrefabs.Count)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] {m_spawnPool.name}[{rnd}] index is out of range. Using {m_spawnPool.name}[0].");
            rnd = 0;
          }

          switch (m_spawnerType)
          {
            case SpawnerType.MiniBoss:
              SpawnBoss(spawnPoolPrefabs, rnd, levelMin, levelMax, i);
              break;

            case SpawnerType.Enemy:
              SpawnEnemy(spawnPoolPrefabs, rnd, levelMin, levelMax, i);
              break;

            case SpawnerType.Destructible:
              SpawnDestructible(spawnPoolPrefabs, rnd, i);
              break;

            case SpawnerType.Treasure:
              SpawnTreasure(spawnPoolPrefabs, rnd, i);
              break;

            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    private int GetQuantity(int quantityMin, int quantityMax)
    {
      if (quantityMin == -1) quantityMin = m_quantityMin;
      if (quantityMax == -1) quantityMax = m_quantityMax;
      var quantity = quantityMin == quantityMax ? quantityMax : Random.Range(quantityMin, quantityMax + 1);
      return quantity;
    }

    public static IEnumerator ScaleEquipmentCoroutine(GameObject prefab)
    {
      yield return new WaitForSeconds(5);
      prefab.ScaleEquipment();
    }

    private void SpawnBoss(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i).AsBoss(m_scaleSize, levelMin, levelMax);

      _coroutine = ScaleEquipmentCoroutine(prefab);
      StartCoroutine(_coroutine);

      var humanoid = prefab.GetComponent<Humanoid>();
      var zNetView = prefab.GetComponent<ZNetView>();
      if (zNetView != null)
      {
        // zNetView.m_zdo.Save();
      }

      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ***************************************************************");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] @ {prefab.transform.position}, Scale: {prefab.transform.localScale}, Level: {humanoid?.GetLevel()}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] parent == null : {prefab.transform.parent == null}, parent?.name: {prefab.transform.parent?.name}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ***************************************************************");
    }

    private void SpawnDestructible(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ---------------------------------------------------------------");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} @ {prefab.transform.position}, Scale: {prefab.transform.localScale}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ---------------------------------------------------------------");
    }

    private void SpawnEnemy(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i).AsEnemy(m_scaleSize, levelMin, levelMax);

      var humanoid = prefab.GetComponent<Humanoid>();
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] @ {prefab.transform.position}, Scale: {prefab.transform.localScale}, Level: {humanoid?.GetLevel()}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] parent == null : {prefab.transform.parent == null}, parent?.name: {prefab.transform.parent?.name}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }

    private GameObject SpawnPrefab(GameObject spawnablePrefab, int i)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}]");
      var prefab = Instantiate(spawnablePrefab, transform.position, Quaternion.identity, transform)
                   .SetLocalPosition(i)
                   .SetLocalRotation(Quaternion.identity);
      return prefab;
    }

    private void SpawnTreasure(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ===============================================================");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {prefab.name} @ {prefab.transform.position}, Scale: {prefab.transform.localScale}");
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ===============================================================");
    }

    #endregion

    // public delegate void LogEventHandler(string message);
    // public event LogEventHandler OnServerResponse;
    // private void ServerResponded(string message)
    // {
    //   if (OnServerResponse != null)
    //   {
    //     Delegate[] subscribers = OnServerResponse.GetInvocationList();
    //     foreach (LogEventHandler subscriber in subscribers)
    //     {
    //       try
    //       {
    //         subscriber(message);
    //       }
    //       catch (Exception e)
    //       {
    //         HandleDelegateError(subscriber.Method, e);
    //       }
    //     }
    //   }
    // }
  }
}
