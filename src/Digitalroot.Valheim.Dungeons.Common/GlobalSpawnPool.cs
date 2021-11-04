using Digitalroot.Valheim.TrapSpawners;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class GlobalSpawnPool : SpawnPool
  {
    private const string GlobalSpawnPoolName = "GlobalSpawnPool";

    /// <inheritdoc />
    public GlobalSpawnPool(GameObject dungeon)
    {
      _spawnPool = dungeon?.transform.Find(GlobalSpawnPoolName)?.gameObject?.GetComponent<TrapSpawnPool>();
    }

    public TrapSpawnPool SpawnPool => _spawnPool;
  }
}
