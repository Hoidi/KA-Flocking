using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    public Toggle CircularToggle;
    public Toggle RectangularToggle;
    public Toggle ArrowToggle;
    public GameObject CircularToggleOn;
    public GameObject CircularToggleOff;
    public GameObject RectangularToggleOn;
    public GameObject RectangularToggleOff;
    public GameObject ArrowToggleOn;
    public GameObject ArrowToggleOff;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CircularToggle.isOn)
        {
            CircularToggleOn.SetActive(true);
            CircularToggleOff.SetActive(false);
        }
        else if (!CircularToggle.isOn)
        {
            CircularToggleOn.SetActive(false);
            CircularToggleOff.SetActive(true);
        }

        if (RectangularToggle.isOn)
        {
            RectangularToggleOn.SetActive(true);
            RectangularToggleOff.SetActive(false);
        }
        else if (!RectangularToggle.isOn)
        {
            RectangularToggleOn.SetActive(false);
            RectangularToggleOff.SetActive(true);
        }

        if (ArrowToggle.isOn)
        {
            ArrowToggleOn.SetActive(true);
            ArrowToggleOff.SetActive(false);
        }
        else if (!ArrowToggle.isOn)
        {
            ArrowToggleOn.SetActive(false);
            ArrowToggleOff.SetActive(true);
        }
    }
}
