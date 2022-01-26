using Digitalroot.Valheim.Common;
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
      Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Creating Spawn Pool ({realObject.name})");
      // realObject.LogEvent += HandleLogEvent;
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
    public void AddEnemy([NotNull] GameObject prefab) => AddPrefab(prefab);

    /// <inheritdoc />
    public void AddEnemy([NotNull] string prefabName) => AddPrefab(prefabName);

    /// <inheritdoc />
    public void AddMiniBoss([NotNull] GameObject prefab) => AddPrefab(prefab);

    /// <inheritdoc />
    public void AddMiniBoss([NotNull] string prefabName) => AddPrefab(prefabName);

    /// <inheritdoc />
    public void AddPrefab([NotNull] GameObject prefab)
    {
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] prefab.name : {prefab.name}");
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] RealObject == null : {RealObject == null}");
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] prefab == null : {prefab == null}");
      
      if (RealObject == null) return;
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] RealObject : {RealObject}");
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] RealObject.GetType().Name : {RealObject.GetType().Name}");
      RealObject.AddPrefab(prefab);
    }

    /// <inheritdoc />
    public void AddPrefab([NotNull] string prefabName)
    {
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] prefabName : {prefabName}");
      var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
      // Log.Trace(Logger, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] prefab == null : {prefab == null}");
      if (prefab == null) return;
      
      AddPrefab(prefab);
    }

    /// <inheritdoc />
    public int Count => RealObject.Count;

    #endregion
  }
}
