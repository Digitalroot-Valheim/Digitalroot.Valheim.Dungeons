using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public class TrapTriggerProxy : AbstractProxy<TrapTrigger>
  {
    private readonly StaticSourceLogger _logger = new(true);
    private const string RoomTriggerName = "SpawnTrigger";

    public TrapTriggerProxy(TrapTrigger realObject)
      : base(realObject) { }

    public TrapTriggerProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] string roomTriggerName = RoomTriggerName)
      : base(dungeon.transform.Find(GetPath(roomName, roomTriggerName))
                    ?.gameObject?.GetComponent<TrapTrigger>()
             ?? throw new NullReferenceException($"{nameof(TrapTriggerProxy)} '{GetPath(roomName, roomTriggerName)}' not found.")
            )
    {
    }

    private static string GetPath(string roomName, string roomTriggerName) => $"Interior/Dungeon/Rooms/{roomName}/Spawners/{roomTriggerName}";

    public List<TrapSpawnerProxy> Spawners => RealObject.TrapSpawners.Select(trapSpawner => new TrapSpawnerProxy(trapSpawner.GetComponent<TrapSpawner>())).ToList();

    [UsedImplicitly]
    private bool ShouldTrigger(Collider other)
    {
      var character = other.gameObject.GetComponent<Character>();
      Log.Trace(_logger, $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} character == null : {character == null}");
      Log.Trace(_logger, $"{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name} character?.IsPlayer() : {character?.IsPlayer()}");
      return character != null && character.IsPlayer();
    }

    public void SetIsTriggered(bool value) => RealObject.SetIsTriggered(value);

  }
}
