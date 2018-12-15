using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreItem;

    [SerializeField]
    Transform playerList;

    bool blue;

    private void OnEnable()
    {
        // Get all players

        PlayerSetupNetwork[] players = GameManager.GetAllPlayers();

        //Loop through and set up a list item for each player score

        foreach (PlayerSetupNetwork player in players)
        {

            // Debug.Log(player.username + "|" + player.GetComponent<CanvasManager>().kills + "|" + player.GetComponent<CanvasManager>().deaths);

            GameObject itemGO= Instantiate(playerScoreItem, playerList);
            if (blue)
            {
                itemGO.GetComponent<Image>().color = new Color(0, 0, 0);
                blue = false;
            }
            else
            {
                blue = true;
            }


            ScoreBoardItem item = itemGO.GetComponent<ScoreBoardItem>();

            if (item!=null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }
}
