using Digitalroot.Valheim.Common.Json;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using Digitalroot.Valheim.TrapSpawners.Models;
using fastJSON;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class DungeonCreature : EventLoggingMonoBehaviour
  {
    protected DungeonCreatureData _dungeonCreatureData;
    protected ZNetView m_netView;
    protected IEnumerator _coroutine;

    static DungeonCreature()
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
      m_netView = gameObject.GetComponent<ZNetView>();
      if (!gameObject.IsDungeonCreature())
      {
        m_netView.GetZDO().Set(Common.Utils.IsDungeonCreature, true);
        _dungeonCreatureData = gameObject.ToDungeonCreatureData();
        m_netView.GetZDO().Set(Common.Utils.DungeonCreatureDataKey, _dungeonCreatureData.ToJson());
      }
    }

    [UsedImplicitly]
    protected virtual void Start()
    {
      LogTrace($"**************************************************");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] {gameObject.IsDungeonCreature()}");
      LogTrace($"**************************************************");
      if (gameObject.IsDungeonCreature())
      {
        m_netView.GetZDO().Set(Common.Utils.DungeonCreatureDataKey, gameObject.ToDungeonCreatureData().ToJson());
      }
    }

    [UsedImplicitly]
    protected virtual void OnEnable()
    {
      LogTrace($"+++++++++++++++++++++++++++++++++++++++++++++++++");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] {gameObject.IsDungeonCreature()}");
      LogTrace($"+++++++++++++++++++++++++++++++++++++++++++++++++");

      if (gameObject.IsDungeonCreature())
      {
        var json = m_netView.GetZDO().GetString(Common.Utils.DungeonCreatureDataKey);
        if (!string.IsNullOrEmpty(json))
        {
          _dungeonCreatureData = JsonSerializationProvider.FromJson<DungeonCreatureData>(json);
          Hydrate();
          gameObject.AddLedgeJumping();
          gameObject.ConfigureBaseAI();
        }
      }
    }

    [UsedImplicitly]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UsedImplicitly")]
    protected virtual void OnDisable()
    {
      LogTrace($"-------------------------------------------------");
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] {gameObject.IsDungeonCreature()}");
      LogTrace($"-------------------------------------------------");

      if (gameObject.IsDungeonCreature())
      {
        Dehydrate();
      }
    }

    protected virtual void Dehydrate()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");

      if (m_netView.GetZDO().m_uid.ToString() != _dungeonCreatureData.m_zdo_uid)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_zdo_uid: {m_netView.GetZDO().m_uid} != {_dungeonCreatureData.m_zdo_uid}");
      }

      if (gameObject.name.GetStableHashCode() != _dungeonCreatureData.m_prefab_hash)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_prefab_hash: {gameObject.name.GetStableHashCode()} != {_dungeonCreatureData.m_prefab_hash}");
      }

      // LogTrace($"gameObject.transform.localScale : {gameObject.transform.localScale}");
      // LogTrace($"m_netView.GetZDO() ==  null : {m_netView.GetZDO() == null}");
      var json = gameObject.ToDungeonCreatureData();
      // LogTrace($"x == null : {json == null}");
      LogTrace($"json?.ToJson() : {json?.ToJson(true)}");

      m_netView.GetZDO().Set(Common.Utils.DungeonCreatureDataKey, json?.ToJson());
    }

    protected virtual void Hydrate()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      if (m_netView.GetZDO().m_uid.ToString() != _dungeonCreatureData.m_zdo_uid)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_zdo_uid: {m_netView.GetZDO().m_uid} != {_dungeonCreatureData.m_zdo_uid}");
      }

      if (gameObject.name.GetStableHashCode() != _dungeonCreatureData.m_prefab_hash)
      {
        throw new Exception($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] invalid m_prefab_hash: {gameObject.name.GetStableHashCode()} != {_dungeonCreatureData.m_prefab_hash}");
      }

      var humanoid = gameObject.GetComponent<Humanoid>();
      // var max_h = humanoid?.GetMaxHealth().CompareTo(_dungeonCreatureData.m_humanoid_max_health);
      // LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Max Health: {humanoid?.GetMaxHealth()} == {_dungeonCreatureData.m_humanoid_max_health}, {max_h}");
      // LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Current Health: {humanoid?.GetMaxHealth()} == {_dungeonCreatureData.m_humanoid_max_health}, {max_h}");

      if (humanoid != null)
      {
        humanoid.m_name = GetDungeonCreatureName();
        humanoid.m_boss = GetDungeonCreatureIsBoss();
        humanoid.m_faction = GetDungeonCreatureFaction();
        // humanoid.SetLevel(GetDungeonCreatureLevel());
        // humanoid.SetHealth(GetDungeonCreatureHealth());
        // humanoid.SetMaxHealth(GetDungeonCreatureMaxHealth());
        // humanoid.SetupMaxHealth();
        humanoid.m_jumpForce = GetDungeonCreatureJumpForce();
        humanoid.m_jumpForceForward = GetDungeonCreatureJumpForceForward();
      }

      var characterDrop = gameObject.GetComponent<CharacterDrop>();
      if (characterDrop != null)
      {
        characterDrop.m_drops.Clear();
        foreach (var drop in _dungeonCreatureData.m_characterDrop_dropList)
        {
          characterDrop.m_drops.Add(drop.ToCharacterDropDrop());
        }
      }

      var monsterAI = gameObject.GetComponent<MonsterAI>();
      if (monsterAI != null)
      {
        monsterAI.SetDespawnInDay(GetDungeonCreatureDespawnInDay());
        monsterAI.m_fleeIfLowHealth = GetDungeonCreatureFleeIfLowHealth();
        monsterAI.m_jumpInterval = GetDungeonCreatureJumpInterval();
        monsterAI.m_pathAgentType = GetDungeonCreaturePathAgentType();
        monsterAI.SetPatrolPoint(GetDungeonCreaturePatrolPoint());
      }

      gameObject.SetLocalScale(GetDungeonCreatureScaleSize());
    }

    protected virtual Vector3 GetDungeonCreatureScaleSize()
    {
      return _dungeonCreatureData.m_scaleSize;
    }

    protected virtual Vector3 GetDungeonCreaturePatrolPoint()
    {
      return _dungeonCreatureData.m_monsterAI_patrolPoint;
    }

    protected virtual Pathfinding.AgentType GetDungeonCreaturePathAgentType()
    {
      return _dungeonCreatureData.m_monsterAI_pathAgentType;
    }

    protected virtual float GetDungeonCreatureJumpInterval()
    {
      return _dungeonCreatureData.m_monsterAI_jumpInterval;
    }

    protected virtual float GetDungeonCreatureFleeIfLowHealth()
    {
      return _dungeonCreatureData.m_monsterAI_fleeIfLowHealth;
    }

    protected virtual bool GetDungeonCreatureDespawnInDay()
    {
      return _dungeonCreatureData.m_monsterAI_despawnInDay;
    }

    protected virtual int GetDungeonCreatureLevel()
    {
      return _dungeonCreatureData.m_humanoid_level;
    }

    protected virtual float GetDungeonCreatureJumpForceForward()
    {
      return _dungeonCreatureData.m_humanoid_jumpForceForward;
    }

    protected virtual float GetDungeonCreatureJumpForce()
    {
      return _dungeonCreatureData.m_humanoid_jumpForce;
    }

    protected virtual float GetDungeonCreatureMaxHealth()
    {
      return _dungeonCreatureData.m_humanoid_max_health;
    }

    protected virtual float GetDungeonCreatureHealth()
    {
      return _dungeonCreatureData.m_humanoid_health;
    }

    protected virtual Character.Faction GetDungeonCreatureFaction()
    {
      return _dungeonCreatureData.m_humanoid_faction;
    }

    protected virtual bool GetDungeonCreatureIsBoss()
    {
      return _dungeonCreatureData.m_humanoid_boss;
    }

    protected virtual string GetDungeonCreatureName()
    {
      if (!GetDungeonCreatureIsBoss())
      {
        if (!_dungeonCreatureData.m_humanoid_name.StartsWith(Common.Utils.EnemyNamePrefix))
        {
          return $"{Common.Utils.EnemyNamePrefix} {_dungeonCreatureData.m_humanoid_name}";
        }
      }

      return _dungeonCreatureData.m_humanoid_name;
    }

    protected static IEnumerator ScaleEquipmentCoroutine(GameObject prefab)
    {
      yield return new WaitForSeconds(5);
      prefab.ScaleEquipment();
    }
  }
}
