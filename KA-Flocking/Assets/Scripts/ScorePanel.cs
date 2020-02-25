using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public Text infantryNoText;
    public string teamFlockName;
    private Flock flock;
    public NextScene next;

    void Start() {
        flock = GameObject.Find(teamFlockName).GetComponent<Flock>();
    }

    // Update is called once per frame
    void Update()
    {
        infantryNoText.text = flock.agents.Count.ToString();
        if(flock.agents.Count == 0)
        {
            next.nextScene(); //go to end screen if all soldiers in one of the teams are dead
        }
    }
}
