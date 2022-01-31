using JetBrains.Annotations;
using System.Reflection;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  /// <inheritdoc />
  [UsedImplicitly]
  public class DungeonMonster : AbstractDungeonCreature
  {
    private bool IsDungeonMonster
    {
      get => m_zdo?.GetBool(Common.Utils.IsDungeonMonsterKey) ?? false;
      set => m_zdo?.Set(Common.Utils.IsDungeonMonsterKey, value);
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
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] IsDungeonMonster : {IsDungeonMonster}");
      if (m_zNetView.IsValid() && m_zNetView.IsOwner() && !IsDungeonMonster)
      {
        LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}] Init DungeonCreatureData");
        IsDungeonMonster = true;
        Save();
      }
    }

    protected override string GetDungeonCreatureName()
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{gameObject.name}]");
      return $"{Common.Utils.EnemyNamePrefix} {m_humanoid.m_name}";
    }

    #endregion
  }
}
