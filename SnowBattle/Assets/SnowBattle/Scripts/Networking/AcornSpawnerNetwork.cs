using UnityEngine;
using UnityEngine.Networking;

public class AcornSpawnerNetwork : NetworkBehaviour {
    public GameObject acornPrefab;
    private GameObject[] acorns;
    public int numberOfAcorns;

    public override void OnStartServer()
    {
        for (int i = 0; i < numberOfAcorns; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-20.0f, 20.0f),
                2.0f,
                Random.Range(-20.0f, 20.0f));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);

            GameObject acorn = Instantiate(acornPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(acorn);
        }
    }
}
