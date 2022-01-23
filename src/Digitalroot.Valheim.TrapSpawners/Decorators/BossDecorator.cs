using Digitalroot.Valheim.TrapSpawners.Extensions;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Digitalroot.Valheim.TrapSpawners.Decorators
{
  public static class BossDecorator
  {
    public static GameObject AsBoss(this GameObject prefab, float scaleSize, int levelMin, int levelMax)
    {
      var humanoid = prefab.GetComponent<Humanoid>();
      if (humanoid == null) return prefab;

      humanoid.m_boss = true;
      humanoid.SetLevel(levelMin == levelMax ? levelMax : Random.Range(levelMin, levelMax + 1));
      humanoid.SetMaxHealth(humanoid.GetMaxHealth() * Convert.ToSingle(Math.Pow(scaleSize, 2)));
      humanoid.SetHealth(humanoid.GetMaxHealth());
      humanoid.m_name = $"{DecoratorUtils.GenerateName(Random.Range(4, 9))} the {humanoid.m_name}";
      humanoid.m_faction = Character.Faction.Boss;
      humanoid.m_jumpForce = Random.Range(8f, 13f);
      humanoid.m_jumpForceForward = Random.Range(8f, 13f);

      prefab
        // .SetLevel(levelMin, levelMax)
            .ConfigureBaseAI()
            .ConfigureBossAI()
            .AddLedgeJumping();

      var characterDrop = prefab.GetComponent<CharacterDrop>();
      if (characterDrop == null) return prefab;

      foreach (var drop in characterDrop.m_drops.Where(d => d.m_levelMultiplier))
      {
        drop.m_levelMultiplier = false;
        drop.m_amountMax = humanoid.GetLevel() * 2;
      }

      return prefab;
    }
  }
}
