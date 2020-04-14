using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ErrorChat : MonoBehaviour
{
    public GameObject board;
    private Text[] errorTexts;
    public float fadeSpeed = 0.3f;

    void Start()
    {
        errorTexts = board.GetComponentsInChildren<Text>();
    }

    // Fades text
    void Update() 
    {
        foreach (Text text in errorTexts)
        {
            text.color = Color.Lerp(text.color, new Color(1.0f,0,0,0), Time.unscaledDeltaTime*fadeSpeed/text.color.a);
        }
    }

    // Prints the error to the board and reorders the board
    public void ShowError(string message) {
        for (int i = errorTexts.Length-2; i >= 0; i--)
        {
            swapProperties(errorTexts[i], errorTexts[i+1]);
        }
        errorTexts[0].text = message;
        errorTexts[0].color = new Color (1.0f,0,0,1.0f);
    }

    // Replaces the text and color of 'to' to that of 'from'
    private void swapProperties(Text from, Text to) {
        to.text = from.text;
        to.color = from.color;
    }

}
