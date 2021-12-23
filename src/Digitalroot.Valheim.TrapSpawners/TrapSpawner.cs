using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawner", 31), DisallowMultipleComponent]
  [UsedImplicitly]
  // ReSharper disable once IdentifierTypo
  // public class TrapSpawner : CreatureSpawner, IEventLogger
  public class TrapSpawner : MonoBehaviour, IEventLogger
  {
    [Header("Quantity")]
    [SerializeField, Tooltip("Min number of prefabs to spawn"), Range(1, 5), Delayed]
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

    [Header("Level")]
    [SerializeField, Tooltip("Min level to spawn prefabs at."), Range(1, 9), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public int m_levelMin = 1;

    [SerializeField, Tooltip("Max level to spawn prefabs at."), Range(1, 9), Delayed]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public int m_levelMax = 1;

    [SerializeField, Tooltip("Spawn as a boss")]
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    public bool m_isBoss = false;

    // ReSharper disable once InconsistentNaming
    [Header("Size")]
    [UsedImplicitly, SerializeField, Tooltip("Scale/Size to spawn prefabs at."), Range(0.01f, 5f), Delayed]
    public float m_scaleSize = 1;

    [Header("Local Spawn Pool")]
    [SerializeField, Tooltip("Local Spawn Pool")]
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public TrapSpawnPool m_spawnPool = new();

    public ISpawnPool SpawnPool => m_spawnPool;

    private void DisableMarkerMesh()
    {
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1)
    {
      try
      {
        Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs == null : {spawnPoolPrefabs == null}");
        Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs.Count : {spawnPoolPrefabs?.Count}");
        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          OnLogEvent(new LogEventArgs
          {
            Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Using {m_spawnPool.name} Spawn Pool."
            , LogLevel = LogLevel.Debug
          });
          
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs == null : {m_spawnPool.m_spawnPoolPrefabs == null}");
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs.Count : {m_spawnPool.m_spawnPoolPrefabs?.Count}");
          spawnPoolPrefabs = m_spawnPool.m_spawnPoolPrefabs;
        }

        if (spawnPoolPrefabs == null || spawnPoolPrefabs.Count < 1)
        {
          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs is null or empty");
          return;
        }

        if (quantityMin == -1) quantityMin = m_quantityMin;
        if (quantityMax == -1) quantityMax = m_quantityMax;
        var quantity = quantityMin == quantityMax ? quantityMax : Random.Range(quantityMin, quantityMax + 1);

        for (var i = 0; i < quantity; i++)
        {
          var rnd = Random.Range(0, spawnPoolPrefabs.Count);
          OnLogEvent(new LogEventArgs
          {
            Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] ({i + 1} of {quantity}), Using {rnd} from range 0 - {spawnPoolPrefabs.Count}, Spawn Pool size: {spawnPoolPrefabs.Count}"
            , LogLevel = LogLevel.Debug
          });

          if (spawnPoolPrefabs.Count == 0)
          {
            OnLogEvent(new LogEventArgs
            {
              Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs is empty - Skipping Spawn"
              , LogLevel = LogLevel.Debug
            });
            continue;
          }

          if (rnd >= spawnPoolPrefabs.Count)
          {
            OnLogEvent(new LogEventArgs
            {
              Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] spawnPoolPrefabs[{rnd}] index is out of range. Using spawnPoolPrefabs[0]."
              , LogLevel = LogLevel.Debug
            });
            rnd = 0;
          }

          OnLogEvent(new LogEventArgs
          {
            Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Parent Transform : {transform.position}"
            , LogLevel = LogLevel.Debug
          });

          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] m_spawnPool.m_spawnPoolPrefabs.Count : {m_spawnPool.m_spawnPoolPrefabs?.Count}");

          var go = Instantiate(spawnPoolPrefabs[rnd], transform.position, Quaternion.identity, transform);
          go.transform.localScale = new Vector3(m_scaleSize, m_scaleSize, m_scaleSize);
          var humanoid =  go.GetComponent<Humanoid>();
          
          if (m_isBoss)
          {
            humanoid.m_boss = m_isBoss;
            humanoid.m_health *= Convert.ToSingle(Math.Pow(m_scaleSize, 2));
          }

          Debug.Log($"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Loc Point : {transform.position}, Spawn Loc : {go.transform.position}, isBoss : {humanoid.m_boss}, Health {humanoid.m_health}");

          switch (i)
          {
            case 1:
              // go.transform.position += Vector3.left * 2.5f * m_scaleSize;
              go.transform.localPosition += Vector3.left * 2.5f;// * m_scaleSize;
              break;

            case 2:
              // go.transform.position += Vector3.right * 2.5f * m_scaleSize;
              go.transform.localPosition += Vector3.right * 2.5f;// * m_scaleSize;
              break;

            case 3:
              // go.transform.position += Vector3.forward * 2.5f * m_scaleSize;
              go.transform.localPosition += Vector3.forward * 2.5f;// * m_scaleSize;
              break;

            case 4:
              // go.transform.position += Vector3.back * 2.5f * m_scaleSize;
              go.transform.localPosition += Vector3.back * 2.5f;// * m_scaleSize;
              break;
          }

          if (levelMin == -1) levelMin = m_levelMin;
          if (levelMax == -1) levelMax = m_levelMax;
          var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
          go.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);

          // go.transform.localPosition += Vector3.up * 0.025f;
          var monsterAI = go.GetComponent<MonsterAI>();
          monsterAI?.SetDespawnInDay(false);
          monsterAI?.SetPatrolPoint(transform.position);

          OnLogEvent(new LogEventArgs
          {
            Message = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}.{name}] Spawning: {go.name} @ {go.transform.position}, Scale: {go.transform.localScale}, Level: {level}"
            , LogLevel = LogLevel.Debug
          });
        }
      }
      catch (Exception e)
      {
        OnLogEvent(new LogEventArgs
        {
          Message = e.Message
          , Exception = e
          , LogLevel = LogLevel.Error
        });
      }
    }

    public void Start()
    {
      DisableMarkerMesh();
    }

    #region Logging

    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log(logEventArgs.Message);
        Debug.Log($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} LogEvent == null : {LogEvent == null}");
        LogEvent?.Invoke(this, logEventArgs);
      }
      catch (Exception e)
      {
        LoggingUtils.HandleDelegateError(LogEvent?.Method, e);
      }
    }

    private void OnLogEvent(LogEventArgs logEventArgs)
    {
      OnLogEvent(this, logEventArgs);
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
