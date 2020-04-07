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
    public Toggle CastleToggle;
    public GameObject InfantryToggleOn;
    public GameObject InfantryToggleOff;
    public GameObject PikeToggleOn;
    public GameObject PikeToggleOff;
    public GameObject ArcherToggleOn;
    public GameObject ArcherToggleOff;
    public GameObject ScoutToggleOn;
    public GameObject ScoutToggleOff;
    public GameObject CastleToggleOn;
    public GameObject CastleToggleOff;

    // Update is called once per frame
    void Update()
    {
        //Togglebuttons for formations
        CircularToggleOn.SetActive(CircularToggle.isOn);
        CircularToggleOff.SetActive(!CircularToggle.isOn);

        RectangularToggleOn.SetActive(RectangularToggle.isOn);
        RectangularToggleOff.SetActive(!RectangularToggle.isOn);

        ArrowToggleOn.SetActive(ArrowToggle.isOn);
        ArrowToggleOff.SetActive(!ArrowToggle.isOn);

        //Togglebuttons for troop type
        InfantryToggleOn.SetActive(InfantryToggle.isOn);
        InfantryToggleOff.SetActive(!InfantryToggle.isOn);

        PikeToggleOn.SetActive(PikeToggle.isOn);
        PikeToggleOff.SetActive(!PikeToggle.isOn);

        ArcherToggleOn.SetActive(ArcherToggle.isOn);
        ArcherToggleOff.SetActive(!ArcherToggle.isOn);

        ScoutToggleOn.SetActive(ScoutToggle.isOn);
        ScoutToggleOff.SetActive(!ScoutToggle.isOn);

        CastleToggleOn.SetActive(CastleToggle.isOn);
        CastleToggleOff.SetActive(!CastleToggle.isOn);
    }
    //Sword: https://thenounproject.com/search/?q=sword&i=2444713, ProSymbols US
    //Spear: https://thenounproject.com/search/?q=spear&i=819835, Hamish 
    //Bow: https://thenounproject.com/search/?q=bow&i=2851797 NicklasR, AT
    //Flag: https://thenounproject.com/search/?q=flag&i=714884 Maxim Kulikov
}
