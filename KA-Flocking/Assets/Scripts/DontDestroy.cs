using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static Flock flock1;
    public static Flock flock2;
    void Start()
    {
        if (flock1 == null) {
            flock1 = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
        } else if (flock2 == null) {
            flock2 = GameObject.Find("Team 2 Flock").GetComponent<Flock>();
        }
        //Debug.Log(agents2.Count);
        DontDestroyOnLoad(this.gameObject);
    }
}
