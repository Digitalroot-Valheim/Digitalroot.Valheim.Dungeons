using Digitalroot.Valheim.TrapSpawners.Decorators;
using Digitalroot.Valheim.TrapSpawners.Extensions;
using JetBrains.Annotations;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  /// <inheritdoc />
  [UsedImplicitly]
  public class DungeonMiniBoss : AbstractDungeonCreature
  {
    private bool IsDungeonMiniBoss
    {
      get => m_zdo?.GetBool(Common.Utils.IsDungeonMiniBossKey) ?? false;
      set => m_zdo?.Set(Common.Utils.IsDungeonMiniBossKey, value);
    }

    #region Overrides of AbstractDungeonCreature

    /// <inheritdoc />
    protected override void Awake()
    {
      base.Awake();
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
    }

    /// <inheritdoc />
    protected override void Start()
    {
      base.Start();
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonMiniBoss : {IsDungeonMiniBoss}");
      if (m_zNetView.IsValid() && m_zNetView.IsOwner() && !IsDungeonMiniBoss)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Init MiniBoss-DungeonCreatureData");
        IsDungeonMiniBoss = true;
        m_humanoid.m_boss = true;
        if (m_characterDrop != null)
        {
          foreach (var drop in m_characterDrop.m_drops.Where(d => d.m_levelMultiplier))
          {
            drop.m_levelMultiplier = false;
            drop.m_amountMax = m_humanoid.GetLevel() * 2;
          }
        }

        Save();
      }
    }

    /// <inheritdoc />
    protected override void OnEnable()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonMiniBoss : {IsDungeonMiniBoss} 1");
      base.OnEnable();
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonMiniBoss : {IsDungeonMiniBoss} 2");
      if (m_zNetView.IsValid() && m_zNetView.IsOwner() && IsDungeonMiniBoss)
      {
        gameObject.ConfigureMiniBossAI();
      }
    }

    /// <inheritdoc />
    protected override string GetDungeonCreatureName()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");

      if (m_humanoid == null || m_humanoid.m_name.Contains(" the "))
      {
        return m_humanoid?.m_name;
      }

      return $"{DecoratorUtils.GenerateName(Random.Range(4, 9))} the {m_humanoid?.m_name}";
    }

    #endregion

  }
}
