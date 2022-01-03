using Digitalroot.Valheim.TrapSpawners.Extensions;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Decorators
{
  public static class EnemyDecorator
  {
    public static GameObject AsEnemy(this GameObject prefab, float scaleSize, int levelMin, int levelMax)
    {
      var humanoid = prefab.GetComponent<Humanoid>();
      if (humanoid != null)
      {
        humanoid.m_health *= 2;
        humanoid.m_name = $"Dark {humanoid.m_name}";
        humanoid.m_faction = Character.Faction.Undead;
        humanoid.m_jumpForce = Random.Range(6f, 11f);
        humanoid.m_jumpForceForward = Random.Range(6f, 11f);
      }

      prefab.SetLevel(levelMin, levelMax)
            .ConfigureBaseAI()
            .AddLedgeJumping();

      return prefab;
    }
  }
}
