using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);

    private JoinRoomDelegate joinRoomCallBack;

    [SerializeField]
    private Text roomNameText;

    private  MatchInfoSnapshot match;

    private AudioSource clickButtonSound;

    private void Start()
    {
        Debug.Log("RoomListItem Start");

        clickButtonSound = GetComponent<AudioSource>();
    }

    public void Setup(MatchInfoSnapshot _match,JoinRoomDelegate _joinRoomCallBack)
    {
        Debug.Log("Setup");

        match = _match;

        joinRoomCallBack = _joinRoomCallBack;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinMatch()
    {
        Debug.Log("JoinMatch");

        clickButtonSound.Play();

        joinRoomCallBack.Invoke(match);
    }
}
