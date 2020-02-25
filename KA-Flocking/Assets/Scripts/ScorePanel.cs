using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public Text infantryNoText;
    public string teamFlockName;
    private Flock flock;

    void Start() {
        flock = GameObject.Find(teamFlockName).GetComponent<Flock>();
    }

    // Update is called once per frame
    void Update()
    {
        infantryNoText.text = flock.agents.Count.ToString();
    }
}
