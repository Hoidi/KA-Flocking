using UnityEngine;
using UnityEngine.UI;

public class BluePanel : MonoBehaviour
{
    public Text infantryNoText;
    public Flock blueFlock;
    private int infantryNo;

    // Update is called once per frame
    void Update()
    {
        infantryNoText.text = blueFlock.agents.Count.ToString();
        Debug.Log(blueFlock.agents.Count.ToString());
    }
}