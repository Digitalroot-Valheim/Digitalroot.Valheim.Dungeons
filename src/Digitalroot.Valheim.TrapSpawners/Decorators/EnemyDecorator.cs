using Digitalroot.Valheim.TrapSpawners.Extensions;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Digitalroot.Valheim.TrapSpawners.Decorators
{
  public static class EnemyDecorator
  {
    public static GameObject AsEnemy(this GameObject prefab, float scaleSize, int levelMin, int levelMax)
    {
      try
      {
        var humanoid = prefab.GetComponent<Humanoid>();
        if (humanoid != null)
        {
          humanoid.m_level = levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1);
          humanoid.m_health *= humanoid.m_level *= 2;
          // humanoid.SetMaxHealth(humanoid.m_health * humanoid.m_level * 2);
          // humanoid.SetMaxHealth(humanoid.GetMaxHealth() * 2);
          // humanoid.SetHealth(humanoid.GetMaxHealth());
          humanoid.m_name = $"{Common.Utils.EnemyNamePrefix} {humanoid.m_name}";
          humanoid.m_faction = Character.Faction.Undead;
          humanoid.m_jumpForce = Random.Range(6f, 11f);
          humanoid.m_jumpForceForward = Random.Range(6f, 11f);
        }

        prefab
              // .SetLevel(levelMin, levelMax)
              .ConfigureBaseAI()
              .AddLedgeJumping();

        return prefab;
      }
      catch (Exception e)
      {
        e.Data.Add("prefab", prefab.name);
        e.Data.Add("scaleSize", scaleSize);
        e.Data.Add("levelMin", levelMin);
        e.Data.Add("levelMax", levelMax);
        throw;
      }
      
    }
  }
}
