//This script is controlling the button.
//Later on this script will fetch the scene that starts a new game.

using UnityEngine;
using UnityEngine.UI;

public class restartGame : MonoBehaviour
{
	public Text text;
	public Button yourButton;

	void Start()
	{
		//Button btn = yourButton.GetComponent<Button>();
		yourButton.onClick.AddListener(ResetGame);
	}

	void ResetGame()
	{
		//Here should the scene for the game be loaded!
		text.text = ("Play again!");
	}
}

