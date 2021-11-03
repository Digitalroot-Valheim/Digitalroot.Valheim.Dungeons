using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.TrapSpawners;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Digitalroot.Valheim.DungeonsDemo
{
  public class DungeonBossRoom : DungeonRoom
  {
    public readonly string MiniBossTrigger = "MiniBoss_Trigger";
    public readonly string MiniBossSpawnPoint = "MiniBoss_SpawnPoint";

    /// <inheritdoc />
    public DungeonBossRoom(string name)
      : base(name)
    {
    }
  }

  public class DungeonRoom
  {
    public DungeonRoom(string name)
    {
      Name = name;
    }

    public readonly string Name;
    public readonly string RoomTrigger = "Room_Trigger";
    public readonly string RoomSpawnPool = "Room_SpawnPool";
    public readonly string RoomSpawnPoint = "Room_SpawnPoint";
  }

  [BepInPlugin(Guid, Name, Version)]
  public class Main : BaseUnityPlugin, ITraceableLogging
  {
    public const string Version = "1.0.0";
    public const string Name = "Digitalroot Dungeons Demo";

    // ReSharper disable once MemberCanBePrivate.Global
    public const string Guid = "digitalroot.mods.DungeonsDemo";
    public const string Namespace = "Digitalroot.Valheim." + nameof(DungeonsDemo);
    private Harmony _harmony;
    private AssetBundle _assetBundle;

    // ReSharper disable once MemberCanBePrivate.Global
    public static Main Instance;

    // ReSharper disable once IdentifierTypo
    private const string OdinsHollow = nameof(OdinsHollow);
    private const string BlueRoom = "Blue_Room";
    private const string GreenRoom = "Green_Room";
    private const string RedRoom = "Red_Room";
    private const string MiniBossSpawnPoint = "MiniBoss_SpawnPoint";
    private const string MiniBossTrigger = "MiniBossTrigger";

    public Main()
    {
      Instance = this;
#if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
#else
      EnableTrace = false;
#endif
      Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");

        RepositoryLoader.LoadAssembly("Digitalroot.Valheim.TrapSpawners.dll");

        _assetBundle = AssetUtils.LoadAssetBundleFromResources("op_dungeons", typeof(Main).Assembly);

#if DEBUG
        foreach (var scene in _assetBundle.GetAllScenePaths())
        {
          Log.Trace(Main.Instance, scene);
          SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(scene), LoadSceneMode.Additive);
        }

        foreach (var assetName in _assetBundle.GetAllAssetNames())
        {
          Log.Trace(Main.Instance, assetName);
        }
#endif
        PrefabManager.Instance.AddPrefab(new CustomPrefab(_assetBundle.LoadAsset<GameObject>(OdinsHollow), true));

        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;

        _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
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
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");

        // ReSharper disable once IdentifierTypo
        var odinsHollow = PrefabManager.Instance.GetPrefab(OdinsHollow);
        // ReSharper disable once StringLiteralTypo
        Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] odinsHollow == null : {odinsHollow == null}");


        var globalSpawnPool = odinsHollow?.transform.Find("GlobalSpawnPool")?.gameObject?.GetComponent<TrapSpawnPool>();
        // var globalSpawnPool = odinsHollow?.transform.Find("GlobalSpawnPool");
        Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] globalSpawnPool == null : {globalSpawnPool == null}");

        // globalSpawnPool.FixReferences();

        globalSpawnPool?.m_spawnPoolPrefabs.Clear(); // Remove anything already in the GSP.
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.SkeletonPoison));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.Blob));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.BlobElite));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.Draugr));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrElite));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrRanged));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.SkeletonNoArcher));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.SkeletonNoArcher));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.SkeletonNoArcher));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.SkeletonNoArcher));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.BonePileSpawner));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(PrefabNames.SpawnerDraugrPile));

        SeedSpawnPoolsFor(new DungeonBossRoom(BlueRoom), odinsHollow);
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

    private void SeedSpawnPoolsFor(DungeonBossRoom room, GameObject odinsHollow)
    {
      Jotunn.Logger.LogDebug($"Seeding bosses for {room.Name}");
      var miniBossTrigger = odinsHollow?.transform.Find(room.Name)?.Find(room.MiniBossTrigger).gameObject?.GetComponent<TrapTrigger>();
      if (miniBossTrigger != null)
      {
        miniBossTrigger.m_useTriggerSpawnPool = false; // true | false
        miniBossTrigger.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrElite));
      }

      SeedSpawnPoolsFor(room as DungeonRoom, odinsHollow);
    }

    private void SeedSpawnPoolsFor(DungeonRoom room, GameObject odinsHollow)
    {
      Jotunn.Logger.LogDebug($"Seeding spawns for {room.Name}");
      var roomSpawnPool = odinsHollow?.transform.Find(room.Name)?.Find(room.RoomSpawnPool).gameObject?.GetComponent<TrapSpawnPool>();
      if (roomSpawnPool != null)
      {
        roomSpawnPool.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.Draugr));
        roomSpawnPool.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrRanged));
      }
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
        _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
