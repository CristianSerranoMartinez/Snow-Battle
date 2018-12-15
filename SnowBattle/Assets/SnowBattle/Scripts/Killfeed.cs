using UnityEngine;
using System.Collections;

public class Killfeed : MonoBehaviour {

	[SerializeField]
	GameObject killfeedItemPrefab;

	// Use this for initialization...
	void Start ()
    {
        Debug.Log("Killfeed Start");

        if (GameManager.instance != null)
        {
            GameManager.instance.onPlayerKilledCallback += OnKill;
        }
	}

	public void OnKill (string player,string action, string source)
	{
        Debug.Log("OnKill");

        GameObject go = Instantiate(killfeedItemPrefab, transform);

		go.GetComponent<KillfeedItem>().Setup(player,action, source);

		Destroy(go, 4f);
	}

}
