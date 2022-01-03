using Digitalroot.Valheim.TrapSpawners.Extensions;
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
  /// <summary>
  /// Needs the following saved to ZDOs
  /// - _isTriggered
  /// </summary>
  [AddComponentMenu("Traps/Trigger", 30)]
  [UsedImplicitly, Serializable, DisallowMultipleComponent]
  public class TrapTrigger : EventLoggingMonoBehaviour
  {
    private const string Namespace = "Digitalroot.Valheim.TrapSpawners";

    private ZNetView m_nview;
    private string m_uniqueName;

    private bool _isTriggered;

    [Header("Config")]
    [SerializeField, Tooltip("Hide the marker on spawn.")]
    private bool m_hideMarker = true;

    [Tooltip("Trigger fires right away."), SerializeField]
    [UsedImplicitly]
    public bool m_triggerOnStart;

    [Header("Spawners"), SerializeField]
    private List<GameObject> m_trapSpawners = new(0);

    public bool IsTriggered
    {
      get => _isTriggered;
      set
      {
        _isTriggered = value;
        if (m_nview.IsOwner())
        {
          m_nview.GetZDO().Set(nameof(_isTriggered), value);
        }
      }
    }

    public List<GameObject> TrapSpawners
    {
      get => m_trapSpawners;
      set => m_trapSpawners = value;
    }

    #region MonoBehaviour Overrides

    public void Awake()
    {
      m_nview = gameObject.GetComponent<ZNetView>() ?? transform.root?.GetComponent<ZNetView>();
      m_uniqueName = gameObject.GetUniqueName();
      if (m_nview == null)
      {
        LogError(new Exception($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_nview is null"));
        return;
      }

      if (m_nview.GetZDO() == null)
      {
        return;
      }
      m_nview.Register<bool>(FormatNameOfRPC(nameof(RPC_SetIsTriggered)), RPC_SetIsTriggered);
      m_nview.Register(FormatNameOfRPC(nameof(RPC_RequestOwn)), RPC_RequestOwn);
      IsTriggered = (m_nview.GetZDO().GetBool(nameof(_isTriggered)));
    }

    [UsedImplicitly]
    public void Start()
    {
      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Count : {m_trapSpawners.Count}");

      DisableMarkerMesh();
      if (m_trapSpawners.Count == 0) return;

      var spawners = m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()).ToList();

      //LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>().Count : {spawners.Count}");
      //foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner.GetComponent<TrapSpawner>()))
      //{
      //  LogTrace($"Logger Wire up for {trapSpawner.name}");
      //  trapSpawner.LogEvent += OnLogEvent;
      //}

      if (m_triggerOnStart) DoSpawn();
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
      if (m_trapSpawners.Count == 0) return;

      foreach (var trapSpawner in m_trapSpawners.Select(spawner => spawner?.GetComponent<TrapSpawner>()))
      {
        if (trapSpawner != null)
        {
          trapSpawner.LogEvent -= OnLogEvent;
        }
      }
    }

    [UsedImplicitly]
    public void OnTriggered(Collider other)
    {
      if (IsTriggered) return;
      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Trap Triggered **");
      LogTrace($"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} ** Triggered By: {other.name} **");
      DoSpawn();
    }

    #endregion

    private void DisableMarkerMesh()
    {
      if (!m_hideMarker) return;
      var mesh = GetComponentInChildren<MeshRenderer>();
      if (mesh == null) return;
      mesh.enabled = false;
    }

    private void DoSpawn()
    {
      if (IsTriggered) return;
      m_nview.InvokeRPC(FormatNameOfRPC(nameof(RPC_SetIsTriggered)), true);
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

    #region RPC

    private string FormatNameOfRPC(string rpcName)
    {
      return $"{rpcName}.{m_uniqueName}";
    }

    private void RPC_SetIsTriggered(long sender, bool value)
    {
      if (m_nview.IsOwner())
      {
        IsTriggered = value;
      }
    }

    private void RPC_RequestOwn(long sender)
    {
      if (m_nview.IsOwner())
      {
        m_nview.GetZDO().SetOwner(sender);
      }
    }

    #endregion

  }
}
