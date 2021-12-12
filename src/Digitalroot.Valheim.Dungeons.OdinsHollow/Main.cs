using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.Dungeons.Common;
using Digitalroot.Valheim.Dungeons.Common.Rooms;
using Digitalroot.Valheim.Dungeons.OdinsHollow.Enums;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  [BepInPlugin(Guid, Name, Version)]
  public class Main : BaseUnityPlugin, ITraceableLogging
  {
    public const string Version = "1.0.0";
    public const string Name = "Odin's Hollow Dungeon by the Odin Plus Team";

    // ReSharper disable once MemberCanBePrivate.Global
    public const string Guid = "digitalroot.mods.dungeons.odinshollow";
    public const string Namespace = "Digitalroot.Valheim.Dungeons." + nameof(OdinsHollow);

    private Harmony _harmony;
    private AssetBundle _assetBundle;

    // ReSharper disable once MemberCanBePrivate.Global
    public static Main Instance;

    // ReSharper disable once IdentifierTypo
    private const string OdinsHollow = nameof(OdinsHollow);
    private Dungeon _dungeon;

    public Main()
    {
      Instance = this;
      #if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
      #else
      EnableTrace = false;
      #endif
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
    }

    [UsedImplicitly]
    public void Awake()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");

        RepositoryLoader.LoadAssembly("Digitalroot.Valheim.TrapSpawners.dll");

        _assetBundle = AssetUtils.LoadAssetBundleFromResources("op_dungeons", typeof(Main).Assembly);

        #if DEBUG
        foreach (var scene in _assetBundle.GetAllScenePaths())
        {
          Log.Trace(Instance, scene);
          SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(scene), LoadSceneMode.Additive);
        }

        foreach (var assetName in _assetBundle.GetAllAssetNames())
        {
          Log.Trace(Instance, assetName);
        }
        #endif
        PrefabManager.Instance.AddPrefab(new CustomPrefab(_assetBundle.LoadAsset<GameObject>(OdinsHollow), true));
        // PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;

        // _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #region Spawn Pool Seeding

    private void SeedGlobalSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding Global Spawn Pool");
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] _dungeon.GlobalSpawnPool == null : {_dungeon.GlobalSpawnPool == null}");

      if (_dungeon.GlobalSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of Global Spawn Pool");
        return;
      }

      _dungeon.GlobalSpawnPool?.Clear(); // Remove anything already in the GSP.
      _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.SkeletonPoison);
      _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.Blob);
      // _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.BlobElite);
      _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.Draugr);
      // _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.DraugrElite);
      _dungeon.GlobalSpawnPool?.AddEnemy(EnemyNames.DraugrRanged);
      // _dungeon.GlobalSpawnPool?.AddEnemy(PrefabNames.SkeletonNoArcher);
      // _dungeon.GlobalSpawnPool?.AddEnemy(PrefabNames.SkeletonNoArcher);
      // _dungeon.GlobalSpawnPool?.AddEnemy(PrefabNames.SkeletonNoArcher);
      // _dungeon.GlobalSpawnPool?.AddEnemy(PrefabNames.SkeletonNoArcher);
      // _dungeon.GlobalSpawnPool?.AddPrefab(PrefabNames.BonePileSpawner);
      // _dungeon.GlobalSpawnPool?.AddPrefab(PrefabNames.SpawnerDraugrPile);
    }

    private void SeedSpawnPoolsFor(DungeonBossRoom room)
    {
      Log.Trace(Instance, $"Seeding bosses for {room.Name}");
      Log.Trace(Instance, $"Room Health Check [{room.Name}]");
      Log.Trace(Instance, $"room.RoomBossSpawnPoints == null [{room.RoomBossSpawnPoint == null}]");
      Log.Trace(Instance, $"room.RoomBossTrigger == null [{room.RoomBossTrigger == null}]");

      room.RoomBossSpawnPoint?.AddBoss(EnemyNames.DraugrElite);

      SeedSpawnPoolsFor(room as DungeonRoom);
    }

    private void SeedSpawnPoolsFor(DungeonRoom room)
    {
      Log.Trace(Instance, $"Seeding trash for {room.Name}");
      Log.Trace(Instance, $"Room Health Check [{room.Name}]");
      Log.Trace(Instance, $"room.RoomSpawnPool == null : {room.RoomSpawnPool == null}");
      Log.Trace(Instance, $"room.RoomSpawnPool?.SpawnPoolCount() == null : {room.RoomSpawnPool?.SpawnPoolCount()}");
      Log.Trace(Instance, $"room.RoomTrigger == null : {room.RoomTrigger == null}");
      Log.Trace(Instance, $"room.RoomSpawnPoints == null : {room.RoomSpawnPoints == null}");
      Log.Trace(Instance, $"room.RoomSpawnPoints?.Count : {room.RoomSpawnPoints?.Count}");

      if (room.RoomSpawnPool == null) return;

      room.RoomSpawnPool?.AddEnemy(EnemyNames.Draugr);
      room.RoomSpawnPool?.AddEnemy(EnemyNames.DraugrRanged);
    }

    #endregion

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion

    #region Events

    [UsedImplicitly]
    private void OnDestroy()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
        _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    internal void OnObjectDBAwake(ref ObjectDB instance)
    {
    }

    internal void OnObjectDBCopyOtherDB(ref ObjectDB instance)
    {
    }

    internal void OnSpawnedPlayer(ref Game instance, Vector3 spawnPoint)
    {
    }

    private void OnVanillaPrefabsAvailable()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
        var dungeonPrefab = PrefabManager.Instance.GetPrefab(OdinsHollow);
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] dungeonPrefab == null : {dungeonPrefab == null}");
        Debug.Assert(dungeonPrefab != null, nameof(dungeonPrefab) + " != null");

        // Configure
        _dungeon = new Dungeon(OdinsHollow, dungeonPrefab);
        _dungeon.SetEnableTrace(EnableTrace);
        _dungeon.AddDungeonBossRoom(DungeonsRoomNames.BlueRoom);

        // Seed
        SeedGlobalSpawnPoolIfNecessary();
        foreach (var dungeonDungeonBossRoom in _dungeon.DungeonBossRooms)
        {
          SeedSpawnPoolsFor(dungeonDungeonBossRoom);
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
      finally
      {
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
      }
    }

    internal void OnZNetAwake(ref ZNet instance)
    {
    }

    internal void OnZNetSceneAwake(ref ZNetScene instance)
    {
    }

    #endregion
  }
}
