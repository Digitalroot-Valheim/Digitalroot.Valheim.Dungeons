using BepInEx;
using Digitalroot.CustomMonoBehaviours;
using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Names.Vanilla;
using Digitalroot.Valheim.TrapSpawners;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Digitalroot.Valheim.DungeonsDemo
{
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
        Log.Trace(Main.Instance, $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}] globalSpawnPool == null : {globalSpawnPool == null}");

        globalSpawnPool?.m_spawnPoolPrefabs.Clear(); // Remove anything already in the GSP.
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.SkeletonPoison));
        globalSpawnPool?.m_spawnPoolPrefabs.Add(PrefabManager.Cache.GetPrefab<GameObject>(EnemyNames.Blob));
        
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

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
