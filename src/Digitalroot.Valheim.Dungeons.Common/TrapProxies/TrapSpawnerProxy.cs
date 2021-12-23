using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  [UsedImplicitly]
  public class TrapSpawnerProxy : AbstractProxy<TrapSpawner>
  {
    private const string RoomSpawnPointName = "SpawnPoint";
    private static string GetPath(string roomName, string roomSpawnPointName) => $"Interior/Dungeon/Rooms/{roomName}/Spawners/{roomSpawnPointName}";

    public TrapSpawnerProxy([NotNull] TrapSpawner trapSpawner)
      : base(trapSpawner) { }

    public TrapSpawnerProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] string roomSpawnPointName = RoomSpawnPointName)
      : base(dungeon.transform.Find(GetPath(roomName, roomSpawnPointName))
                    ?.gameObject?.GetComponent<TrapSpawner>()
             ?? throw new NullReferenceException($"{nameof(TrapSpawnerProxy)} '{GetPath(roomName, roomSpawnPointName)}' not found.")) { }

    public ISpawnPool SpawnPool => RealObject.SpawnPool;

  }
}
