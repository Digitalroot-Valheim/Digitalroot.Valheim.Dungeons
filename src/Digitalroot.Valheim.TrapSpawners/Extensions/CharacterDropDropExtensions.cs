using Digitalroot.Valheim.TrapSpawners.Models;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Extensions
{
  public static class CharacterDropDropExtensions
  {
    public static DungeonCreatureData.Drop ToDungeonCreatureDataDrop(this CharacterDrop.Drop drop)
    {
      Debug.Log($"*** {nameof(ToDungeonCreatureDataDrop)} ***");
      Debug.Log($"{nameof(drop.m_amountMax)} : {drop.m_amountMax}");
      Debug.Log($"{nameof(drop.m_amountMin)} : {drop.m_amountMin}");
      Debug.Log($"{nameof(drop.m_chance)} : {drop.m_chance}");
      Debug.Log($"{nameof(drop.m_levelMultiplier)} : {drop.m_levelMultiplier}");
      Debug.Log($"{nameof(drop.m_onePerPlayer)} : {drop.m_onePerPlayer}");
      Debug.Log($"{nameof(drop.m_prefab)} : {drop.m_prefab}");
      Debug.Log($"{nameof(drop.m_prefab.name)} : {drop.m_prefab.name}");
      Debug.Log($"drop.m_prefab.name.GetStableHashCode() : {drop.m_prefab.name.GetStableHashCode()}");

      return new DungeonCreatureData.Drop
      {
        m_amountMax = drop.m_amountMax,
        m_amountMin = drop.m_amountMin,
        m_chance = drop.m_chance,
        m_levelMultiplier = drop.m_levelMultiplier,
        m_onePerPlayer = drop.m_onePerPlayer,
        m_prefab_hash = drop.m_prefab.name.GetStableHashCode(),
        m_prefab = null
      };
    }
  }
}
