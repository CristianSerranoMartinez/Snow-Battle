using UnityEngine.Networking;
using UnityEngine;

public class AcornNetwork : NetworkBehaviour {

    [SyncVar]
    bool active;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("OnTriggerEnter");
            Eat(other.name);
        }
    }

    [Client]
    public void Eat(string other)
    {
        Debug.Log("Eat");

        CmdPlayerShootPlayer(other, 10);
    }

    [Command]
    void CmdPlayerShootPlayer(string Id, int life)
    {
        Debug.Log("CmdPlayerShootPlayer");
        PlayerSetupNetwork player = GameManager.GetPlayer(Id);

        if (player.gameObject != null)
        {
            player.gameObject.GetComponent<CanvasManagerNetwork>().RpcGiveLife(life);
        }

        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
