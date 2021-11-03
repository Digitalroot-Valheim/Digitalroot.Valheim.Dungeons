using Digitalroot.Valheim.TrapSpawners;
using System.Collections.Generic;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  public class DungeonRoom
  {
    public readonly string Name;

    protected readonly GameObject Dungeon;

    private const string RoomTriggerName = "Room_Trigger";

    private const string RoomSpawnPoolName = "Room_SpawnPool";

    // ReSharper disable once MemberCanBeProtected.Global
    public DungeonRoom(string name, GameObject dungeon)
    {
      Name = name;
      Dungeon = dungeon;
    }

    public TrapTrigger RoomTrigger => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{RoomTriggerName}")?.gameObject.GetComponent<TrapTrigger>();
    public TrapSpawnPool RoomSpawnPool => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{RoomSpawnPoolName}")?.gameObject.GetComponent<TrapSpawnPool>();
    public List<GameObject> RoomSpawnPoints => RoomTrigger.m_trapSpawners;
  }
}
