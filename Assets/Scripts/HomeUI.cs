using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour
{
  
    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Quit");
    }


}
