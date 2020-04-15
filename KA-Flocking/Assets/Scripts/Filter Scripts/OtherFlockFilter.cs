using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Other Flock")]
public class OtherFlockFilter : ContextFilter
{
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null && itemAgent.GetAgentFlock() != agent.GetAgentFlock())
            {
                filtered.Add(itemAgent.transform);
            }
        }
        return filtered;
    }
}
