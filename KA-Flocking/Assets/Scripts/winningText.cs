//Here the text can be changed. Later on this script will be used to fetch information
//from the game. 

using UnityEngine;
using UnityEngine.UI;

public class winningText : MonoBehaviour
{
    public Text winText; 

    // Update is called once per frame
    void Start()
    {
        winText.text = ("Best team won!");
    }
}
