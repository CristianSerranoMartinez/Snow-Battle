using UnityEngine;
using UnityEngine.Networking;


public class PlayerSetupNetwork : NetworkBehaviour {

    [SyncVar]
    public string username = "Loading...";

    [SyncVar]
    public int kills;

    [SyncVar]
    public int deaths;

    [SyncVar]
    public int connID;

    [SyncVar]
    public TeamSettings myTeam;

    [SerializeField]
    private GameObject camPrefab;

    [SerializeField]
    private GameObject canvasPrefab;

    private GameObject cam;

    private GameObject canvasInstance;

    private SphereCollider headcollider;


    void Start()
    {     
        canvasInstance = Instantiate(canvasPrefab);

        if (isLocalPlayer)
        {
            CmdSetUsername(transform.name, VariablesRealTime.nickName);
        }
        else
        {
            gameObject.tag = myTeam.name;

            canvasInstance.SetActive(false);
        }      
    }

    void OnDisable()
    {
        Destroy(canvasInstance);

        if (isLocalPlayer)
            Destroy(cam);

        GameManager.UnregisterPlayer(transform.name);
    }

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");

        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();

        PlayerSetupNetwork player = GetComponent<PlayerSetupNetwork>();

        GameManager.RegisterPlayer(netID, player);
    }

    //comannd function for to set up user name
    [Command]
    void CmdSetUsername(string playerID, string _username)
    {
        Debug.Log("CmdSetUsername");
        //Get the current player from the GamaManager on the network

        PlayerSetupNetwork player = GameManager.GetPlayer(playerID);

        if (player != null)
        {
            Debug.Log(_username + " has joined!");

            player.username = _username;
        }
    }

    //Function to get canvas isntatiated
    public GameObject GetCanvas()
    {
        return canvasInstance;
    }

    //Called on the local player by the NetworkManager
    [ClientRpc]
    public void RpcSetupPlayer(int connectionID, TeamSettings team)
    {
        Debug.Log("RpcSetupPlayer");
        //Call fuction for setup team, conneciton and tag
        SetupPlayer(connectionID, team);

        //Call fuction to spawn cam
        InstatiatePlayerCam();
    }

    public void SetupPlayer(int connectionID, TeamSettings team)
    {
        Debug.Log("Set up Player (team:"+team.name+", conn: "+connectionID+", tag: "+tag);

        myTeam = team;

        connID = connectionID;

        //gameObject.tag = myTeam.name;
    }

    //Instatiate the cam prefab for the local player accord to the tag selected
    public void InstatiatePlayerCam()
    {
        Debug.Log("InstatiatePlayerCam with tag: " + gameObject.tag);

        if (isLocalPlayer)
        {       
            if (myTeam.name == "Up")
            {
                cam = Instantiate(camPrefab, new Vector3(transform.position.x, transform.position.y + 12, transform.position.z + 18), transform.rotation);
            }

            if (myTeam.name == "Down")
            {
                cam = Instantiate(camPrefab, new Vector3(transform.position.x, transform.position.y + 12, transform.position.z - 18), transform.rotation);
            }

            cam.GetComponent<CameraFollow>().SetTarget(transform);

            Debug.Log("SetTarget with transform: " + transform.name);
        }
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer: Local player has been setup");
        Debug.Log("My conn ID is: " + connID);
    }
}
