using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class SelectCharacterManager : MonoBehaviour {
    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private GameObject selfCanvas;
    [SerializeField]
    private GameObject selectMapCanvas;
    [SerializeField]
    private GameObject firstCharacter;

    private Image choosedCharacter;

    NetworkManager networkManager;

    /*public void Start()
    {
        networkManager = NetworkManager.singleton;

        OnChooseCharacter(firstCharacter);
    }*/

    private void OnEnable()
    {
        networkManager = NetworkManager.singleton;

        OnChooseCharacter(firstCharacter);
    }

    public void OnChooseCharacter(GameObject obj)
    {
        if (choosedCharacter != null)
        {
            choosedCharacter.enabled = false;
        }

        choosedCharacter = obj.GetComponent<Image>();

        choosedCharacter.enabled = true;

        switch (obj.name)
        {
            case "NameCharacter1": { VariablesRealTime.team = "Bear"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 0; } break;
            case "NameCharacter2": { VariablesRealTime.team = "Cat"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 1; } break;
            case "NameCharacter3": { VariablesRealTime.team = "Bear"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 0; } break;
            case "NameCharacter4": { VariablesRealTime.team = "Cat"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 1; } break;
            case "NameCharacter5": { VariablesRealTime.team = "Bear"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 0; } break;
            case "NameCharacter6": { VariablesRealTime.team = "Cat"; networkManager.GetComponent<CustomNetworkManager>().currentplayer = 1; } break;
        }
    }

    void OnApplicationQuit()
    {
        VariablesRealTime.auth.SignOut();
    }

    public void ButtonLeave()
    {
        //SceneManager.LoadScene("Menu");
        menuCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        selectMapCanvas.SetActive(false);
    }

    public void ButtonReady()
    {
        //SceneManager.LoadScene("SelectMap");
        selectMapCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        menuCanvas.SetActive(false);
    }
}
