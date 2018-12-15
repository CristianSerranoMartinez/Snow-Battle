using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMapManager : MonoBehaviour {

    [SerializeField]
    private Text textNameMap;
    [SerializeField]
    private Text textWarning;
    [SerializeField]
    private GameObject firstMap;
    [SerializeField]
    private GameObject panelLoading;
    [SerializeField]
    private GameObject imageLoading;
    [SerializeField]
    private GameObject selfCanvas;
    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private GameObject selectCharacterCanvas;

    private NetworkManager networkManager;
    private Image choosedMap;
    private bool ok;

    /*public void Start()
    {
        networkManager = NetworkManager.singleton;
        OnChooseMap(firstMap);
        networkManager.onlineScene = firstMap.name;
    }*/

    private void OnEnable()
    {
        networkManager = NetworkManager.singleton;
        OnChooseMap(firstMap);
        networkManager.onlineScene = firstMap.name;
    }

    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ButtonCancel();
            }
        }
        if (ok)
        {
            imageLoading.transform.Rotate(0, 0, 5);
        }
    }
    public void OnChooseMap(GameObject obj)
    {
        if (choosedMap != null)
        {
            choosedMap.enabled = false;
        }

        choosedMap = obj.GetComponent<Image>();

        choosedMap.enabled = true;

        if (textNameMap == null) { Debug.Log("textMap"); }
        if (networkManager == null) { Debug.Log("networkManager"); }
        if (firstMap == null) { Debug.Log("firstMap"); }
        if (obj == null) { Debug.Log("obj"); }

        switch (obj.name)
        {
            case "Map1": { VariablesRealTime.map = 1; networkManager.onlineScene = obj.name; } break;
            case "Map2": { VariablesRealTime.map = 2; networkManager.onlineScene = obj.name; } break;
            case "Map3": { VariablesRealTime.map = 3; networkManager.onlineScene = obj.name; } break;
            case "Map4": { VariablesRealTime.map = 4; networkManager.onlineScene = obj.name; } break;
            case "Map5": { VariablesRealTime.map = 5; networkManager.onlineScene = obj.name; } break;
            case "Map6": { VariablesRealTime.map = 6; networkManager.onlineScene = obj.name; } break;
        }
    }

    public void ButtonCancel()
    {
        //SceneManager.LoadScene("Menu");
        menuCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        selectCharacterCanvas.SetActive(false);
    }

    public void ButtonOK()
    {
        Debug.Log(VariablesRealTime.nickName);
        if (VariablesRealTime.nickName != "" && VariablesRealTime.nickName != null)
        {
            Debug.Log("Creating Room: " + VariablesRealTime.nickName + " with room for " + 6 + " players.");
            textWarning.text = "Loading...";
            panelLoading.SetActive(true);
            ok = true;
            networkManager.matchMaker.CreateMatch(VariablesRealTime.nickName, 6, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
    void OnApplicationQuit()
    {
        VariablesRealTime.auth.SignOut();
    }
}
