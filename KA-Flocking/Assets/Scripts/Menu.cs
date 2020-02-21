using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void buttonClick(){

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
