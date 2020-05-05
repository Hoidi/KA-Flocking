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
    private int[] troopDead;
    private int totalAlive;
    private int totalDead;

    void Start() {
        flock = GameObject.Find(teamFlockName).GetComponent<Flock>();
        troopAlive = new int[troopTypes.Length];
        troopDead = new int[troopTypes.Length];
        countAlive();
        countDead();
    }

    // Update is called once per frame
    void Update()
    {
        countAlive();
        countDead();

        for (int i = 0; i < troopTypes.Length; i++)
        {
            // Lose if the amount of castles is 0 or if there's only castles alive
            if (troopTypes[i] == "Castle" && 
                (troopAlive[i] == 0 || totalAlive <= troopAlive[i])) {
                Lose();
            }
        }
    }

    private void Lose() {
        if (flock.name == "Team 1 Flock") winningText = "Red team won!";
        else winningText = "Blue team won!";
        next.nextScene(); //go to end screen if all soldiers in one of the teams are dead
    }

    private void countAlive() {
        // If there is still an equal amount of alive units, don't count
        if (flock.agents.Count == totalAlive) return;
        totalAlive = flock.agents.Count;
        troopAlive = new int[troopTypes.Length];
        foreach (FlockAgent agent in flock.agents)
        {
            for (int i = 0; i < troopTypes.Length; i++)
            {
                if (agent.unit.name == troopTypes[i]+"(Clone)") {
                    troopAlive[i] += 1;
                    break;
                }
            }
        }
        for (int i = 0; i < troopTypes.Length; i++)
        {
            troopNoTexts[i].text = troopAlive[i].ToString();;
        }
    }

    private void countDead() {
        // If there is still an equal amount of dead units, don't count
        if (flock.deadUnits.Count == totalDead) return;
        totalDead = flock.deadUnits.Count;
        troopDead = new int[troopTypes.Length];
        foreach (FlockAgent agent in flock.deadUnits)
        {
            for (int i = 0; i < troopTypes.Length; i++)
            {
                if (agent.unit.name == troopTypes[i]+"(Clone)") {
                    troopDead[i]++;
                    break;
                }
            }
        }
        for (int i = 0; i < troopTypes.Length; i++)
        {
            troopNoDeadTexts[i].text = troopDead[i].ToString();;
        }
    }
}
