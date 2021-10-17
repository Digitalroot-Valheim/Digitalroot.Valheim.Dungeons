using BepInEx;
using Digitalroot.Valheim.Common;
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
    public const string Guid = "digitalroot.mods.DungeonsDemo";
    public const string Namespace = "Digitalroot.Valheim." + nameof(DungeonsDemo);
    private Harmony _harmony;
    private AssetBundle _assetBundle;
    public static Main Instance;

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

        // foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        // {
        //   Log.Trace(Main.Instance, $"{go.scene.path}:{go.scene.name}:{go.name}");
        // }

#endif
        PrefabManager.Instance.AddPrefab(new CustomPrefab(_assetBundle.LoadAsset<GameObject>("OdinsHollow"), false));

        _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
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
