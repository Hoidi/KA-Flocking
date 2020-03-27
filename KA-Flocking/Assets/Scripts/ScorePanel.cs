using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public string teamFlockName;
    private Flock flock;
    public NextScene next;
    public static string winningText;
    public Text[] troopNoTexts = new Text[4];
    public Text[] troopNoDeadTexts = new Text[4];
    public string[] troopTypes = new string[4];
    private int[] troopAlive;

    void Start() {
        flock = GameObject.Find(teamFlockName).GetComponent<Flock>();
        troopAlive = new int[troopTypes.Length];
        countAlive();
    }

    // Update is called once per frame
    void Update()
    {
 //       flock.deadUnits.Add += new EventHandler(countAlive);

        if(flock.agents.Count == 0)
        {
            if (flock.name == "Team 1 Flock") winningText = "Team 2 won!";
            else winningText = "Team 1 won!";
            next.nextScene(); //go to end screen if all soldiers in one of the teams are dead
        }
    }

    private void countAlive() {
        foreach (FlockAgent agent in flock.agents)
        {
            for (int i = 0; i < troopTypes.Length; i++)
            {
                if (agent.unit.GetType().ToString() == troopTypes[i]) {
                    troopAlive[i] += 1;
                }
            }
        }
    }
}
