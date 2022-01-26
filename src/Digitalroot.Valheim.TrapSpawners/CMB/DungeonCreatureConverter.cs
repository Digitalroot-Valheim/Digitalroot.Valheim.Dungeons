using Digitalroot.Valheim.TrapSpawners.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  [AddComponentMenu("Traps/Dungeon Creature Converter", 35), DisallowMultipleComponent]
  public class DungeonCreatureConverter : EventLoggingMonoBehaviour
  {
    private readonly System.Timers.Timer _timer = new(15000);

    [UsedImplicitly]
    public void Awake()
    {
      _timer.Enabled = false;
    }

    [UsedImplicitly]
    public void OnEnable()
    {
      _timer.Start();
    }

    [UsedImplicitly]
    public void OnDisable()
    {
      _timer.Stop();
    }

    [UsedImplicitly]
    public void Start()
    {
      _timer.Elapsed += TimerElapsed;
      _timer.AutoReset = true;
      _timer.Enabled = true;
      _timer.Start();
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
      _timer.Close();
      _timer.Dispose();
    }

    private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      LogTrace($"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}.{name}]");

      foreach (var dungeonCreature in FindNearByDungeonCreatures())
      {
        if (dungeonCreature.gameObject.IsDungeonCreature())
        {
          dungeonCreature.gameObject.GetOrAddMonoBehaviour<DungeonCreature>();
        }
      }
    }

    private IEnumerable<Collider> FindNearByDungeonCreatures() => Common.Utils.FindNearByDungeonCreaturesByOverlapBoxNonAlloc(gameObject.transform.position, new Vector3(gameObject.transform.localScale.x / 2, gameObject.transform.localScale.y / 2, gameObject.transform.localScale.z / 2));
  }
}
