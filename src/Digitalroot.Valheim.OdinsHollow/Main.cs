using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        SeedGlobalSpawnPoolIfNecessary(odinsHollow);

        SeedSpawnPoolsFor(new DungeonBossRoom(DungeonsRoomNames.BlueRoom, odinsHollow));
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

    #region Spawn Pool Seeding

    private void SeedGlobalSpawnPoolIfNecessary(GameObject odinsHollow)
    {
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Seeding Global Spawn Pool");
      // var globalSpawnPool = odinsHollow?.transform.Find("GlobalSpawnPool")?.gameObject?.GetComponent<TrapSpawnPool>();
      var globalSpawnPool = new GlobalSpawnPool(odinsHollow);
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] globalSpawnPool.SpawnPool == null : {globalSpawnPool.SpawnPool == null}");

      if (!globalSpawnPool.SpawnPool)
      {
        Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] Skipping Seeding of Global Spawn Pool");
        return;
      }

      // globalSpawnPool.FixReferences(); // JVL Mocks Broken

      globalSpawnPool.Clear(); // Remove anything already in the GSP.
      globalSpawnPool.AddEnemy(EnemyNames.SkeletonPoison);
      globalSpawnPool.AddEnemy(EnemyNames.Blob);
      globalSpawnPool.AddEnemy(EnemyNames.BlobElite);
      globalSpawnPool.AddEnemy(EnemyNames.Draugr);
      globalSpawnPool.AddEnemy(EnemyNames.DraugrElite);
      globalSpawnPool.AddEnemy(EnemyNames.DraugrRanged);
      globalSpawnPool.AddEnemy(PrefabNames.SkeletonNoArcher);
      globalSpawnPool.AddEnemy(PrefabNames.SkeletonNoArcher);
      globalSpawnPool.AddEnemy(PrefabNames.SkeletonNoArcher);
      globalSpawnPool.AddEnemy(PrefabNames.SkeletonNoArcher);
      globalSpawnPool.AddPrefab(PrefabNames.BonePileSpawner);
      globalSpawnPool.AddPrefab(PrefabNames.SpawnerDraugrPile);
    }

    private void SeedSpawnPoolsFor(DungeonBossRoom room)
    {
      Log.Trace(Main.Instance, $"Seeding bosses for {room.Name}");
      Log.Trace(Main.Instance, $"Room Health Check [{room.Name}]");
      Log.Trace(Main.Instance, $"room.MiniBossSpawnPoint == null [{room.MiniBossSpawnPoint == null}]");
      // ReSharper disable once IdentifierTypo
      // var miniBossSpawner = odinsHollow?.transform.Find(room.Name)?.Find(room.MiniBossSpawnPointName).gameObject?.GetComponent<TrapSpawner>();
      // Log.Trace(Main.Instance, $"miniBossSpawner == null : {miniBossSpawner == null}");
      if (room.MiniBossSpawnPoint != null)
      {
        room.MiniBossSpawnPoint.m_ignoreSpawnPoolOverrides = true; // true | false
        room.MiniBossSpawnPoint.m_spawnPoolPrefabs.Add(ConfigureAsBoss(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrElite)));
      }

      SeedSpawnPoolsFor(room as DungeonRoom);
    }

    private void SeedSpawnPoolsFor(DungeonRoom room)
    {
      
      Log.Trace(Main.Instance, $"Seeding trash for {room.Name}");
      Log.Trace(Main.Instance, $"Room Health Check [{room.Name}]");
      Log.Trace(Main.Instance, $"room.RoomSpawnPool == null : {room.RoomSpawnPool == null}");
      Log.Trace(Main.Instance, $"room.RoomSpawnPool?.m_spawnPoolPrefabs == null : {room.RoomSpawnPool?.m_spawnPoolPrefabs == null}");
      Log.Trace(Main.Instance, $"room.RoomSpawnPool?.m_spawnPoolPrefabs?.Count : {room.RoomSpawnPool?.m_spawnPoolPrefabs?.Count}");
      Log.Trace(Main.Instance, $"room.RoomTrigger == null : {room.RoomTrigger == null}");
      Log.Trace(Main.Instance, $"room.RoomSpawnPoints == null : {room.RoomSpawnPoints == null}");
      Log.Trace(Main.Instance, $"room.RoomSpawnPoints?.Count : {room.RoomSpawnPoints?.Count}");

      //
      //
      // var roomSpawnPool = odinsHollow?.transform.Find(room.Name)?.Find(room.RoomSpawnPoolName).gameObject?.GetComponent<TrapSpawnPool>();
      // Log.Trace(Main.Instance, $"roomSpawnPool == null : {roomSpawnPool == null}");
      if (room.RoomSpawnPool != null)
      {
        room.RoomSpawnPool?.m_spawnPoolPrefabs?.Add(ConfigureAsTrash(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.Draugr)));
        room.RoomSpawnPool?.m_spawnPoolPrefabs?.Add(ConfigureAsTrash(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.DraugrRanged)));
      }
    }

    
    private GameObject ConfigureAsBoss(GameObject prefab)
    {
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      if (prefab == null) return null;
      prefab.transform.localScale *= 2;
      var character = prefab.GetComponent<Character>();
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(4); // Need to do this at spawn time. 
      return prefab;
    }

    private GameObject ConfigureAsTrash(GameObject prefab)
    {
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab == null : {prefab == null}");
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] prefab?.name : {prefab?.name}");
      if (prefab == null) return null;
      // prefab.transform.localScale *= 2;
      var character = prefab.GetComponent<Character>();
      Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] character == null : {character == null}");
      // character?.SetLevel(3); // Need to do this at spawn time. 
      return prefab;
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
