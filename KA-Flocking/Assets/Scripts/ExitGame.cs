using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitGame : MonoBehaviour
{
    public void exit(){
        Application.Quit();
        Debug.Log("quit game");
    }
}
