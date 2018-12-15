using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {

	[SerializeField]
	Text text;

	public void Setup (string player, string action, string source)
	{
        //text.text = "<b>" + source + "</b>" + " kills " + "<i>" + player + "</i>";
        text.text = "<b>" + source + " </b>" + action + " <i>" + player + "</i>";
    }

}
