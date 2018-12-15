using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager: NetworkManager
{
    public TeamSettings Up;
    public TeamSettings Down;

    public int currentplayer;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        Debug.Log("OnServerAddPlayer");

        TeamSettings team = GetTeam(conn.connectionId);

        Transform spawnPoint;
        do
        {
            spawnPoint = GetStartPosition();
        }
        while (team.name != spawnPoint.name);


        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.selectedCharacter;
        Debug.Log("server add with message " + selectedClass);


        Debug.Log("Spawning at: " + spawnPoint.name);

        GameObject player = Instantiate(spawnPrefabs[selectedClass], spawnPoint.position, spawnPoint.rotation);

        Debug.Log("Set up team to the player");
        player.GetComponent<PlayerSetupNetwork>().myTeam = team;

        Debug.Log("AddPlayerForConnection");
        NetworkServer.AddPlayerForConnection(conn, player, 0);

        Debug.Log("RPC Set up connection and team to the player");
        player.GetComponent<PlayerSetupNetwork>().RpcSetupPlayer(conn.connectionId, team);

    }

    public TeamSettings GetTeam(int connectionID)
    {
        Debug.Log("GetTeam: " + connectionID);

        TeamSettings T = null;

        if (connectionID == 0)
        {
            Debug.Log("connectionID == 0");

            if (VariablesRealTime.team == "Bear")
            {
                Debug.Log("VariablesRealtime equals: " + Up.name);
                T = Up;
            }

            if (VariablesRealTime.team == "Cat")
            {
                Debug.Log("VariablesRealtime equals: " + Down.name);
                T = Down;
            }

        }
        else
        {
            Debug.Log("connectionID != 0");

            if (connectionID % 2 == 0)
            {
                T = Down;
            }
            else
                T = Up;
        }
        return T;
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage test = new NetworkMessage();
        test.selectedCharacter = currentplayer;

        ClientScene.AddPlayer(conn, 0, test);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }
}
