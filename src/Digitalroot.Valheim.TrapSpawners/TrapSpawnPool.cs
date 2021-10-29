using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once IdentifierTypo
namespace Digitalroot.Valheim.TrapSpawners
{
  [AddComponentMenu("Traps/Spawn Pool", 32)]
  // ReSharper disable once ClassNeverInstantiated.Global
  public class TrapSpawnPool : MonoBehaviour
  {
    [SerializeField, Tooltip("Collection of all the prefabs that can spawn")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(0);
  }
}
