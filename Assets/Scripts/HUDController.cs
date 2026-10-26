using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{

    public void ExitToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
