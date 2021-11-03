using Digitalroot.Valheim.TrapSpawners;
using UnityEngine;

namespace Digitalroot.Valheim.Dungeons.OdinsHollow
{
  public class DungeonBossRoom : DungeonRoom
  {
    private const string MiniBossTriggerName = "MiniBoss_Trigger";
    private const string MiniBossSpawnPointName = "MiniBoss_SpawnPoint";

    public DungeonBossRoom(string name, GameObject dungeon)
      : base(name, dungeon)
    {
    }

    public TrapSpawner MiniBossSpawnPoint => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{MiniBossSpawnPointName}")?.gameObject.GetComponent<TrapSpawner>();
    public TrapTrigger MiniBossTrigger => Dungeon.transform.Find($"Interior/Dungeon/Rooms/{Name}/Traps/{MiniBossTriggerName}")?.gameObject.GetComponent<TrapTrigger>();
  }
}
