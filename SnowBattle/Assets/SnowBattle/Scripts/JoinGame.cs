using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private GameObject panelCharacters;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private GameObject firstCharacter;

    [SerializeField]
    private GameObject panelCountDown;

    [SerializeField]
    private Transform roomListParent;

    [SerializeField]
    private Text textWarning;

    [SerializeField]
    private Text textCountDown;

    private NetworkManager networkManager;

    private Image choosedCharacter;

    MatchInfoSnapshot selectedMatch;

    void Start()
    {
        Debug.Log("JoinGame Start");

        networkManager = NetworkManager.singleton;

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }
    public void RefreshRoomList()
    {
        Debug.Log("RefreshRoomList");

        ClearRoomList();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);

        textWarning.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        Debug.Log("OnMatchList");

        textWarning.text = "";

        if (!success || matchList == null)
        {
            textWarning.text = "Couldn't get room list.";

            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);

            _roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();

            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, ReadyToSelectCharacter);
            }

            roomList.Add(_roomListItemGO);
        }

        if (roomList.Count == 0)
        {
            textWarning.text = "No rooms at the moment.";
        }
    }

    void ClearRoomList()
    {
        Debug.Log("ClearRoomList");

        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void ReadyToSelectCharacter(MatchInfoSnapshot _match)
    {
        Debug.Log("JoinRoom");

        selectedMatch = _match;

        panelCharacters.SetActive(true);

        OnChooseCharacter(firstCharacter);
    }

    public void ChangeRegion(Text Region)
    {
        Debug.Log("ChangeRegion");

        switch (Region.text)
        {
            case "EEUU":
                networkManager.SetMatchHost("us1-mm.unet.unity3d.com", networkManager.matchPort, true);
                return;

            case "EUROPE":
                networkManager.SetMatchHost("eu1-mm.unet.unity3d.com", networkManager.matchPort, true);
                return;

            case "SINGAPURE":
                networkManager.SetMatchHost("ap1-mm.unet.unity3d.com", networkManager.matchPort, true);
                return;

        }
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

    public void ButtonReady()
    {
        panelCountDown.SetActive(true);

        networkManager.matchMaker.JoinMatch(selectedMatch.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);

        StartCoroutine(WaitForJoin());
    }

    public void ButtonCancel()
    {
        panelCharacters.SetActive(false);

        selectedMatch = null;

        RefreshRoomList();
    }

    IEnumerator WaitForJoin()
    {
        Debug.Log("WaitForJoin");

        ClearRoomList();

        int countdown = 10;

        while (countdown > 0)
        {
            textCountDown.text = ""+ countdown;

            yield return new WaitForSeconds(1);

            countdown--;
        }

        selectedMatch = null;

        textWarning.text = "Failed to connect.";

        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;

        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);

            networkManager.StopHost();
        }

        panelCountDown.SetActive(false);

        panelCharacters.SetActive(false);

        RefreshRoomList();
    }
}
