using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    // Start is called before the first frame update
    public Text infantryNoText;
    public Flock flock;

    // Update is called once per frame
    void Update()
    {
        infantryNoText.text = DontDestroy.agents.Count.ToString();
    }
}
