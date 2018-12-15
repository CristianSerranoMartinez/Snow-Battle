using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class CanvasManagerNetwork : NetworkBehaviour
{
    [SyncVar]
    private bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }

        set { isDead = value; }
    }

    [SyncVar]
    private int currentHealth = 100;

    [SerializeField]
    GameObject onPlayPanel;
    [SerializeField]
    GameObject onPausePanel;
    [SerializeField]
    Slider slider;
    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    private bool[] wasEnabled;

    private GameObject canvas;

    private NetworkManager networkManager;

    private bool firstSetup = true;

    private void Start()
    {
        canvas = GetComponent<PlayerSetupNetwork>().GetCanvas();

        networkManager = NetworkManager.singleton;

        FindChilds(canvas);

        if (isLocalPlayer)
            SetupPlayer();
    }


    public void SetupPlayer()
    {
        CmdBroadCastNewPlayerSetup();
    }


    void FindChilds(GameObject obj)
    {
        if (obj.name == "ButtonA")
        {
            //set up event button down for butonA shoot 
            EventTrigger trigger1 = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            entry1.eventID = EventTriggerType.PointerDown;
            entry1.callback.AddListener((eventData) => { GetComponent<PlayerShootingNetwork>().OnPointerDown(); });
            trigger1.triggers.Add(entry1);

            //set up event button up for butonA to shoot
            EventTrigger trigger2 = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerUp;
            entry2.callback.AddListener((eventData) => { GetComponent<PlayerShootingNetwork>().OnPointerUp(); });
            trigger2.triggers.Add(entry2);
        }

        if (obj.name == "Fixed Joystick Left") { GetComponent<PlayerMovementNetwork>().joystickleft = obj.GetComponent<FixedJoystick>(); }
        if (obj.name == "Fixed Joystick Right") { GetComponent<PlayerMovementNetwork>().joystickright = obj.GetComponent<FixedJoystick>(); }
        //if (obj.name == "LoadingScreen") { loadingPanel = obj; }
        if (obj.name == "OnPause") { onPausePanel = obj; }
        if (obj.name == "OnPlay") { onPlayPanel = obj; }
        if (obj.name == "Health") { slider = obj.GetComponent<Slider>(); }
        if (obj.name == "QuitButton") { obj.GetComponent<Button>().onClick.AddListener(delegate { OnPressQuitButton(); }); }
        if (obj.name == "ResumeButton") { obj.GetComponent<Button>().onClick.AddListener(delegate { OnPlay(); }); }
        if (obj.name == "ButtonStart") { obj.GetComponent<Button>().onClick.AddListener(delegate { Onpause(); }); }

        foreach (Transform t in obj.transform)
        {
            FindChilds(t.gameObject);
        }
    }


    public void Onpause()
    {
        onPausePanel.SetActive(true);
        onPlayPanel.SetActive(false);
    }
    public void OnPlay()
    {
        onPlayPanel.SetActive(true);
        onPausePanel.SetActive(false);
    }

    public void OnPressQuitButton()
    {
        MatchInfo matchInfo = networkManager.matchInfo;

        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);

        networkManager.StopHost();
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, string sourcePlayer)
    {
        if (currentHealth > 0)
        {
            currentHealth = currentHealth - damage;

            slider.value = currentHealth;

            if (currentHealth <= 0)
            {
                Die(sourcePlayer);
            }
        }
    }


    [ClientRpc]
    public void RpcGiveLife(int life)
    {
        Debug.Log("RpcGiveLife");

        if (currentHealth < 100)
        {
            currentHealth = currentHealth + life;

            slider.value = currentHealth;      
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        PlayerSetupNetwork sourcePlayer = GameManager.GetPlayer(_sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;

            Debug.Log("xx:" + sourcePlayer.username);

            GameManager.instance.onPlayerKilledCallback.Invoke(GetComponent<PlayerSetupNetwork>().username, "Kill", sourcePlayer.username);
        }

        GetComponent<PlayerSetupNetwork>().deaths++;

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //SeUIMode
        Onpause();

        //Disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.isKinematic = true;


        /* Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
         rigidbody.position = _spawnPoint.position;
         rigidbody.rotation = _spawnPoint.rotation;*/

        //set up position respawn acording to tag
        if (tag == "Cat")
        {
            GetComponent<Rigidbody>().MovePosition(GameObject.Find("Cat").transform.position);
            GetComponent<Rigidbody>().MoveRotation(GameObject.Find("Cat").transform.rotation);
        }
        //set up position respawn acording to tag
        if (tag == "Bear")
        {
            GetComponent<Rigidbody>().MovePosition(GameObject.Find("Bear").transform.position);
            GetComponent<Rigidbody>().MoveRotation(GameObject.Find("Bear").transform.rotation);
        }

        //Spawn a death effect
        /*GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);*/

        //Switch cameras
        /*if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }*/

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }


    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }


    public void SetDefaults()
    {
        isDead = false;
        currentHealth = 100;
        slider.value = currentHealth;


        //Enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        OnPlay();

        //Enable the gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.isKinematic = false;

        //Create spawn effect
        /*GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);*/
    }

    private void DieOnFall()
    {

    }
}