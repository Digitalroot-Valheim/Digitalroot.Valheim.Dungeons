using Digitalroot.Valheim.TrapSpawners.Decorators;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using System.Linq;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  public class DungeonMiniBoss : DungeonCreature
  {
    #region Overrides of DungeonCreature

    /// <inheritdoc />
    protected override void Awake()
    {
      base.Awake();

      var humanoid = gameObject.GetComponent<Humanoid>();
      if (humanoid == null) return;

      humanoid.m_boss = true;
      humanoid.m_name = $"{DecoratorUtils.GenerateName(Random.Range(4, 9))} the {humanoid.m_name}";

      var characterDrop = gameObject.GetComponent<CharacterDrop>();
      if (characterDrop == null) return;

      foreach (var drop in characterDrop.m_drops.Where(d => d.m_levelMultiplier))
      {
        drop.m_levelMultiplier = false;
        drop.m_amountMax = humanoid.GetLevel() * 2;
      }

      // humanoid.SetMaxHealth(humanoid.GetMaxHealth() * Convert.ToSingle(Math.Pow(scaleSize, 2)));
      // humanoid.SetHealth(humanoid.GetMaxHealth());

      m_netView.GetZDO().Set(Common.Utils.DungeonCreatureDataKey, gameObject.ToDungeonCreatureData().ToJson());
    }


    /// <inheritdoc />
    protected override void OnEnable()
    {
      base.OnEnable();

      if (gameObject.IsDungeonCreature())
      {
        gameObject.ConfigureMiniBossAI();


        _coroutine = ScaleEquipmentCoroutine(gameObject);
        StartCoroutine(_coroutine);
      }


    }

    #endregion
  }
}