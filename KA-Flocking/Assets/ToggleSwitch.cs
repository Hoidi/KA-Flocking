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

    public Toggle InfantryToggle;
    public Toggle PikeToggle;
    public Toggle ArcherToggle;
    public Toggle ScoutToggle;
    public GameObject InfantryToggleOn;
    public GameObject InfantryToggleOff;
    public GameObject PikeToggleOn;
    public GameObject PikeToggleOff;
    public GameObject ArcherToggleOn;
    public GameObject ArcherToggleOff;
    public GameObject ScoutToggleOn;
    public GameObject ScoutToggleOff;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Toggle buttons for formation type
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
        //Toggle buttons for troop type
        if (InfantryToggle.isOn)
        {
            InfantryToggleOn.SetActive(true);
            InfantryToggleOff.SetActive(false);
        }
        else if (!InfantryToggle.isOn)
        {
            InfantryToggleOn.SetActive(false);
            InfantryToggleOff.SetActive(true);
        }

        if (PikeToggle.isOn)
        {
            PikeToggleOn.SetActive(true);
            PikeToggleOff.SetActive(false);
        }
        else if (!PikeToggle.isOn)
        {
            PikeToggleOn.SetActive(false);
            PikeToggleOff.SetActive(true);
        }

        if (ArcherToggle.isOn)
        {
            ArcherToggleOn.SetActive(true);
            ArcherToggleOff.SetActive(false);
        }
        else if (!ArcherToggle.isOn)
        {
            ArcherToggleOn.SetActive(false);
            ArcherToggleOff.SetActive(true);
        }

        if (ScoutToggle.isOn)
        {
            ScoutToggleOn.SetActive(true);
            ScoutToggleOff.SetActive(false);
        }
        else if (!ScoutToggle.isOn)
        {
            ScoutToggleOn.SetActive(false);
            ScoutToggleOff.SetActive(true);
        }


    }
}
