using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.CMB
{
  internal class BossAura : MonoBehaviour
  {
    private float _update;

    void Awake()
    {
      Debug.Log("Awake");
      _update = 0.0f;
    }

    void Update()
    {
      _update += Time.deltaTime;
      if (_update > 3.0f)
      {
        _update = 0.0f;
        Instantiate(ZNetScene.instance.GetPrefab("shaman_heal_aoe".GetStableHashCode()), gameObject.transform.localPosition, Quaternion.identity, gameObject.transform);
      }
    }
  }
}
