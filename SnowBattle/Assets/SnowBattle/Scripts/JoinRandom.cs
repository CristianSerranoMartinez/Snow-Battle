using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JoinRandom : MonoBehaviour
{
    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private GameObject panelLoading;

    [SerializeField]
    private GameObject imageLoading;

    [SerializeField]
    private GameObject buttonOK;

    private NetworkManager networkManager;

    List<GameObject> roomList = new List<GameObject>();

    private bool ok;


    void Start()
    {
        networkManager = NetworkManager.singleton;

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }
    private void Update()
    {
        if (ok)
        {
            imageLoading.transform.Rotate(0, 0, 5);
        }
    }
    public void RefreshRoomList()
    {
        ClearRoomList();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (!success || matchList == null)
        {

            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();

            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            roomList.Add(_roomListItemGO);
        }

        if (roomList.Count == 0)
        {

            buttonOK.SetActive(true);

            imageLoading.SetActive(false);
        }
        else
        {
            buttonOK.SetActive(false);

            imageLoading.SetActive(true);
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);

        StartCoroutine(WaitForJoin());
    }

    IEnumerator WaitForJoin()
    {
        ClearRoomList();

        int countdown = 10;

        while (countdown > 0)
        {

            yield return new WaitForSeconds(1);

            countdown--;
        }

        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;

        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);

            networkManager.StopHost();
        }

        RefreshRoomList();
    }


    public void JoinRandomRoom()
    {

        panelLoading.SetActive(true);

        ok = true;

        if (roomList.Count > 0)
        {
            int i = Random.Range(0, roomList.Count);

            roomList[i].GetComponent<RoomListItem>().JoinMatch();
        }
    }

    public void ButtonOK()
    {
        panelLoading.SetActive(false);
    }
}
