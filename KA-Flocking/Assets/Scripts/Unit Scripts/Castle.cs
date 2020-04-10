using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Castle")]
public class Castle : Infantry
{
    [System.NonSerialized]
    // Cost, Prefab, UnitType, Amount, Sprite
    public List<(int, FlockAgent, Unit, int, Sprite)> items = new List<(int, FlockAgent, Unit, int, Sprite)>();
    [Range(0,30f)]
    public float spawnTime = 10f;
    public bool spawning = false;

    public IEnumerator SpawningRoutine(FlockAgent agent, Flock flock) {
        spawning = true;
        while (agent.isActiveAndEnabled) {
            yield return new WaitForSeconds(spawnTime);
            (FlockAgent, Unit) troop = spawnTroop();
            if (troop.Item1 != null) {
                // The position should be in front of the castle
                Vector3 pos = agent.transform.position + agent.transform.forward * 7;
                // Double check the y vector
                Physics.Raycast(new Vector3(pos.x, 100, pos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity);
                pos.y = hit.point.y;
                
                flock.CreateUnit(
                    troop.Item1,
                    pos,
                    agent.transform.rotation,
                    troop.Item2
                    );
            }
        }
    }

    private (FlockAgent,Unit) spawnTroop() {
        if (items.Count == 0) return (null,null);
        var v = items[0];
        if (v.Item4 > 1) {
            v.Item4--;
            items[0] = v;
            return (v.Item2, v.Item3);
        } else {
            items.RemoveAt(0);
            return (v.Item2, v.Item3);
        }
    }
}
