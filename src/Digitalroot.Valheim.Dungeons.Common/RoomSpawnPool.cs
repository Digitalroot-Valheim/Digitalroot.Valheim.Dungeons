using Digitalroot.Valheim.TrapSpawners;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.Common
{
  public class RoomSpawnPool : SpawnPool
  {
    private const string RoomSpawnPoolName = "Room_SpawnPool";

    /// <inheritdoc />
    public RoomSpawnPool(GameObject dungeon, string roomName)
    {
      _spawnPool = dungeon?.transform.Find($"Interior/Dungeon/Rooms/{roomName}/Traps/{RoomSpawnPoolName}")?.gameObject?.GetComponent<TrapSpawnPool>();
    }
  }
}
