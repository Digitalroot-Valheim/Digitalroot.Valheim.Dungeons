using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.TrapSpawners;
using Digitalroot.Valheim.TrapSpawners.Enums;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
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
    public SpawnerType SpawnerType => RealObject.m_spawnerType;
    public float ScaleSize => RealObject.m_scaleSize;
    public int LevelMin => RealObject.m_levelMin;
    public int LevelMax => RealObject.m_levelMax;
    public int QuantityMin => RealObject.m_quantityMin;
    public int QuantityMax => RealObject.m_quantityMax;
    public void DoSpawn([CanBeNull] List<GameObject> spawnPoolPrefabs = null, int quantityMin = -1, int quantityMax = -1, int levelMin = -1, int levelMax = -1) => RealObject.DoSpawn(spawnPoolPrefabs, quantityMin, quantityMax, levelMin, levelMax);

    private static string GetPath(string roomName, string roomSpawnPointName) => $"Interior/Dungeon/Rooms/{roomName}/Spawners/{roomSpawnPointName}";

    private TrapSpawnerProxy([NotNull] TrapSpawner realObject, [NotNull] ITraceableLogging logger)
      : base(realObject, logger)
    {
      Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Spawner ({realObject.name})");
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
