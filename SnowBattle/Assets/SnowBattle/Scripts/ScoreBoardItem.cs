using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardItem : MonoBehaviour {
    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killsText;

    [SerializeField]
    Text deathsText;

    //04006D
    public void Setup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killsText.text = "Kills: "+kills;
        deathsText.text = "Death: " + deaths;
    }
}
