using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public MatchSettings matchSettings;
    public delegate void OnPlayerKilledCallback(string player, string action, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Start()
    {
        Debug.Log("Start");

        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Awake()
    {
        Debug.Log("Awake");

        if (instance != null)
        {
            Debug.Log("More than one manager in the scene.");
        }
        else
        {
            instance = this;
        }

    }
    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerSetupNetwork> players = new Dictionary<string, PlayerSetupNetwork>();

    public static void RegisterPlayer(string netID, PlayerSetupNetwork player)
    {
        Debug.Log("RegisterPlayer: player"+netID);

        string playerID = PLAYER_ID_PREFIX + netID;

        players.Add(playerID, player);

        player.transform.name = playerID;
    }

    public static void UnregisterPlayer(string playerId)
    {
        Debug.Log("UnregisterPlayer:"+playerId);

        players.Remove(playerId);
    }

    public static PlayerSetupNetwork GetPlayer(string playerId)
    {
        Debug.Log("GetPlayer");

        return players[playerId];
    }


    public static PlayerSetupNetwork[] GetAllPlayers()
    {
        Debug.Log("GetAllPlayers");

        return players.Values.ToArray();
    }
}
