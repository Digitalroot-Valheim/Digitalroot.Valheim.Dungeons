using Digitalroot.Unity3d.Log;
using Digitalroot.Valheim.Common.Json;
using Digitalroot.Valheim.TrapSpawners.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming

namespace Digitalroot.Valheim.TrapSpawners.Extensions
{
  public static class GameObjectExtensions
  {
    #region Logging

    private static TraceLogger _logger = new(Common.Utils.Namespace, true);

    private static void LogError(Exception e)
    {
      LogTrace(e.Message);
      LogTrace(e.StackTrace);
    }

    private static void LogTrace(string msg) => _logger?.Trace(msg);

    #endregion

    public static GameObject AddLedgeJumping(this GameObject prefab)
    {
      prefab.GetOrAddMonoBehaviour<AutoJumpLedge>();
      return prefab;
    }

    public static bool HasParent(this GameObject prefab) => prefab.transform.GetParent() != null;

    public static GameObject GetParent(this GameObject prefab) => prefab.HasParent() ? prefab.transform.GetParent().gameObject : null;

    //public static GameObject AddToSpawnedGameObjectDataCollection(this GameObject prefab, List<DungeonCreatureData> spawnedGameObjectList)
    //{
    //  if (spawnedGameObjectList == null) return prefab;

    //  var spawnedGameObjectData = new DungeonCreatureData();

    //  var zNetView = prefab.GetComponent<ZNetView>();
    //  if (zNetView != null)
    //  {
    //    spawnedGameObjectData.m_zdo_uid = zNetView.GetZDO().m_uid.ToString();
    //  }

    //  spawnedGameObjectData.m_prefab_hash = prefab.name.GetStableHashCode();

    //  var humanoid = prefab.GetComponent<Humanoid>();
    //  if (humanoid != null)
    //  {
    //    spawnedGameObjectData.m_humanoid_name = humanoid.m_name;
    //    spawnedGameObjectData.m_humanoid_boss = humanoid.m_boss;
    //    spawnedGameObjectData.m_humanoid_faction = humanoid.m_faction;
    //    spawnedGameObjectData.m_humanoid_health = humanoid.GetHealth();
    //    spawnedGameObjectData.m_humanoid_max_health = humanoid.GetMaxHealth();
    //    spawnedGameObjectData.m_humanoid_jumpForce = humanoid.m_jumpForce;
    //    spawnedGameObjectData.m_humanoid_jumpForceForward = humanoid.m_jumpForceForward;
    //    spawnedGameObjectData.m_humanoid_level = humanoid.m_level;
    //  }

    //  var characterDrop = prefab.GetComponent<CharacterDrop>();
    //  if (characterDrop != null)
    //  {
    //    spawnedGameObjectData.m_characterDrop_dropList.Clear();
    //    foreach (var drop in characterDrop.m_drops)
    //    {
    //      spawnedGameObjectData.m_characterDrop_dropList.Add(drop.ToDungeonCreatureDataDrop());
    //    }
    //  }

    //  var monsterAI = prefab.GetComponent<MonsterAI>();
    //  if (monsterAI != null)
    //  {
    //    spawnedGameObjectData.m_monsterAI_despawnInDay = monsterAI.DespawnInDay();
    //    spawnedGameObjectData.m_monsterAI_fleeIfLowHealth = monsterAI.m_fleeIfLowHealth;
    //    spawnedGameObjectData.m_monsterAI_jumpInterval = monsterAI.m_jumpInterval;
    //    spawnedGameObjectData.m_monsterAI_pathAgentType = monsterAI.m_pathAgentType;
    //    monsterAI.GetPatrolPoint(out spawnedGameObjectData.m_monsterAI_patrolPoint);
    //  }

    //  spawnedGameObjectData.m_scaleSize = prefab.transform.localScale;
    //  spawnedGameObjectList.Add(spawnedGameObjectData);

    //  // Debug.Log(JsonSerializationProvider.ToJson(spawnedGameObjectData, true));

    //  return prefab;
    //}

    public static bool IsBoss(this GameObject prefab) => prefab.GetComponent<Character>()?.IsBoss() ?? false;

    public static bool IsDungeonCreature(this GameObject prefab) => prefab.GetComponent<ZNetView>()?.GetZDO()?.GetBool(Common.Utils.IsDungeonCreatureKey, false) ?? false;

    private static Vector3 GetScale(string itemName, Vector3 currentScale)
    {
      List<string> names = new()
      {
        "shield"
        , "axe"
        , "mace"
      };

      if (names.Contains(itemName.ToLowerInvariant()))
      {
        return Vector3.one * 2;
      }

      return Vector3.one;
    }

    public static string GetUniqueName(this GameObject prefab)
    {
      List<string> paths = new();

      var parent = prefab.transform.GetParent();

      while (parent != null)
      {
        paths.Add(parent.name);
        parent = parent.GetParent();
      }

      var sb = new StringBuilder();
      for (var i = paths.Count; i > 0; i--)
      {
        sb.Append(paths[i - 1]).Append('.');
      }

      sb.Append(prefab.name);
      return sb.ToString();
    }

    /// <summary>
    /// Configure the base AI
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject ConfigureBaseAI(this GameObject prefab)
    {
      var monsterAI = prefab.GetComponent<MonsterAI>();
      if (monsterAI == null) return prefab;

      monsterAI.SetDespawnInDay(false);
      if (prefab.HasParent())
      {
        monsterAI.SetPatrolPoint(prefab.transform.parent.transform.position);
      }
      else
      {
        monsterAI.SetPatrolPoint();
      }

      if (!monsterAI.m_randomFly)
      {
        monsterAI.m_jumpInterval = Random.Range(5f, 9f);
      }

      monsterAI.m_fleeIfLowHealth = 0f;

      return prefab;
    }

    /// <summary>
    /// Configure AI for boss.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject ConfigureMiniBossAI(this GameObject prefab)
    {
      var monsterAI = prefab.GetComponent<MonsterAI>();
      if (monsterAI == null) return prefab;

      if (monsterAI.m_randomFly) return prefab;

      monsterAI.m_pathAgentType = Pathfinding.AgentType.TrollSize;
      monsterAI.m_jumpInterval = Random.Range(8f, 13f);

      return prefab;
    }

    /// <summary>
    /// Set the level of the prefab.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="levelMin"></param>
    /// <param name="levelMax"></param>
    /// <returns></returns>
    public static GameObject SetLevel(this GameObject prefab, int levelMin, int levelMax)
    {
      var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
      var character = prefab.GetComponent<Character>();
      character?.SetLevel(level);
      character?.SetupMaxHealth();

      // prefab.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);
      return prefab;
    }

    /// <summary>
    /// Set prefab's local position.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    public static GameObject SetLocalPosition(this GameObject prefab, int i)
    {
      switch (i)
      {
        case 1:
          prefab.transform.localPosition += Vector3.left * 2.5f;
          break;

        case 2:
          prefab.transform.localPosition += Vector3.right * 2.5f;
          break;

        case 3:
          prefab.transform.localPosition += Vector3.forward * 2.5f;
          break;

        case 4:
          prefab.transform.localPosition += Vector3.back * 2.5f;
          break;
      }

      return prefab;
    }

    /// <summary>
    /// Set prefab's local scale
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="scaleSize"></param>
    /// <returns></returns>
    [UsedImplicitly]
    public static GameObject SetLocalScale(this GameObject prefab, float scaleSize)
    {
      var zNetView = prefab.GetComponent<ZNetView>();
      if (zNetView != null)
      {
        zNetView.m_syncInitialScale = true;
        zNetView.GetZDO()?.Set(Common.Utils.DungeonCreatureScaleKey, scaleSize); 
      }

      return prefab;
    }

    /// <summary>
    /// Set prefab's local scale
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="scaleSize"></param>
    /// <returns></returns>
    [UsedImplicitly]
    [Obsolete("Do not use", true)]
    public static GameObject SetLocalScale(this GameObject prefab, Vector3 scaleSize)
    {
      var zNetView = prefab.GetComponent<ZNetView>();
      if (zNetView != null)
      {
        zNetView.m_syncInitialScale = true;
        zNetView.GetZDO()?.Set(Common.Utils.DungeonCreatureScaleKey, scaleSize); 
      }

      return prefab;
    }

    /// <summary>
    /// Set prefab's local scale
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    public static GameObject SetLocalRotation(this GameObject prefab, Quaternion quaternion)
    {
      prefab.transform.rotation = quaternion;
      return prefab;
    }

    /// <summary>
    /// Set prefab's max health
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="healthMultiplier"></param>
    /// <param name="healthExponent"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static GameObject SetMaxHealth(this GameObject prefab, float healthMultiplier = 1f, int healthExponent = 1)
    {
      if (healthMultiplier < 1f) healthMultiplier = 1f;
      if (healthExponent < 1) healthExponent = 1;

      var zNetView = prefab.GetComponent<ZNetView>();

      if (zNetView.GetZDO() == null) throw new NullReferenceException($"zNetView is null for {prefab.name}");

      var character = prefab.GetComponent<Character>();

      float health = character.m_health * (float)character.GetLevel() * healthMultiplier * Convert.ToSingle(Math.Pow(character.GetLevel(), healthExponent));

      if (zNetView.IsOwner() && Math.Abs(character.GetHealth() - health) > 1 && Math.Abs(character.GetMaxHealth() - health) > 1)
      {
        if (zNetView.GetZDO() != null)
        {
          zNetView.GetZDO().Set("max_health", health);
        }

        character.SetHealth(health);
      }

      return prefab;
    }

    public static bool IsItem(this GameObject prefab) => prefab.GetComponent<ItemDrop>() != null;

    public static bool IsCreature(this GameObject prefab) => prefab.GetComponent<ItemDrop>() == null;

    public static GameObject ScaleEquipment(this GameObject prefab)
    {
      var visEquipment = prefab.GetComponent<VisEquipment>();
      if (visEquipment == null) return prefab;
      if (visEquipment.m_leftItemInstance != null)
      {
        for (var i = 0; i < visEquipment.m_leftItemInstance.transform.childCount; i++)
        {
          var item = visEquipment.m_leftItemInstance.transform.GetChild(i);
          item.localScale = GetScale(item.gameObject.name, item.localScale);
        }
      }

      if (visEquipment.m_rightItemInstance != null)
      {
        for (var i = 0; i < visEquipment.m_rightItemInstance.transform.childCount; i++)
        {
          var item = visEquipment.m_rightItemInstance.transform.GetChild(i);
          item.localScale = GetScale(item.gameObject.name, item.localScale);
        }
      }

      return prefab;
    }

    //public static DungeonCreatureData ToDungeonCreatureData(this GameObject prefab)
    //{
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}]");
    //  var dungeonCreatureData = new DungeonCreatureData();

    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] A");
    //  var zNetView = prefab.GetComponent<ZNetView>();
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] A.1");
    //  if (zNetView != null)
    //  {
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] A.2 {dungeonCreatureData.m_zdo_uid}");
    //    dungeonCreatureData.m_zdo_uid = zNetView.GetZDO()?.m_uid.ToString() ?? dungeonCreatureData.m_zdo_uid;
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] A.3 {dungeonCreatureData.m_zdo_uid}");
    //  }
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] A.4");

    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B");
    //  dungeonCreatureData.m_prefab_hash = prefab.name.GetStableHashCode();
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B.1");
    //  var humanoid = prefab.GetComponent<Humanoid>();
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B.2");
    //  if (humanoid != null)
    //  {
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B.3");
    //    dungeonCreatureData.m_humanoid_name = humanoid.m_name;
    //    dungeonCreatureData.m_humanoid_boss = humanoid.m_boss;
    //    dungeonCreatureData.m_humanoid_faction = humanoid.m_faction;
    //    dungeonCreatureData.m_humanoid_health = humanoid.GetHealth();
    //    dungeonCreatureData.m_humanoid_max_health = humanoid.GetMaxHealth();
    //    dungeonCreatureData.m_humanoid_jumpForce = humanoid.m_jumpForce;
    //    dungeonCreatureData.m_humanoid_jumpForceForward = humanoid.m_jumpForceForward;
    //    dungeonCreatureData.m_humanoid_level = humanoid.m_level;
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B.4");
    //  }
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] B.5");
      
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C");
    //  var characterDrop = prefab.GetComponent<CharacterDrop>();
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C.1");
    //  if (characterDrop != null)
    //  {
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C.2");
    //    dungeonCreatureData.m_characterDrop_dropList.Clear();
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C.3");
    //    foreach (var drop in characterDrop.m_drops)
    //    {
    //      dungeonCreatureData.m_characterDrop_dropList.Add(drop.ToDungeonCreatureDataDrop());
    //    }
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C.4");
    //  }
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] C.5");

    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] D");
    //  var monsterAI = prefab.GetComponent<MonsterAI>();
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] D.1");
    //  if (monsterAI != null)
    //  {
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] D.2");
    //    dungeonCreatureData.m_monsterAI_despawnInDay = monsterAI.m_despawnInDay;
    //    dungeonCreatureData.m_monsterAI_fleeIfLowHealth = monsterAI.m_fleeIfLowHealth;
    //    dungeonCreatureData.m_monsterAI_jumpInterval = monsterAI.m_jumpInterval;
    //    dungeonCreatureData.m_monsterAI_pathAgentType = monsterAI.m_pathAgentType;
    //    dungeonCreatureData.m_monsterAI_patrolPoint = monsterAI.m_patrolPoint;
    //    LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] D.3");
    //  }
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] D.4");

    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] E");
    //  dungeonCreatureData.m_scaleSize = prefab.transform.localScale;
    //  LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] E.1");
    //  // LogTrace($"[{MethodBase.GetCurrentMethod()?.Name}.{prefab.name}] \n{dungeonCreatureData.ToJson(true)}");

    //  return dungeonCreatureData;
    //}

    /// <summary>
    /// Returns the component of Type type. If one doesn't already exist on the GameObject it will be added.
    /// </summary>
    /// <remarks>
    /// Inspired by Jotunn JVL
    /// Source: https://wiki.unity3d.com/index.php/GetOrAddComponent
    /// </remarks>
    /// <typeparam name="T">The type of Component to return.</typeparam>
    /// <param name="gameObject">The GameObject the Component is attached to.</param>
    /// <returns>Returns the component of Type T</returns>
    [UsedImplicitly]
    public static T GetOrAddMonoBehaviour<T>([NotNull] this GameObject gameObject)
      where T : MonoBehaviour
    {
      return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }
  }
}
