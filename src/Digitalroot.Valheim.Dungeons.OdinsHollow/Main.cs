using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.Dungeons.Common;
using Digitalroot.Valheim.Dungeons.Common.Logging;
using Digitalroot.Valheim.Dungeons.OdinsHollow.Enums;
using Digitalroot.Valheim.TrapSpawners.Logging;
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

    // private Harmony _harmony;
    private AssetBundle _assetBundle;

    // ReSharper disable once MemberCanBePrivate.Global
    public static Main Instance;

    // ReSharper disable once IdentifierTypo
    private const string OdinsHollow = nameof(OdinsHollow);
    private Dungeon _dungeon;
    private readonly EventLogHandler _eventLogHandler;

    public Main()
    {
      Instance = this;
      #if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
      Log.SetEnableTraceForAllLoggers(true);
      #else
      EnableTrace = false;
      #endif
      _eventLogHandler = new EventLogHandler(Instance);
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

        #if DEBUG2

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

        var prefab = _assetBundle.LoadAsset<GameObject>(OdinsHollow);
        var eventLogCollector = prefab.GetComponent<EventLogCollector>();
        eventLogCollector.LogEvent += _eventLogHandler.HandleLogEvent;
        PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab, true));
        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;

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
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalEnemySpawnPoolProxy)}");
      // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] _dungeon.GlobalEnemySpawnPool == null : {_dungeon.GlobalEnemySpawnPoolProxy == null}");

      if (_dungeon.GlobalEnemySpawnPoolProxy == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalEnemySpawnPoolProxy)}");
        return;
      }

      _dungeon.GlobalEnemySpawnPoolProxy?.Clear(); // Remove anything already in the GSP.
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.SkeletonPoison);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.Blob);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.BlobElite);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.BlobTar);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.Draugr);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.DraugrElite);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.DraugrRanged);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.SkeletonNoArcher);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.Ghost);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.Wraith);
      // _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Abomination);
      _dungeon.GlobalEnemySpawnPoolProxy?.AddEnemy(EnemyNames.Surtling);
      
      // _dungeon.GlobalEnemySpawnPool?.AddEnemy(PrefabNames.SkeletonNoArcher);
      // _dungeon.GlobalEnemySpawnPool?.AddPrefab(PrefabNames.BonePileSpawner);
      // _dungeon.GlobalEnemySpawnPool?.AddPrefab(PrefabNames.SpawnerDraugrPile);
    }

    private void SeedGlobalLootableSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalLootableSpawnPoolProxy)}");
      // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] _dungeon.GlobalLootableSpawnPool == null : {_dungeon.GlobalLootableSpawnPoolProxy == null}");

      if (_dungeon.GlobalLootableSpawnPoolProxy == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalLootableSpawnPoolProxy)}");
        return;
      }

      _dungeon.GlobalLootableSpawnPoolProxy?.Clear(); // Remove anything already in the GSP.
      // _dungeon.GlobalLootableSpawnPool?.AddPrefab(PrefabNames.Mudpile);
      _dungeon.GlobalLootableSpawnPoolProxy?.AddPrefab(PrefabNames.Mudpile2); // MP2 just looks so much better
    }

    // private void SeedSpawnPoolsFor(DungeonBossRoom room)
    // {
    //   Log.Trace(Instance, $"Seeding bosses for {room.Name}");
    //   Log.Trace(Instance, $"Room Health Check [{room.Name}]");
    //   Log.Trace(Instance, $"room.RoomBossSpawnPoints == null [{room.RoomBossSpawnPoint == null}]");
    //   Log.Trace(Instance, $"room.RoomBossSpawnPoints.Count() [{room.RoomBossSpawnPoint?.Count()}]");
    //   // Log.Trace(Instance, $"room.RoomBossTrigger == null [{room.RoomBossTrigger == null}]");
    //
    //   // room.RoomBossSpawnPoint?.SpawnPool.AddBoss(EnemyNames.DraugrElite);
    //
    //   // SeedSpawnPoolsFor(room as DungeonRoom);
    // }

    // private void SeedSpawnPoolsFor(DungeonRoom room)
    // {
    //   Log.Trace(Instance, $"Seeding trash for {room.Name}");
    //   Log.Trace(Instance, $"Room Health Check [{room.Name}]");
    //   // Log.Trace(Instance, $"room.RoomSpawnPool == null : {room.RoomSpawnPool == null}");
    //   // Log.Trace(Instance, $"room.RoomSpawnPool?.Count() == null : {room.RoomSpawnPool?.Count}");
    //   Log.Trace(Instance, $"room.RoomTrigger == null : {room.RoomTrigger == null}");
    //   Log.Trace(Instance, $"room.RoomSpawnPoints == null : {room.RoomSpawnPoints == null}");
    //   Log.Trace(Instance, $"room.RoomSpawnPoints?.Count : {room.RoomSpawnPoints?.Count()}");
    //
    //   // if (room.RoomSpawnPool == null) return;
    //   // room.RoomSpawnPool?.AddEnemy(EnemyNames.Draugr);
    //   // room.RoomSpawnPool?.AddEnemy(EnemyNames.DraugrRanged);
    // }

    #endregion

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion

    #region Events

    [UsedImplicitly]
    public void OnDestroy()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}()");

        var oh = PrefabManager.Cache.GetPrefab<GameObject>(OdinsHollow);
        var eventLogCollector = oh?.GetComponent<EventLogCollector>();
        if (eventLogCollector != null) eventLogCollector.LogEvent -= _eventLogHandler.HandleLogEvent;

        // _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void OnVanillaPrefabsAvailable()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
        var dungeonPrefab = PrefabManager.Instance.GetPrefab(OdinsHollow);
        // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] dungeonPrefab == null : {dungeonPrefab == null}");
        if (dungeonPrefab == null)
        {
          throw new NullReferenceException(nameof(dungeonPrefab) + " is null.");
        }

        // Configure
        _dungeon = new Dungeon(OdinsHollow, dungeonPrefab, Instance);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveEntrance);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall1);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall2);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveBridge);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall3);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveRoom1);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall4);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall5);
        _dungeon.AddDungeonBossRoom(DungeonsRoomNames.RedRoom);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveHall6);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveRoom2);
        _dungeon.AddDungeonBossRoom(DungeonsRoomNames.GreenRoom);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveRoom3);
        _dungeon.AddDungeonRoom(DungeonsRoomNames.CaveRoom4);
        _dungeon.AddDungeonBossRoom(DungeonsRoomNames.BlueRoom);

        // Seed
        SeedGlobalSpawnPoolIfNecessary();
        SeedGlobalLootableSpawnPoolIfNecessary();
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

    #endregion
  }
}
