using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Trigger", 30)]
  public class TrapTrigger : MonoBehaviour, IEventLogger
  {
    [SerializeField]
    [HideInInspector]
    public bool _isTriggered;

    [Header("Config")]
    [Tooltip("Trigger fires right away."), SerializeField]
    [UsedImplicitly]
    public bool m_triggerOnStart;

    [Header("Spawners"), SerializeField]
    public List<GameObject> m_trapSpawners = new(0);

    public List<GameObject> TrapSpawners
    {
      get => m_trapSpawners;
      set => m_trapSpawners = value;
    }

    private void DisableMarkerMesh()
    {
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
      if (m_trapSpawners.Count == 0) return;

      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      {
        trapSpawner.LogEvent -= OnLogEvent;
      }
    }

    [UsedImplicitly]
    public void OnTriggered(Collider other)
    {
      if (_isTriggered) return;
      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Trap Triggered **");
      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Triggered By: {other.name} **");
      DoSpawn();
    }

    private void DoSpawn()
    {
      if (_isTriggered) return;
      _isTriggered = true;
      if (m_trapSpawners.Count < 1) return;

      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      {
        if (!trapSpawner.enabled)
        {
          LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} Skipping: {trapSpawner.name}");
          continue;
        }

        trapSpawner.DoSpawn();
      }
    }

    public void SetIsTriggered(bool value)
    {
      _isTriggered = value;
    }

    [UsedImplicitly]
    public void Start()
    {
      LogTrace(($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Count : {m_trapSpawners.Count}"));
      DisableMarkerMesh();
      if (m_trapSpawners.Count == 0) return;

      var spawners = m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()).ToList();

      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>().Count : {spawners.Count}");
      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      {
        LogTrace("Logger Wire up");
        trapSpawner.LogEvent += OnLogEvent;
      }

      if (m_triggerOnStart) DoSpawn();
    }

    #region Logging

    [HideInInspector]
    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log($"[REMOVE] {logEventArgs.Message}"); // Todo: Remove
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
  }
}
