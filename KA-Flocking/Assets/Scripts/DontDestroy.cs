using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static List<FlockAgent> agents;
    public static List<FlockAgent> agents2;
    void Start()
    {
        DontDestroy.agents = GameObject.Find("Team 1 Flock").GetComponent<List<FlockAgent>>();
        DontDestroy.agents2 = GameObject.Find("Team 2 Flock").GetComponent<List<FlockAgent>>();
        //Debug.Log(agents2.Count);
        //DontDestroyOnLoad(this.gameObject);
    }
}
