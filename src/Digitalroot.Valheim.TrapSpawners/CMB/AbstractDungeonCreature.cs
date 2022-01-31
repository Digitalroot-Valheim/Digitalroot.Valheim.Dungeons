using Digitalroot.Valheim.Common.Extensions;
using Digitalroot.Valheim.Common.Json;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using Digitalroot.Valheim.TrapSpawners.Models;
using fastJSON;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public abstract class AbstractDungeonCreature : EventLoggingMonoBehaviour
  {
    private IEnumerator _coroutine;
    protected ZNetView m_zNetView;
    protected Humanoid m_humanoid;
    protected CharacterDrop m_characterDrop;
    protected MonsterAI m_monsterAi;
    protected ZDO m_zdo;
    private GameObject m_visual;
    protected CapsuleCollider m_collider;
    protected CapsuleCollider m_attackCollider;
    protected VisEquipment m_visEquipment;
    protected float m_spawnerScale;
    private string m_uniqueName;
    private bool m_isInit;
    private System.Timers.Timer _initTimer = new();
    private float m_lastOwnerRequest;

    // Left
    protected Vector3 m_leftHandLocalScale => m_visEquipment.m_leftHand.localScale;
    protected Transform m_leftHand => m_visEquipment.m_leftHand;
    protected string m_leftItem => m_visEquipment.m_leftItem;
    protected GameObject m_leftItemInstance => m_visEquipment.m_leftItemInstance;

    // Right
    protected Vector3 m_rightHandLocalScale => m_visEquipment.m_rightHand.localScale;
    protected Transform m_rightHand => m_visEquipment.m_rightHand;
    protected string m_rightItem => m_visEquipment.m_rightItem;
    protected GameObject m_rightItemInstance => m_visEquipment.m_rightItemInstance;

    private DungeonCreatureData DungeonCreatureData
    {
      get
      {
        var json = m_zdo?.GetString(Common.Utils.DungeonCreatureDataKey);
        return json.IsNullOrEmpty() ? new DungeonCreatureData() : JsonSerializationProvider.FromJson<DungeonCreatureData>(json);
      }

      set => m_zdo?.Set(Common.Utils.DungeonCreatureDataKey, value.ToJson());
    }

    private bool IsDungeonCreature
    {
      get => m_zdo?.GetBool(Common.Utils.IsDungeonCreatureKey) ?? false;
      set => m_zdo?.Set(Common.Utils.IsDungeonCreatureKey, value);
    }

    static AbstractDungeonCreature()
    {
      JSON.RegisterCustomType(typeof(CharacterDrop.Drop),
                              x =>
                              {
                                var drop = (CharacterDrop.Drop)x;
                                return JSON.ToJSON(drop.ToDungeonCreatureDataDrop());
                              },
                              x =>
                              {
                                var drop = JSON.ToObject<DungeonCreatureData.Drop>(x);
                                return drop.ToCharacterDropDrop();
                              });
    }

    [UsedImplicitly]
    protected virtual void Awake()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      m_zNetView = gameObject.GetComponent<ZNetView>();
      m_humanoid = gameObject.GetComponent<Humanoid>();
      m_characterDrop = gameObject.GetComponent<CharacterDrop>();
      m_monsterAi = gameObject.GetComponent<MonsterAI>();
      m_zdo = m_zNetView.GetZDO();
      m_visual = gameObject.transform.Find("Visual").gameObject;
      m_collider = gameObject.GetComponent<CapsuleCollider>();
      m_attackCollider = gameObject.transform.Find("Attack collider")?.GetComponent<CapsuleCollider>();
      m_visEquipment = gameObject.GetComponent<VisEquipment>();
      m_uniqueName = gameObject.GetUniqueName();

      // m_leftHandLocalScale = m_visEquipment.m_leftHand.localScale;
      // m_leftItem = m_visEquipment.m_leftItem;
      // m_leftItemInstance = m_visEquipment.m_leftItemInstance;
      // m_rightHandLocalScale = m_visEquipment.m_rightHand.localScale;
      // m_rightItem = m_visEquipment.m_rightItem;
      // m_rightItemInstance = m_visEquipment.m_rightItemInstance;

      _initTimer.Enabled = false;
      _initTimer.AutoReset = true;
      _initTimer.Interval = 5000d;
      _initTimer.Elapsed += InitTimerElapsed;

      m_zNetView.Register(FormatNameOfRPC("RequestOwn"), RPC_RequestOwn);
    }

    [UsedImplicitly]
    protected virtual void Start()
    {
      LogTrace($"**************************************************");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonCreature : {IsDungeonCreature}");
      LogTrace($"**************************************************");
      if (m_zdo != null && m_zdo.IsOwner() && !IsDungeonCreature)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Init DungeonCreatureData");
        IsDungeonCreature = true;
        m_humanoid.m_name = GetDungeonCreatureName();
        m_spawnerScale = m_zdo.GetFloat(Common.Utils.DungeonCreatureScaleKey, 1f);
        Save();
        gameObject.AddLedgeJumping();
        gameObject.ConfigureBaseAI();

        m_isInit = true;

        _coroutine = ScaleEquipmentCoroutine();
        StartCoroutine(_coroutine);
      }
      _initTimer.Start();
    }

    [UsedImplicitly]
    protected virtual void OnEnable()
    {
      LogTrace($"+++++++++++++++++++++++++++++++++++++++++++++++++");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonCreature : {IsDungeonCreature}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo == null : {m_zdo == null}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo?.IsOwner() : {m_zdo?.IsOwner()}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo?.HasOwner() : {m_zdo?.HasOwner()}");
      LogTrace($"+++++++++++++++++++++++++++++++++++++++++++++++++");

      if (m_zdo != null && m_zdo.IsOwner() && IsDungeonCreature)
      {
        Hydrate();
        gameObject.AddLedgeJumping();
        gameObject.ConfigureBaseAI();
        m_isInit = true;
      }
    }

    [UsedImplicitly]
    protected virtual void OnDisable()
    {
      LogTrace($"-------------------------------------------------");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonCreature : {IsDungeonCreature}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo == null : {m_zdo == null}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo?.IsOwner() : {m_zdo?.IsOwner()}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo?.HasOwner() : {m_zdo?.HasOwner()}");
      LogTrace($"-------------------------------------------------");

      if (m_zdo != null && m_zdo.IsOwner())
      {
        Dehydrate();
      }
    }

    protected virtual void Hydrate()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");

      LogTrace($"/------------------- JSON Read -----------------\\");
      LogTrace(DungeonCreatureData.ToJson(true));
      LogTrace($"\\-----------------------------------------------/");

      if (m_zdo.m_uid.ToString() != DungeonCreatureData.m_zdo_uid)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_zdo_uid: {m_zdo.m_uid} != {DungeonCreatureData.m_zdo_uid}");
      }

      if (gameObject.name.GetStableHashCode() != DungeonCreatureData.m_prefab_hash)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_prefab_hash: {gameObject.name.GetStableHashCode()} != {DungeonCreatureData.m_prefab_hash}");
      }

      if (m_humanoid != null)
      {
        m_humanoid.m_name = DungeonCreatureData.m_humanoid_name;
        m_humanoid.m_boss = DungeonCreatureData.m_humanoid_boss;
        m_humanoid.m_faction = DungeonCreatureData.m_humanoid_faction;
        m_humanoid.SetHealth(DungeonCreatureData.m_humanoid_health);
        m_humanoid.SetMaxHealth(DungeonCreatureData.m_humanoid_max_health);
        m_humanoid.m_jumpForce = DungeonCreatureData.m_humanoid_jumpForce;
        m_humanoid.m_jumpForceForward = DungeonCreatureData.m_humanoid_jumpForceForward;
      }

      if (m_characterDrop != null)
      {
        m_characterDrop.m_drops.Clear();
        foreach (var drop in DungeonCreatureData.m_characterDrop_dropList)
        {
          m_characterDrop.m_drops.Add(drop.ToCharacterDropDrop());
        }
      }

      if (m_monsterAi != null)
      {
        m_monsterAi.SetDespawnInDay(DungeonCreatureData.m_monsterAI_despawnInDay);
        m_monsterAi.m_fleeIfLowHealth = DungeonCreatureData.m_monsterAI_fleeIfLowHealth;
        m_monsterAi.m_jumpInterval = DungeonCreatureData.m_monsterAI_jumpInterval;
        m_monsterAi.m_pathAgentType = DungeonCreatureData.m_monsterAI_pathAgentType;
        m_monsterAi.SetPatrolPoint(DungeonCreatureData.m_monsterAI_patrolPoint);
      }

      m_spawnerScale = DungeonCreatureData.m_spawnerScale;

      // m_zNetView.m_syncInitialScale = true;
      // m_visual.transform.localScale = new Vector3(DungeonCreatureData.m_scaleSize.x, DungeonCreatureData.m_scaleSize.y, DungeonCreatureData.m_scaleSize.z);
      // m_visEquipment.m_leftHand.localScale = new Vector3(DungeonCreatureData.m_scaleSize.x, DungeonCreatureData.m_scaleSize.y, DungeonCreatureData.m_scaleSize.z);
      // m_visEquipment.m_rightHand.localScale = new Vector3(DungeonCreatureData.m_scaleSize.x, DungeonCreatureData.m_scaleSize.y, DungeonCreatureData.m_scaleSize.z);
      // if (m_attack_collider != null)
      // {
      //   m_attack_collider.transform.localScale = new Vector3(DungeonCreatureData.m_scaleSize.x, DungeonCreatureData.m_scaleSize.y, DungeonCreatureData.m_scaleSize.z);
      // }

      _coroutine = ScaleEquipmentCoroutine();
      StartCoroutine(_coroutine);
    }

    [Conditional("DEBUG")]
    private void Dump()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zNetView?.GetZDO() == null : {m_zNetView?.GetZDO() == null}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zdo == null : {m_zdo == null}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Level: m_humanoid.m_level : m_humanoid.GetLevel(), DungeonCreatureData.m_humanoid_level");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Level: {m_humanoid?.m_level}, {m_humanoid?.GetLevel()}, {DungeonCreatureData?.m_humanoid_level}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Health: m_humanoid.m_health, m_humanoid.GetHealth(), DungeonCreatureData.m_humanoid_health, m_zdo?.GetFloat(\"health\")");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Health: {m_humanoid?.m_health}, {m_humanoid?.GetHealth()}, {DungeonCreatureData?.m_humanoid_max_health}, {m_zdo?.GetFloat("health")}");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Max Health: m_humanoid.GetMaxHealth(), DungeonCreatureData.m_humanoid_max_health, m_zdo?.GetFloat(\"max_health\")");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Max Health: {m_humanoid?.GetMaxHealth()}, {DungeonCreatureData?.m_humanoid_max_health}, {m_zdo?.GetFloat("max_health")}");
    }

    protected virtual void Dehydrate()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");

      if (m_zdo?.m_uid.ToString() != DungeonCreatureData.m_zdo_uid)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_zdo_uid: {m_zNetView.GetZDO().m_uid} != {DungeonCreatureData.m_zdo_uid}");
      }

      if (gameObject.name.GetStableHashCode() != DungeonCreatureData.m_prefab_hash)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_prefab_hash: {gameObject.name.GetStableHashCode()} != {DungeonCreatureData.m_prefab_hash}");
      }

      Save();
      LogTrace($"/------------------- JSON Save -----------------\\");
      LogTrace(DungeonCreatureData.ToJson(true));
      LogTrace($"\\-----------------------------------------------/");
    }

    private string FormatNameOfRPC(string rpcName)
    {
      return $"{rpcName}.{m_uniqueName}";
    }

    protected abstract string GetDungeonCreatureName();

    private void InitTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_isInit : {m_isInit}");
      if (m_isInit)
      {
        _initTimer.Stop();
        _initTimer.Enabled = false;
        _initTimer.Elapsed -= InitTimerElapsed;
        _initTimer.Dispose();
        _initTimer = null;
        return;
      }

      if (m_zdo.IsOwner())
      {
        OnEnable();
        return;
      }

      if (m_zdo.HasOwner())
      {
        RequestOwn();
      }
    }

    protected void RequestOwn()
    {
      if (!(Time.time - m_lastOwnerRequest < 0.2f) && !m_zNetView.IsOwner())
      {
        m_lastOwnerRequest = Time.time;
        m_zNetView.InvokeRPC("RequestOwn");
      }
    }

    private void RPC_RequestOwn(long sender)
    {
      if (m_zNetView.IsOwner())
      {
        m_zNetView.GetZDO().SetOwner(sender);
      }
    }

    protected virtual void Save()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");

      var dungeonCreatureData = DungeonCreatureData;

      dungeonCreatureData.m_prefab_hash = gameObject.name.GetStableHashCode();
      dungeonCreatureData.m_spawnerScale = m_spawnerScale;

      if (m_zNetView != null)
      {
        dungeonCreatureData.m_zdo_uid = m_zNetView.GetZDO()?.m_uid.ToString() ?? dungeonCreatureData.m_zdo_uid;
      }

      if (m_humanoid != null)
      {
        dungeonCreatureData.m_humanoid_name = m_humanoid.m_name;
        dungeonCreatureData.m_humanoid_boss = m_humanoid.m_boss;
        dungeonCreatureData.m_humanoid_faction = m_humanoid.m_faction;
        dungeonCreatureData.m_humanoid_health = m_zdo.GetFloat("health");
        dungeonCreatureData.m_humanoid_max_health = m_zdo.GetFloat("max_health");
        dungeonCreatureData.m_humanoid_jumpForce = m_humanoid.m_jumpForce;
        dungeonCreatureData.m_humanoid_jumpForceForward = m_humanoid.m_jumpForceForward;
        dungeonCreatureData.m_humanoid_level = m_humanoid.m_level;
      }

      if (m_characterDrop != null)
      {
        dungeonCreatureData.m_characterDrop_dropList.Clear();
        foreach (var drop in m_characterDrop.m_drops)
        {
          dungeonCreatureData.m_characterDrop_dropList.Add(drop.ToDungeonCreatureDataDrop());
        }
      }

      if (m_monsterAi != null)
      {
        dungeonCreatureData.m_monsterAI_despawnInDay = m_monsterAi.m_despawnInDay;
        dungeonCreatureData.m_monsterAI_fleeIfLowHealth = m_monsterAi.m_fleeIfLowHealth;
        dungeonCreatureData.m_monsterAI_jumpInterval = m_monsterAi.m_jumpInterval;
        dungeonCreatureData.m_monsterAI_pathAgentType = m_monsterAi.m_pathAgentType;
        dungeonCreatureData.m_monsterAI_patrolPoint = m_monsterAi.m_patrolPoint;
      }

      DungeonCreatureData = dungeonCreatureData;
    }

    private void ScaleAttackCollider()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      if (m_attackCollider != null)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_attack_collider.transform.localScale : {m_attackCollider?.transform?.localScale}");
        if (m_attackCollider.transform != null) m_attackCollider.transform.localScale = new Vector3(m_spawnerScale, m_spawnerScale, m_spawnerScale);
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_attack_collider.transform.localScale : {m_attackCollider?.transform?.localScale}");
      }
    }

    private bool ScaleEquipment()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_spawnerScale : {m_spawnerScale}");

      if (m_zdo != null && m_zdo.IsOwner())
      {
        ScaleZNetView();
      }

      return true;
    }

    private IEnumerator ScaleEquipmentCoroutine()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      yield return new WaitForSeconds(10);
      yield return ScaleEquipment();
    }

    private void ScaleVisEquipment()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      if (m_visEquipment != null)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_visEquipment.m_leftHand.localScale : {m_visEquipment?.m_leftHand?.localScale}");
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_visEquipment.m_rightHand.localScale : {m_visEquipment?.m_rightHand?.localScale}");
        if (m_visEquipment.m_leftHand != null) m_visEquipment.m_leftHand.localScale = new Vector3(m_spawnerScale, m_spawnerScale, m_spawnerScale);
        if (m_visEquipment.m_rightHand != null) m_visEquipment.m_rightHand.localScale = new Vector3(m_spawnerScale, m_spawnerScale, m_spawnerScale);
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_visEquipment.m_leftHand.localScale : {m_visEquipment?.m_leftHand?.localScale}");
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_visEquipment.m_rightHand.localScale : {m_visEquipment?.m_rightHand?.localScale}");
      }
    }

    private void ScaleVisual()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      var visual = gameObject.transform.Find("Visual");
      if (visual != null)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] visual.localScale : {visual.localScale}");
        visual.localScale = new Vector3(m_spawnerScale, m_spawnerScale, m_spawnerScale);
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] visual.localScale : {visual.localScale}");
      }
    }

    private void ScaleZNetView()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zNetView.gameObject.transform.localScale : {m_zNetView.gameObject.transform.localScale}");
      m_zNetView.m_syncInitialScale = true;
      m_zNetView.SetLocalScale(new Vector3(m_spawnerScale, m_spawnerScale, m_spawnerScale));
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] m_zNetView.gameObject.transform.localScale : {m_zNetView.gameObject.transform.localScale}");
    }
  }
}
