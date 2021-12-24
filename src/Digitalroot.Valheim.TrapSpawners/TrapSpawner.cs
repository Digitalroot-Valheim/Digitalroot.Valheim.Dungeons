using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawner", 31), DisallowMultipleComponent]
  [UsedImplicitly]
  public class TrapSpawner : MonoBehaviour, IEventLogger
  {
    #region Unity

    [Header("Details")]
    [SerializeField, Tooltip("Is the spawn deco?")]
    public bool m_isDeco = false;

    [SerializeField, Tooltip("Spawn as a boss")]
    public bool m_isBoss = false;

    [Header("Quantity")]
    [SerializeField, Tooltip("Min number of prefabs to spawn"), Range(1, 5), Delayed]
    public int m_quantityMin = 1;

    [SerializeField, Tooltip("Max number of prefabs to spawn"), Range(1, 5), Delayed]
    public int m_quantityMax = 1;

    [Header("Level")]
    [SerializeField, Tooltip("Min level to spawn prefabs at."), Range(1, 9), Delayed]
    public int m_levelMin = 1;

    [SerializeField, Tooltip("Max level to spawn prefabs at."), Range(1, 9), Delayed]
    public int m_levelMax = 1;

    [Header("Size")]
    [UsedImplicitly, SerializeField, Tooltip("Scale/Size to spawn prefabs at."), Range(0.01f, 5f), Delayed]
    public float m_scaleSize = 1;

    [Header("Local Spawn Pool")]
    [SerializeField, Tooltip("Local Spawn Pool")]
    public TrapSpawnPool m_spawnPool = new();

    public ISpawnPool SpawnPool => m_spawnPool;

    #endregion

    private void DisableMarkerMesh()
    {
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    public void Start()
    {
      DisableMarkerMesh();
    }

    #region Spawn

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1)
    {
      try
      {
        LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs == null : {spawnPoolPrefabs == null}");
        LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Using {m_spawnPool.name} Spawn Pool.");
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs == null : {m_spawnPool.m_spawnPoolPrefabs == null}");
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs.Count : {m_spawnPool.m_spawnPoolPrefabs?.Count}");
          spawnPoolPrefabs = m_spawnPool.m_spawnPoolPrefabs;
        }

        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs is null or empty");
          return;
        }

        var quantity = GetQuantity(quantityMin, quantityMax);

        for (var i = 0; i < quantity; i++)
        {
          var rnd = Random.Range(0, spawnPoolPrefabs.Count);
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ({i + 1} of {quantity}), Using {rnd} from range 0 - {spawnPoolPrefabs.Count}, Spawn Pool size: {spawnPoolPrefabs.Count}");

          if (spawnPoolPrefabs.Count == 0)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs is empty - Skipping Spawn");
            continue;
          }

          if (rnd >= spawnPoolPrefabs.Count)
          {
            LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs[{rnd}] index is out of range. Using spawnPoolPrefabs[0].");
            rnd = 0;
          }

          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Parent Transform : {transform.position}");
          LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs.Count : {m_spawnPool.m_spawnPoolPrefabs?.Count}");

          if (m_isBoss)
          {
            SpawnBoss(spawnPoolPrefabs, rnd, levelMin, levelMax, i);
          }
          else if (m_isDeco)
          {
            SpawnDeco(spawnPoolPrefabs, rnd, i);
          }
          else
          {
            SpawnEnemy(spawnPoolPrefabs, rnd, levelMin, levelMax, i);
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

    public static string GenerateName(int len)
    {
      System.Random r = new();
      string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
      string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
      var Name = "";
      Name += consonants[r.Next(consonants.Length)].ToUpper();
      Name += vowels[r.Next(vowels.Length)];
      var b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
      while (b < len)
      {
        Name += consonants[r.Next(consonants.Length)];
        b++;
        Name += vowels[r.Next(vowels.Length)];
        b++;
      }

      return Name;
    }

    private void SetAI(GameObject go)
    {
      var monsterAI = go.GetComponent<MonsterAI>();
      monsterAI?.SetDespawnInDay(false);
      monsterAI?.SetPatrolPoint(transform.position);
    }

    private int SetLevel(int levelMin, int levelMax, GameObject go)
    {
      if (levelMin == -1) levelMin = m_levelMin;
      if (levelMax == -1) levelMax = m_levelMax;
      var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
      go.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);
      return level;
    }

    private static void SetLocalPosition(int i, GameObject go)
    {
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
    }

    private GameObject SpawnPrefab(GameObject prefab, int i)
    {
      var go = Instantiate(prefab, transform.position, Quaternion.identity, transform);
      go.transform.localScale = new Vector3(m_scaleSize, m_scaleSize, m_scaleSize);
      SetLocalPosition(i, go);
      return go;
    }

    private void SpawnBoss(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      var go = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      var humanoid = go.GetComponent<Humanoid>();
      if (humanoid != null)
      {
        humanoid.m_boss = m_isBoss;
        humanoid.m_health *= Convert.ToSingle(Math.Pow(m_scaleSize, 2));
        humanoid.m_name = $"{GenerateName(Random.Range(4, 9))} the {humanoid.m_name}";
      }

      var level = SetLevel(levelMin, levelMax, go);
      SetAI(go);
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}, Level: {level}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
    }

    private void SpawnDeco(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int i)
    {
      var go = SpawnPrefab(spawnPoolPrefabs[rnd], i);
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}");
    }

    private void SpawnEnemy(IReadOnlyList<GameObject> spawnPoolPrefabs, int rnd, int levelMin, int levelMax, int i)
    {
      var go = SpawnPrefab(spawnPoolPrefabs[rnd], i);

      var humanoid = go.GetComponent<Humanoid>();
      if (humanoid != null)
      {
        humanoid.m_health *= 2;
        humanoid.m_name = $"Dark {humanoid.m_name}";
      }

      var level = SetLevel(levelMin, levelMax, go);
      SetAI(go);
      LogTrace($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}, Level: {level}, isBoss : {humanoid?.m_boss}, Health {humanoid?.m_health}");
    }

    #endregion

    #region Logging

    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log(logEventArgs.Message);
        Debug.Log($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} LogEvent == null : {LogEvent == null}");
        LogEvent?.Invoke(sender, logEventArgs);
      }
      catch (Exception e)
      {
        LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
      }
    }

    private void OnLogEvent(LogEventArgs logEventArgs) => OnLogEvent(this, logEventArgs);
    private void LogError(Exception e) => OnLogEvent(new LogEventArgs { Message = e.Message, Exception = e, LogLevel = LogLevel.Error });
    private void LogTrace(string msg) => Log(msg, LogLevel.Trace);
    private void Log(string msg, LogLevel logLevel) => OnLogEvent(new LogEventArgs { Message = msg, LogLevel = logLevel });

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
