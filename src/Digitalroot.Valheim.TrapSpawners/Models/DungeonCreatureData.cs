using Digitalroot.Valheim.Common.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Digitalroot.Valheim.TrapSpawners.Models
{
  [Serializable]
  public class DungeonCreatureData
  {
    #pragma warning disable CS0649
    public string m_zdo_uid;
    public int m_prefab_hash;
    public string m_humanoid_name;
    public int m_humanoid_level;
    public float m_humanoid_health;
    public float m_humanoid_max_health;
    public Character.Faction m_humanoid_faction;
    public bool m_humanoid_boss;
    public float m_humanoid_jumpForce;
    public float m_humanoid_jumpForceForward;
    public float m_monsterAI_jumpInterval;
    public float m_monsterAI_fleeIfLowHealth;
    public bool m_monsterAI_despawnInDay;
    public Pathfinding.AgentType m_monsterAI_pathAgentType;
    public Vector3 m_monsterAI_patrolPoint;
    public List<Drop> m_characterDrop_dropList = new();
    public Vector3 m_scaleSize;
    #pragma warning restore CS0649

    public string ToJson(bool pretty = false) => JsonSerializationProvider.ToJson(this, pretty);

    [Serializable]
    public class Drop : CharacterDrop.Drop
    {
      public int m_prefab_hash;

      public CharacterDrop.Drop ToCharacterDropDrop()
      {
        m_prefab = ZNetScene.instance.GetPrefab(m_prefab_hash);
        return this;
      }
    }
  }
}
