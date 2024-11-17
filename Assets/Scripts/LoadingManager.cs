using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(1);
        }
    }
}
