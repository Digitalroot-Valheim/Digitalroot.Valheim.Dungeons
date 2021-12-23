using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using System;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  [UsedImplicitly]
  public class TrapSpawnerProxy : AbstractProxy<TrapSpawner>
  {
    private const string RoomSpawnPointName = "SpawnPoint";
    public ISpawnPool SpawnPool => RealObject.SpawnPool;
    public string Name => RealObject.name;
    public bool IsBoss => RealObject.m_isBoss;

    private static string GetPath(string roomName, string roomSpawnPointName) => $"Interior/Dungeon/Rooms/{roomName}/Spawners/{roomSpawnPointName}";

    private TrapSpawnerProxy([NotNull] TrapSpawner realObject, [NotNull] ITraceableLogging logger)
      : base(realObject, logger)
    {
      Log.Trace(_logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Spawner ({realObject.name})");
      realObject.LogEvent += HandleLogEvent;
    }

    private TrapSpawnerProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] ITraceableLogging logger, [NotNull] string roomSpawnPointName = RoomSpawnPointName)
      : this(dungeon.transform.Find(GetPath(roomName, roomSpawnPointName))
                    ?.gameObject?.GetComponent<TrapSpawner>()
             ?? throw new NullReferenceException($"{nameof(TrapSpawnerProxy)} '{GetPath(roomName, roomSpawnPointName)}' not found."), logger) { }

    public static TrapSpawnerProxy CreateInstance([NotNull] TrapSpawner realObject, [NotNull] ITraceableLogging logger)
    {
      return new TrapSpawnerProxy(realObject, logger);
    }

    public static TrapSpawnerProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] ITraceableLogging logger, [NotNull] string roomSpawnPointName = RoomSpawnPointName)
    {
      return new TrapSpawnerProxy(dungeon, roomName, logger, roomSpawnPointName);
    }
  }
}
