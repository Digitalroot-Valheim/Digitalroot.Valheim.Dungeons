using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.Dungeons.Common;
using Digitalroot.Valheim.Dungeons.Common.Enums;
using Digitalroot.Valheim.Dungeons.Common.Logging;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.OdinsHollow.Enums;
using Digitalroot.Valheim.TrapSpawners.Logging;
using JetBrains.Annotations;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;

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
    private const string TriggerTest = nameof(TriggerTest);
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
        Log.Trace(Instance, Application.consoleLogPath);
        Log.Trace(Instance, Application.dataPath);
        Log.Trace(Instance, Application.persistentDataPath);
        Log.Trace(Instance, Application.streamingAssetsPath);
        Log.Trace(Instance, Application.temporaryCachePath);
        Log.Trace(Instance, Application.internetReachability);
        Log.Trace(Instance, Application.platform);

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
        // var eventLogCollector = prefab.GetComponent<EventLogCollector>();
        // eventLogCollector.LogEvent += _eventLogHandler.HandleLogEvent;
        PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab, false));
        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;

        var prefab2 = _assetBundle.LoadAsset<GameObject>(TriggerTest);
        PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab2, false));
        
        // _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #region Spawn Pool Seeding

    private void SeedGlobalSpawnPoolsIfNecessary()
    {
      SeedGlobalEnemySpawnPoolIfNecessary();
      SeedGlobalMiniBossSpawnPoolIfNecessary();
      SeedGlobalDestructibleSpawnPoolIfNecessary();
      SeedGlobalTreasureSpawnPoolIfNecessary();
    }

    private void SeedGlobalEnemySpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalEnemySpawnPool)}");
      // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] _dungeon.GlobalEnemySpawnPool == null : {_dungeon.GlobalEnemySpawnPoolProxy == null}");

      if (_dungeon.GlobalEnemySpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalEnemySpawnPool)}");
        return;
      }

      _dungeon.GlobalEnemySpawnPool?.Clear(); // Remove anything already in the GSP.
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.SkeletonPoison);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Blob);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.BlobElite);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.BlobTar);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Draugr);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.DraugrElite);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.DraugrRanged);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.SkeletonNoArcher);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Ghost);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Wraith);
      _dungeon.GlobalEnemySpawnPool?.AddEnemy(EnemyNames.Surtling);
    }

    private void SeedGlobalDestructibleSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalDestructibleSpawnPool)}");

      if (_dungeon.GlobalDestructibleSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalDestructibleSpawnPool)}");
        return;
      }

      _dungeon.GlobalDestructibleSpawnPool?.Clear(); // Remove anything already in the GSP.
      // _dungeon.GlobalLootableSpawnPool?.AddPrefab(PrefabNames.Mudpile);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.Mudpile2);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.MineRockTin);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.MineRockCopper);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.Rock3Silver);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableBogIronOre);
      
    }

    private void SeedGlobalMiniBossSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalMiniBossSpawnPool)}");

      if (_dungeon.GlobalMiniBossSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalMiniBossSpawnPool)}");
        return;
      }

      _dungeon.GlobalMiniBossSpawnPool?.Clear(); // Remove anything already in the GSP.
      _dungeon.GlobalMiniBossSpawnPool?.AddBoss(EnemyNames.SkeletonPoison);
      _dungeon.GlobalMiniBossSpawnPool?.AddBoss(EnemyNames.Draugr);
      _dungeon.GlobalMiniBossSpawnPool?.AddBoss(EnemyNames.DraugrElite);
      _dungeon.GlobalMiniBossSpawnPool?.AddBoss(EnemyNames.DraugrRanged);
      _dungeon.GlobalMiniBossSpawnPool?.AddBoss(EnemyNames.SkeletonNoArcher);
    }

    private void SeedGlobalTreasureSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalTreasureSpawnPool)}");

      if (_dungeon.GlobalTreasureSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalTreasureSpawnPool)}");
        return;
      }

      _dungeon.GlobalTreasureSpawnPool?.Clear(); // Remove anything already in the GSP.
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableDolmenTreasure);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableForestCryptRandom);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableSunkenCryptRandom);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableForestCryptRemains01);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableForestCryptRemains02);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableForestCryptRemains03);
      _dungeon.GlobalDestructibleSpawnPool?.AddPrefab(PrefabNames.PickableForestCryptRemains04);
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

        #if DEBUG
        var trapTestPrefab = PrefabManager.Instance.GetPrefab(TriggerTest);

        if (trapTestPrefab == null)
        {
          throw new NullReferenceException(nameof(trapTestPrefab) + " is null.");
        }

        var enemySpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Enemy, trapTestPrefab, Instance);
        var miniBossSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.MiniBoss, trapTestPrefab, Instance);
        var destructibleSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Destructible, trapTestPrefab, Instance);
        var treasureSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Treasure, trapTestPrefab, Instance);

        enemySpawnPool.Clear();
        enemySpawnPool.AddEnemy(EnemyNames.Draugr);
        miniBossSpawnPool.Clear();
        miniBossSpawnPool.AddBoss(EnemyNames.DraugrElite);
        destructibleSpawnPool.Clear();
        destructibleSpawnPool.AddPrefab(PrefabNames.Mudpile2);
        destructibleSpawnPool.AddPrefab(PrefabNames.MineRockTin);
        destructibleSpawnPool.AddPrefab(PrefabNames.MineRockCopper);
        destructibleSpawnPool.AddPrefab(PrefabNames.Rock3Silver);
        destructibleSpawnPool.AddPrefab(PrefabNames.PickableBogIronOre);
        treasureSpawnPool.Clear();
        treasureSpawnPool.AddPrefab(PrefabNames.Ruby);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableDolmenTreasure);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableForestCryptRandom);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableSunkenCryptRandom);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableForestCryptRemains01);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableForestCryptRemains02);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableForestCryptRemains03);
        treasureSpawnPool.AddPrefab(PrefabNames.PickableForestCryptRemains04);
        
        #endif

        var dungeonPrefab = PrefabManager.Instance.GetPrefab(OdinsHollow);
        // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] dungeonPrefab == null : {dungeonPrefab == null}");
        if (dungeonPrefab == null)
        {
          throw new NullReferenceException(nameof(dungeonPrefab) + " is null.");
        }

        // for (int i = 0; i < dungeonPrefab.transform.childCount; i++)
        // {
        //   Log.Trace(Instance, $"[{Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}] {dungeonPrefab.transform.GetChild(i).name}");
        // }

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
        SeedGlobalSpawnPoolsIfNecessary();
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
