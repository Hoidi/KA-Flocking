//Here the text can be changed. Later on this script will be used to fetch information
//from the game. 

using UnityEngine;
using UnityEngine.UI;

public class winningText : MonoBehaviour
{
    public Text winText;
    public Text team1kills;
    public Text team2kills;
    Flock flock;
    Flock flock2;

    // Update is called once per frame
    void Start(){
        flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
        flock2 = GameObject.Find("Team 2 Flock").GetComponent<Flock>();
        winText.text = ScorePanel.winningText;
        team1kills.text += flock2.deadUnits.Count.ToString();
        team2kills.text += flock.deadUnits.Count.ToString();
    }
}
