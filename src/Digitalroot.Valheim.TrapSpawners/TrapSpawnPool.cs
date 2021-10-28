using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Traps/Spawn Pool", 32)]
public class TrapSpawnPool : MonoBehaviour
{
  [SerializeField, Tooltip("Collection of all the prefabs that can spawn")]
  public List<GameObject> m_spawnPoolPrefabs = new List<GameObject>(0);

}
