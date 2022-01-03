using System.Collections.Generic;
using System.Text;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Digitalroot.Valheim.TrapSpawners.Extensions
{
  public static class GameObjectExtensions
  {
    /// <summary>
    /// Set the level of the prefab.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="levelMin"></param>
    /// <param name="levelMax"></param>
    /// <returns></returns>
    internal static GameObject SetLevel(this GameObject prefab, int levelMin, int levelMax)
    {
      var level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
      prefab.SendMessage("SetLevel", level, SendMessageOptions.RequireReceiver);
      return prefab;
    }

    /// <summary>
    /// Configure the base AI
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    internal static GameObject ConfigureBaseAI(this GameObject prefab)
    {
      var monsterAI = prefab.GetComponent<MonsterAI>();
      if (monsterAI == null) return prefab;

      monsterAI.SetDespawnInDay(false);
      monsterAI.SetPatrolPoint(prefab.transform.parent.transform.position);

      if (!monsterAI.m_randomFly)
      {
        monsterAI.m_jumpInterval = Random.Range(5f, 9f);
      }

      monsterAI.m_fleeIfLowHealth = 0f;

      return prefab;
    }

    internal static GameObject AddLedgeJumping(this GameObject prefab)
    {
      prefab.AddComponent<AutoJumpLedge>();
      return prefab;
    }

    /// <summary>
    /// Configure AI for boss.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    internal static GameObject ConfigureBossAI(this GameObject prefab)
    {
      var monsterAI = prefab.GetComponent<MonsterAI>();
      if (monsterAI == null) return prefab;

      if (monsterAI.m_randomFly) return prefab;

      monsterAI.m_pathAgentType = Pathfinding.AgentType.TrollSize;
      monsterAI.m_jumpInterval = Random.Range(8f, 13f);

      return prefab;
    }

    /// <summary>
    /// Set prefab's local position.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    internal static GameObject SetLocalPosition(this GameObject prefab, int i)
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
    internal static GameObject SetLocalScale(this GameObject prefab, float scaleSize)
    {
      prefab.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
      return prefab;
    }

    /// <summary>
    /// Set prefab's local scale
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    internal static GameObject SetLocalRotation(this GameObject prefab, Quaternion quaternion)
    {
      prefab.transform.rotation = quaternion;
      return prefab;
    }

    internal static GameObject ScaleEquipment(this GameObject prefab)
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

    internal static string GetUniqueName(this GameObject prefab)
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
  }
}
