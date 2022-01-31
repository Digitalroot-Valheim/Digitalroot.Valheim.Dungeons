using Digitalroot.Valheim.TrapSpawners.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Common
{
  public static class Utils
  {
    public const string Namespace = "Digitalroot.Valheim.TrapSpawners";
    public const string EnemyNamePrefix = "Dark";
    public static string IsDungeonCreatureKey = $"{Namespace}_{nameof(IsDungeonCreatureKey)}";
    public static string IsDungeonMonsterKey = $"{Namespace}_{nameof(IsDungeonMonsterKey)}";
    public static string IsDungeonMiniBossKey = $"{Namespace}_{nameof(IsDungeonMiniBossKey)}";
    public static string DungeonCreatureScaleKey = $"{Namespace}_{nameof(DungeonCreatureScaleKey)}";
    public static string DungeonCreatureDataKey = $"{Namespace}_{nameof(DungeonCreatureDataKey)}";

    public static IEnumerable<Collider> FindNearByDungeonCreaturesByOverlapSphereNonAlloc(Vector3 pos, float radius)
    {
      Collider[] hitColliders = new Collider[10];
      int numColliders = Physics.OverlapSphereNonAlloc(pos, radius, hitColliders, LayerMask.GetMask("character", "character_net", "character_ghost"), QueryTriggerInteraction.Ignore);

      foreach (var hitCollider in hitColliders)
      {
        if (hitCollider == null) continue;
        if (hitCollider.gameObject == null) continue;

        if (hitCollider.gameObject.IsDungeonCreature())
        {
          yield return hitCollider;
        }
      }
    }

    private static int _lastCountOverlapBox;

    public static IEnumerable<Collider> FindNearByDungeonCreaturesByOverlapBoxNonAlloc(Vector3 pos, Vector3 halfExtents)
    {
      _lastCountOverlapBox += 10; // 10 more then the last count.

      Collider[] hitColliders = new Collider[_lastCountOverlapBox];

      _lastCountOverlapBox = Physics.OverlapBoxNonAlloc(pos, halfExtents, hitColliders, Quaternion.identity, LayerMask.GetMask("character", "character_net", "character_ghost"), QueryTriggerInteraction.Ignore);

      foreach (var hitCollider in hitColliders)
      {
        if (hitCollider == null) continue;
        if (hitCollider.gameObject == null) continue;

        if (hitCollider.gameObject.IsDungeonCreature())
        {
          yield return hitCollider;
        }
      }
    }

    public static Vector3 GetSize(GameObject gameObject)
    {
      var size = new Vector3();

      while (gameObject.transform.GetParent() != null)
      {
        
      }

      do
      {
        

      } while (gameObject.transform.GetParent());

      return size;
    }
  }
}
