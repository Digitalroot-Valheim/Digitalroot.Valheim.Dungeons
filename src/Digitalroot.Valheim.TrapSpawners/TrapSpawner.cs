using Digitalroot.Valheim.TrapSpawners.CMB;
using Digitalroot.Valheim.TrapSpawners.Enums;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using Digitalroot.Valheim.TrapSpawners.Models;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming

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
  /// - humanoid.level
  /// - monsterAI.SetDespawnInDay(false);
  /// - monsterAI.SetPatrolPoint(prefab.transform.parent.transform.position);
  /// - monsterAI.m_jumpInterval
  /// - monsterAI.m_fleeIfLowHealth
  /// - monsterAI.m_pathAgentType
  /// - drop.m_levelMultiplier
  /// - drop.m_amountMax
  /// - AddComponent<AutoJumpLedge>()
  /// - m_scaleSize
  /// - Boss Auras
  /// </summary>
  [AddComponentMenu("Traps/Spawner", 31)]
  [UsedImplicitly, Serializable, DisallowMultipleComponent]
  [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
  public class TrapSpawner : EventLoggingMonoBehaviour
  {
    private string m_uniqueName;

    #region Unity Props

    /// <summary>
    /// Type of spawner.
    /// </summary>
    [Header("Configure")]
    [SerializeField, Tooltip("Hide the marker on spawn.")]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "UsedImplicitly")]
    private bool m_hideMarker = true;

    public SpawnerType m_spawnerType = SpawnerType.Enemy;

    /// <summary>
    /// Local Spawn Pool
    /// </summary>
    [SerializeField]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "UsedImplicitly")]
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

    /// <summary>
    /// Scale health to spawn prefabs at. m_health =* m_level * m_healthMultiplier * (m_level ^ m_healthExponent)
    /// </summary>
    [HideInInspector]
    public float m_healthMultiplier = 1;

    /// <summary>
    /// Scale health exponentially. m_health =* m_level * m_healthMultiplier * (m_level ^ m_healthExponent)
    /// </summary>
    [HideInInspector]
    public int m_healthExponent  = 1;

    private ZNetView m_nview;

    private bool IsCreatureSpawner => m_spawnerType is SpawnerType.Enemy or SpawnerType.MiniBoss;
    public ISpawnPool SpawnPool => m_spawnPool;

    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "UsedImplicitly")]
    private IEnumerator _coroutine;

    public readonly List<DungeonCreatureData> _spawnedGameObjectList = new();

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
      m_nview = gameObject.GetComponent<ZNetView>() ?? transform.root?.GetComponent<ZNetView>();
      m_uniqueName = gameObject.GetUniqueName();
      if (m_nview == null)
      {
        LogError(new Exception($"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name} m_nview is null"));
        return;
      }

      if (m_nview.GetZDO() == null) return;
      m_nview.Register(FormatNameOfRPC(nameof(RPC_RequestOwn)), RPC_RequestOwn);
    }

    public void Start()
    {
      DisableMarkerMesh();
    }

    //private void OnEnable()
    //{
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}]");
    //  // _coroutine = Pulse(gameObject); // Pulse to find mobs.
    //  // StartCoroutine(_coroutine);
    //}

    #endregion

    private void DisableMarkerMesh()
    {
      if (!m_hideMarker) return;
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    //public IEnumerable<Collider> FindNearByDungeonCreatures(float radius) => Common.Utils.FindNearByDungeonCreaturesByOverlapSphereNonAlloc(gameObject.transform.position, radius);

    private string FormatNameOfRPC(string rpcName)
    {
      return $"{rpcName}.{m_uniqueName}";
    }

    private void RPC_RequestOwn(long sender)
    {
      if (m_nview.IsOwner())
      {
        m_nview.GetZDO().SetOwner(sender);
      }
    }

    #region Spawn

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1)
    {
      try
      {
        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Using {m_spawnPool.name} Spawn Pool.");
          spawnPoolPrefabs = m_spawnPool.m_spawnPoolPrefabs;
        }

        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {m_spawnPool.name} is null or empty");
          return;
        }

        var quantity = GetQuantity(quantityMin, quantityMax);

        if (levelMin == -1) levelMin = m_levelMin;
        if (levelMax == -1) levelMax = m_levelMax;

        for (var i = 0; i < quantity; i++)
        {
          var rnd = Random.Range(0, spawnPoolPrefabs.Count);
          LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ({i + 1} of {quantity}), Using {rnd} from range 0 - {spawnPoolPrefabs.Count}, {m_spawnPool.name} size: {spawnPoolPrefabs.Count}");

          if (spawnPoolPrefabs.Count == 0)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {m_spawnPool.name} is empty - Skipping Spawn");
            continue;
          }

          if (rnd >= spawnPoolPrefabs.Count)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {m_spawnPool.name}[{rnd}] index is out of range. Using {m_spawnPool.name}[0].");
            rnd = 0;
          }

          switch (m_spawnerType)
          {
            case SpawnerType.MiniBoss:
              SpawnMiniBoss(spawnPoolPrefabs, rnd, levelMin, levelMax, i);
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

    private int GetLevel(int levelMin, int levelMax)
    {
      if (levelMin == -1) levelMin = m_levelMin;
      if (levelMax == -1) levelMax = m_levelMax;
      var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
      return level;
    }

    //public static IEnumerator Pulse(GameObject gameObject)
    //{
    //  const int delay = 5;
    //  const int maxLoops = 6;
    //  var cnt = 0;
    //  yield return new WaitForSeconds(2);
    //  yield return cnt++;
    //  yield return OnPulse(cnt, gameObject);
    //  yield return new WaitForSeconds(delay);
    //  yield return cnt++;
    //  yield return OnPulse(cnt, gameObject);
    //  for (var i = cnt; i < maxLoops; i++)
    //  {
    //    yield return new WaitForSeconds(delay * cnt);
    //    yield return cnt++;
    //    yield return OnPulse(cnt, gameObject);
    //  }
    //}

    //private static bool OnPulse(int cnt, GameObject gameObject)
    //{
    //  // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] cnt : {cnt}");
    //  // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] gameObject == null : {gameObject == null}");

    //  var mobs = gameObject?.GetComponent<TrapSpawner>()?.FindNearByDungeonCreatures(Convert.ToSingle(Math.Pow(cnt, 2d))).ToList();

    //  // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] mobs == null : {mobs == null}");

    //  if (mobs == null) return true;

    //  // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] mobs?.Count() : {mobs.Count}");

    //  foreach (var mob in mobs)
    //  {
    //    // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] mob : {mob.gameObject.name}");
    //    // LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}] IsDungeonCreature : {mob.gameObject.IsDungeonCreature()}");
    //    if (mob.gameObject.IsDungeonCreature())
    //    {
    //      mob.gameObject.GetOrAddMonoBehaviour<DungeonCreature>();
    //    }
    //  }

    //  return true;
    //}

    public static IEnumerator ScaleEquipmentCoroutine(GameObject prefab)
    {
      yield return new WaitForSeconds(5);
      prefab.ScaleEquipment();
    }

    private void SpawnMiniBoss(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {levelMin}/{levelMax}");
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      prefab.SetLevel(levelMin, levelMax);
      prefab.SetMaxHealth(m_healthMultiplier, m_healthExponent);

      var humanoid = prefab.GetComponent<Humanoid>();

      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ***************************************************************");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] @ {prefab.transform.position}, Scale: {prefab.transform.localScale}, Level: {humanoid?.GetLevel()}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] parent == null : {prefab.transform.parent == null}, parent?.name: {prefab.transform.parent?.name}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ***************************************************************");
    }

    private void SpawnDestructible(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      var staticPhysics = prefab.GetComponent<StaticPhysics>();
      staticPhysics.m_fall = false;

      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ---------------------------------------------------------------");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} @ {prefab.transform.position}, Scale: {prefab.transform.localScale}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ---------------------------------------------------------------");
    }

    private void SpawnEnemy(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {levelMin}/{levelMax}");
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      prefab.SetLevel(levelMin, levelMax);
      prefab.SetMaxHealth(m_healthMultiplier, m_healthExponent);

      var humanoid = prefab.GetComponent<Humanoid>();
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] @ {prefab.transform.position}, Scale: {prefab.transform.localScale}, Level: {humanoid?.GetLevel()}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} [{humanoid?.m_name}] parent == null : {prefab.transform.parent == null}, parent?.name: {prefab.transform.parent?.name}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }

    private GameObject SpawnPrefab(GameObject spawnablePrefab, int i)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] {spawnablePrefab.name}");
      // var prefab = Instantiate(spawnablePrefab, transform.position, Quaternion.identity, transform)
      var prefab = Instantiate(spawnablePrefab, transform.position, Quaternion.identity)
                   .SetLocalPosition(i)
                   .SetLocalRotation(Quaternion.identity)
                   .SetLocalScale(m_scaleSize);

      return prefab;
    }

    private void SpawnTreasure(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int i)
    {
      var prefab = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ===============================================================");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] Spawning: {prefab.name} @ {prefab.transform.position}, Scale: {prefab.transform.localScale}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}] ===============================================================");
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
