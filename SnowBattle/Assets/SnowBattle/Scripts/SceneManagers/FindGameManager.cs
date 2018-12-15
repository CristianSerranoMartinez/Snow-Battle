using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class FindGameManager : MonoBehaviour {

    [SerializeField]
    private GameObject selfCanvas;
    [SerializeField]
    private GameObject menuCanvas;

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ButtonCancel();
            }
        }
    }

    public void ButtonCancel()
    {
        menuCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        //SceneManager.LoadScene("Menu");
    }

    void OnApplicationQuit()
    {
        VariablesRealTime.auth.SignOut();
    }
}
