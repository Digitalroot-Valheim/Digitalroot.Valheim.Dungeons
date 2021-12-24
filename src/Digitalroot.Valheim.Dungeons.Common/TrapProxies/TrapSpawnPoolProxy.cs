using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Dungeons.Common.Utils;
using Digitalroot.Valheim.TrapSpawners;
using JetBrains.Annotations;
using Jotunn.Managers;
using System;
using System.Reflection;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Digitalroot.Valheim.Dungeons.Common.TrapProxies
{
  public class TrapSpawnPoolProxy : AbstractProxy<TrapSpawnPool>, ISpawnPool
  {
    private const string RoomSpawnPoolName = "SpawnPool";

    private static string GetPath(string roomName, string roomSpawnPoolName) => $"Interior/Dungeon/Rooms/{roomName}/Spawners/{roomSpawnPoolName}";

    // ReSharper disable once MemberCanBeProtected.Global
    private protected TrapSpawnPoolProxy([NotNull] TrapSpawnPool realObject, [NotNull] ITraceableLogging logger)
      : base(realObject, logger)
    {
      Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Creating Spawn Pool ({realObject.name})");
      realObject.LogEvent += HandleLogEvent;
    }

    private protected TrapSpawnPoolProxy([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] ITraceableLogging logger, [NotNull] string roomSpawnPoolName = RoomSpawnPoolName)
      : this(dungeon.transform.Find(GetPath(roomName, roomSpawnPoolName))
                    ?.gameObject?.GetComponent<TrapSpawnPool>()
             ?? throw new NullReferenceException($"{nameof(TrapSpawnPoolProxy)} '{GetPath(roomName, roomSpawnPoolName)}' not found."), logger) { }

    public static TrapSpawnPoolProxy CreateInstance([NotNull] TrapSpawnPool realObject, [NotNull] ITraceableLogging logger)
    {
      return new TrapSpawnPoolProxy(realObject, logger);
    }

    public static TrapSpawnPoolProxy CreateInstance([NotNull] GameObject dungeon, [NotNull] string roomName, [NotNull] ITraceableLogging logger, [NotNull] string roomSpawnPoolName = RoomSpawnPoolName)
    {
      return new TrapSpawnPoolProxy(dungeon, roomName, logger, roomSpawnPoolName);
    }

    #region Implementation of ISpawnPool

    /// <inheritdoc />
    public void Clear() => RealObject.Clear();

    /// <inheritdoc />
    public void AddEnemy(GameObject prefab) => RealObject.AddEnemy(DungeonsUtils.ConfigureAsTrash(prefab, Logger));

    /// <inheritdoc />
    public void AddEnemy(string prefabName) => AddEnemy(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddBoss(GameObject prefab) => RealObject.AddBoss(DungeonsUtils.ConfigureAsBoss(prefab, Logger));

    /// <inheritdoc />
    public void AddBoss(string prefabName) => AddBoss(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public void AddPrefab(GameObject prefab) => RealObject.AddPrefab(prefab);

    /// <inheritdoc />
    public void AddPrefab(string prefabName) => AddPrefab(PrefabManager.Cache.GetPrefab<GameObject>(prefabName));

    /// <inheritdoc />
    public int Count => RealObject.Count;

    #endregion
  }
}
