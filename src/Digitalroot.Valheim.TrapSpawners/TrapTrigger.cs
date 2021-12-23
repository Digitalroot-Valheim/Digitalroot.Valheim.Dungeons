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

      OnLogEvent(new LogEventArgs { Message = $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Trap Triggered **", LogLevel = LogLevel.Trace });
      OnLogEvent(new LogEventArgs { Message = $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Triggered By: {other.name} **", LogLevel = LogLevel.Trace });

      _isTriggered = true;
      if (m_trapSpawners.Count < 1) return;

      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      {
        if (!trapSpawner.enabled)
        {
          OnLogEvent(new LogEventArgs { Message = $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} Skipping: {trapSpawner.name}", LogLevel = LogLevel.Trace });
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
      Debug.LogWarning($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Count : {m_trapSpawners.Count}");

      DisableMarkerMesh();

      OnLogEvent(new LogEventArgs { LogLevel = LogLevel.Trace, Message = $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Count : {m_trapSpawners.Count}" });
      if (m_trapSpawners.Count == 0) return;

      var spawners = m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()).ToList();

      Debug.LogWarning($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>().Count : {spawners.Count}");
      OnLogEvent(new LogEventArgs { LogLevel = LogLevel.Trace, Message = $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>().Count : {spawners.Count}" });

      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      {
        OnLogEvent(new LogEventArgs { LogLevel = LogLevel.Trace, Message = $"Logger Wire up" });
        trapSpawner.LogEvent += OnLogEvent;
      }
    }

    #region Logging

    [HideInInspector]
    public event EventHandler<LogEventArgs> LogEvent;

    /// <inheritdoc />
    public void OnLogEvent(object sender, LogEventArgs logEventArgs)
    {
      try
      {
        Debug.Log(logEventArgs.Message);
        Debug.Log($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} LogEvent == null : {LogEvent == null}");

        if (LogEvent != null)
        {
          foreach (var d in LogEvent.GetInvocationList())
          {
            d.DynamicInvoke(sender, logEventArgs);
          }
        }

        // LogEvent?.Invoke(sender, logEventArgs);
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
  }
}
