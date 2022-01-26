using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.Dungeons.Common;
using Digitalroot.Valheim.Dungeons.Common.Enums;
using Digitalroot.Valheim.Dungeons.Common.SpawnPools;
using Digitalroot.Valheim.Dungeons.OdinsHollow.Enums;
using Digitalroot.Valheim.TrapSpawners.CMB;
using JetBrains.Annotations;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  [BepInPlugin(Guid, Name, Version)]
  public class Main : BaseUnityPlugin, ITraceableLogging
  {
    public const string Version = "1.0.0";
    public const string Name = "Odin's Hollow Dungeon by the OdinPlus Team";

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
    // private readonly EventLogHandler _eventLogHandler;

    private readonly List<string> _destructibleBasePrefabNameList = new()
    {
      PrefabNames.Mudpile2
      , PrefabNames.MineRockIron
      , PrefabNames.MineRockTin
      , PrefabNames.MineRockCopper
      , PrefabNames.MineRockObsidian
    };

    private readonly List<string> _enemyBasePrefabNameList = new()
    {
      EnemyNames.Draugr
      , EnemyNames.DraugrElite
      , EnemyNames.DraugrRanged
      , EnemyNames.Wraith
      , EnemyNames.SkeletonNoArcher
      , EnemyNames.SkeletonPoison
      , EnemyNames.Surtling
      , EnemyNames.BlobTar
      , EnemyNames.Blob
      , EnemyNames.BlobElite
      , EnemyNames.Ghost
    };

    private readonly List<string> _miniBossBasePrefabNameList = new()
    {
      EnemyNames.Draugr
      , EnemyNames.DraugrElite
      , EnemyNames.DraugrRanged
      , EnemyNames.SkeletonNoArcher
      , EnemyNames.SkeletonPoison
    };

    private readonly List<string> _treasureBasePrefabNameList = new()
    {
      PrefabNames.PickableDolmenTreasure
      , PrefabNames.PickableForestCryptRandom
      , PrefabNames.PickableSunkenCryptRandom
      , PrefabNames.PickableForestCryptRemains01
      , PrefabNames.PickableForestCryptRemains02
      , PrefabNames.PickableForestCryptRemains03
      , PrefabNames.PickableForestCryptRemains04
      , PrefabNames.PickableBogIronOre
      , PrefabNames.PickableFlint
      , PrefabNames.PickableObsidian
      , PrefabNames.PickableMushroom
      , PrefabNames.PickableMushroomBlue
      , PrefabNames.PickableMushroomYellow
      , PrefabNames.PickableTin
      , PrefabNames.PickableStone
      , PrefabNames.PickableSurtlingCoreStand
      , PrefabNames.PickableTar
      , PrefabNames.PickableTarBig
      , PrefabNames.PickableBranch
    };

    public Main()
    {
      Instance = this;
      #if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
      // Log.SetEnableTraceForAllLoggers(true);
      #else
        EnableTrace = false;
      #endif
      // _eventLogHandler = new EventLogHandler(Instance);
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
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

        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

        RepositoryLoader.LoadAssembly("Digitalroot.Valheim.TrapSpawners.dll");

        _assetBundle = AssetUtils.LoadAssetBundleFromResources("op_dungeons", typeof(Main).Assembly);

        var prefab = _assetBundle.LoadAsset<GameObject>(OdinsHollow);
        PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab, false));

        var prefab2 = _assetBundle.LoadAsset<GameObject>(TriggerTest);
        PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab2, false));

        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
        PrefabManager.OnPrefabsRegistered += OnPrefabsRegistered;

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
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalEnemySpawnPool)}");
      // Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] _dungeon.GlobalEnemySpawnPool == null : {_dungeon.GlobalEnemySpawnPoolProxy == null}");

      if (_dungeon.GlobalEnemySpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalEnemySpawnPool)}");
        return;
      }

      _dungeon.GlobalEnemySpawnPool?.Clear(); // Remove anything already in the GSP.
      foreach (var enemyBasePrefabName in _enemyBasePrefabNameList)
      {
        _dungeon.GlobalEnemySpawnPool?.AddEnemy($"{nameof(OdinsHollow)}_{enemyBasePrefabName}");
      }
    }

    private void SeedGlobalDestructibleSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalDestructibleSpawnPool)}");

      if (_dungeon.GlobalDestructibleSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalDestructibleSpawnPool)}");
        return;
      }

      _dungeon.GlobalDestructibleSpawnPool?.Clear(); // Remove anything already in the GSP.
      foreach (var destructibleBasePrefabName in _destructibleBasePrefabNameList)
      {
        _dungeon.GlobalDestructibleSpawnPool?.AddPrefab($"{nameof(OdinsHollow)}_{destructibleBasePrefabName}");
      }
    }

    private void SeedGlobalMiniBossSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalMiniBossSpawnPool)}");

      if (_dungeon.GlobalMiniBossSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalMiniBossSpawnPool)}");
        return;
      }

      _dungeon.GlobalMiniBossSpawnPool?.Clear(); // Remove anything already in the GSP.
      foreach (var miniBossBasePrefabName in _miniBossBasePrefabNameList)
      {
        _dungeon.GlobalMiniBossSpawnPool?.AddPrefab($"{nameof(OdinsHollow)}_{miniBossBasePrefabName}");
      }
    }

    private void SeedGlobalTreasureSpawnPoolIfNecessary()
    {
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Seeding {nameof(_dungeon.GlobalTreasureSpawnPool)}");

      if (_dungeon.GlobalTreasureSpawnPool == null)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}] Skipping Seeding of {nameof(_dungeon.GlobalTreasureSpawnPool)}");
        return;
      }

      _dungeon.GlobalTreasureSpawnPool?.Clear(); // Remove anything already in the GSP.
      foreach (var treasureBasePrefabName in _treasureBasePrefabNameList)
      {
        _dungeon.GlobalTreasureSpawnPool?.AddPrefab($"{nameof(OdinsHollow)}_{treasureBasePrefabName}");
      }
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

    private void PrintSpawnPoolInfo()
    {
      // Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}({basePrefabName})");
    }

    #region Init

    private void CreateDungeonCreature(string basePrefabName)
    {
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}({basePrefabName})");
      var prefab = PrefabManager.Instance.CreateClonedPrefab($"{nameof(OdinsHollow)}_{basePrefabName}", basePrefabName);
      var customPrefab = new CustomPrefab(prefab, false);
      var humanoid = customPrefab.Prefab.GetComponent<Humanoid>();
      humanoid.m_faction = Character.Faction.Undead;
      customPrefab.Prefab.AddComponent<DungeonCreature>();

      PrefabManager.Instance.AddPrefab(customPrefab);
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {customPrefab.Prefab.name}");
    }

    private void CreateDungeonCreatures(List<string> basePrefabNames)
    {
      foreach (var basePrefabName in basePrefabNames)
      {
        CreateDungeonCreature(basePrefabName);
      }
    }

    private void CreateDungeonDestructible(string basePrefabName)
    {
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}({basePrefabName})");
      var prefab = PrefabManager.Instance.CreateClonedPrefab($"{nameof(OdinsHollow)}_{basePrefabName}", basePrefabName);
      var customPrefab = new CustomPrefab(prefab, false);
      var staticPhysics = customPrefab.Prefab.GetComponent<StaticPhysics>();
      if (staticPhysics != null)
      {
        staticPhysics.m_fall = false;
      }

      PrefabManager.Instance.AddPrefab(customPrefab);
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {customPrefab.Prefab.name}");
    }

    private void CreateDungeonDestructibles(List<string> basePrefabNames)
    {
      foreach (var basePrefabName in basePrefabNames)
      {
        CreateDungeonDestructible(basePrefabName);
      }
    }

    private void CreateDungeonMiniBoss(string basePrefabName)
    {
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}({basePrefabName})");
      var prefab = PrefabManager.Instance.CreateClonedPrefab($"{nameof(OdinsHollow)}_MiniBoss_{basePrefabName}", basePrefabName);
      var customPrefab = new CustomPrefab(prefab, false);

      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] basePrefabName : {basePrefabName}, {basePrefabName.GetStableHashCode()}");
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] prefab.name : {prefab.name}, {prefab.name.GetStableHashCode()}");
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] customPrefab.Prefab.name : {customPrefab.Prefab.name}, {customPrefab.Prefab.name.GetStableHashCode()}");

      // customPrefab.Prefab.name = $"{nameof(OdinsHollow)}_MiniBoss_{customPrefab.Prefab.name}";
      var humanoid = customPrefab.Prefab.GetComponent<Humanoid>();
      humanoid.m_faction = Character.Faction.Undead;
      customPrefab.Prefab.AddComponent<DungeonCreature>();

      PrefabManager.Instance.AddPrefab(customPrefab);
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {customPrefab.Prefab.name}");
    }

    private void CreateDungeonMiniBosses(List<string> basePrefabNames)
    {
      foreach (var basePrefabName in basePrefabNames)
      {
        CreateDungeonMiniBoss(basePrefabName);
      }
    }

    private void CreateDungeonTreasure(string basePrefabName)
    {
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}({basePrefabName})");
      var prefab = PrefabManager.Instance.CreateClonedPrefab($"{nameof(OdinsHollow)}_{basePrefabName}", basePrefabName);
      var customPrefab = new CustomPrefab(prefab, false);
      var staticPhysics = customPrefab.Prefab.GetComponent<StaticPhysics>();
      if (staticPhysics != null)
      {
        staticPhysics.m_fall = false;
      }

      PrefabManager.Instance.AddPrefab(customPrefab);
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {customPrefab.Prefab.name}");
    }

    private void CreateDungeonTreasures(List<string> basePrefabNames)
    {
      foreach (var basePrefabName in basePrefabNames)
      {
        CreateDungeonTreasure(basePrefabName);
      }
    }

    private void LoadOdinsHollow()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

        var dungeonPrefab = PrefabManager.Instance.GetPrefab(OdinsHollow);
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
        SeedGlobalSpawnPoolsIfNecessary();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [Conditional("DEBUG")]
    private void LoadTriggerTest()
    {
      Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}()");
      var trapTestPrefab = PrefabManager.Instance.GetPrefab(TriggerTest);

      if (trapTestPrefab == null)
      {
        throw new NullReferenceException(nameof(trapTestPrefab) + " is null.");
      }

      var enemySpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Enemy, trapTestPrefab, Instance);
      var miniBossSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.MiniBoss, trapTestPrefab, Instance);
      var destructibleSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Destructible, trapTestPrefab, Instance);
      var treasureSpawnPool = GlobalSpawnPoolFactory.CreateInstance(GlobalSpawnPoolNames.Treasure, trapTestPrefab, Instance);

      
      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Seeding enemySpawnPool");
      enemySpawnPool.Clear();
      foreach (var basePrefabName in _enemyBasePrefabNameList)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Trying to add {basePrefabName}");
        enemySpawnPool.AddEnemy($"{nameof(OdinsHollow)}_{basePrefabName}");
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {nameof(OdinsHollow)}_{basePrefabName}");
      }

      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Seeding miniBossSpawnPool");
      miniBossSpawnPool.Clear();
      foreach (var basePrefabName in _miniBossBasePrefabNameList)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Trying to add {basePrefabName}");
        miniBossSpawnPool.AddMiniBoss($"{nameof(OdinsHollow)}_MiniBoss_{basePrefabName}");
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {nameof(OdinsHollow)}_MiniBoss_{basePrefabName}");
      }

      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Seeding destructibleSpawnPool");
      destructibleSpawnPool.Clear();
      foreach (var basePrefabName in _destructibleBasePrefabNameList)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Trying to add {basePrefabName}");
        destructibleSpawnPool.AddPrefab($"{nameof(OdinsHollow)}_{basePrefabName}");
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {nameof(OdinsHollow)}_{basePrefabName}");
      }

      Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Seeding treasureSpawnPool");
      treasureSpawnPool.Clear();
      foreach (var basePrefabName in _treasureBasePrefabNameList)
      {
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Trying to add {basePrefabName}");
        treasureSpawnPool.AddPrefab($"{nameof(OdinsHollow)}_{basePrefabName}");
        Log.Trace(Instance, $"[{MethodBase.GetCurrentMethod()?.Name}] Added {nameof(OdinsHollow)}_{basePrefabName}");
      }
    }

    #endregion

    #region Events

    [UsedImplicitly]
    public void OnDestroy()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}()");

        // var oh = PrefabManager.Cache.GetPrefab<GameObject>(OdinsHollow);
        //var eventLogCollector = oh?.GetComponent<EventLogCollector>();
        //if (eventLogCollector != null) eventLogCollector.LogEvent -= _eventLogHandler.HandleLogEvent;

        // _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [UsedImplicitly]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UsedImplicitly")]
    private void OnDisable()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}()");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void OnPrefabsRegistered()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
        LoadTriggerTest();
        LoadOdinsHollow();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
      finally
      {
        PrefabManager.OnPrefabsRegistered -= OnPrefabsRegistered;
      }
    }

    private void OnVanillaPrefabsAvailable()
    {
      try
      {
        Log.Trace(Instance, $"{Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

        CreateDungeonCreatures(_enemyBasePrefabNameList);
        CreateDungeonMiniBosses(_miniBossBasePrefabNameList);
        CreateDungeonDestructibles(_destructibleBasePrefabNameList);
        CreateDungeonTreasures(_treasureBasePrefabNameList);
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

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
