using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootingNetwork : NetworkBehaviour
{
    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 100f;
    public GameObject particlePuff;// The distance the gun can fire.

    [SerializeField]
    Transform barrelGun;
    [SerializeField]
    GameObject barrelGunO;

    float timer;                                    // A timer to determine when to fire.
    Ray shootRay;                                   // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
    Light gunLight;                                 // Reference to the light component.
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
    bool mouseDown;
    void Awake()
    {
        // Create a layer mask for the Shootable layer...
        shootableMask = LayerMask.GetMask("Shootable");

        // Set up the references...
        gunParticles = barrelGunO.GetComponent<ParticleSystem>();
        gunLine = barrelGunO.GetComponent<LineRenderer>();
        gunAudio = barrelGunO.GetComponent<AudioSource>();
        gunLight = barrelGunO.GetComponent<Light>();
    }

    void Update()
    {
        // Add the time since Update was last called to the timer...
        if (isLocalPlayer)
        {
            timer += Time.deltaTime;

            // If the Fire1 button is being press and it's time to fire...
            if (timer >= timeBetweenBullets && mouseDown)
            {
                // shoot the gun...
                Shoot();
            }

            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // disable the effects...
                DisableEffects();
            }
        }
    }
    [Client]
    public void DisableEffects()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CmdDisableEffects();
    }

    [Command]
    public void CmdDisableEffects()
    {
        RpcDisableEffects();
    }
    [ClientRpc]
    public void RpcDisableEffects()
    {
        // Disable the line renderer and the light...
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    [Client]
    public void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        timer = 0f;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, LayerMask.GetMask("Shootable")))
        {
            CmdShootPlayerEffect();

            /* if (tag=="Up"&& shootHit.collider.tag == "Down")
             {
                 CmdPlayerShootPlayer(shootHit.collider.name, 10, transform.name);
                 Debug.Log("CmdPlayerShootPlayer" + shootHit.collider.transform.root.name);
             }*/
            /* if (tag=="Down" && shootHit.collider.tag == "Up")
             {
                 CmdPlayerShootPlayer(shootHit.collider.name, 10, transform.name);
                 Debug.Log("CmdPlayerShootPlayer" + shootHit.collider.transform.root.name);
             }*/

            if (shootHit.collider.tag == "Player")
            {
                CmdPlayerShootPlayer(shootHit.collider.name, 10, transform.name);
                Debug.Log("CmdPlayerShootPlayer" + shootHit.collider.transform.root.name);
            }

        }// If the raycast didn't hit anything on the shootable layer...
        else
        {
            //set the second position of the line renderer to the fullest extent of the gun's range...
            CmdShootEffect();
        }
    }

    public void OnPointerDown()
    {
        mouseDown = true;
    }

    public void OnPointerUp()
    {
        mouseDown = false;
    }

    [Command]
    void CmdPlayerShootPlayer(string Id, int damage,string sourceName)
    {
        PlayerSetupNetwork player = GameManager.GetPlayer(Id);

        if (player.gameObject != null)
        {
             player.gameObject.GetComponent<CanvasManagerNetwork>().RpcTakeDamage(damage,sourceName);
        }
    }

    [Command]
    void CmdShootPlayerEffect()
    {
        Rpc_DoShootPlayerEffect();
    }

    [ClientRpc]
    void Rpc_DoShootPlayerEffect()
    {
        // Play the gun shot audioclip.
        gunAudio.Play();

        // Enable the light.
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, barrelGun.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = barrelGun.position;
        shootRay.direction = barrelGun.forward;

        GameObject particle = Instantiate(particlePuff, shootHit.point, Quaternion.identity);
        Destroy(particle, 1f);
        gunLine.SetPosition(1, shootHit.point);
    }

    [Command]
    void CmdShootEffect()
    {
        Rpc_DoShootEffect();
    }

    [ClientRpc]
    void Rpc_DoShootEffect()
    {
        // Play the gun shot audioclip.
        gunAudio.Play();

        // Enable the light.
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, barrelGun.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = barrelGun.position;
        shootRay.direction = barrelGun.forward;

        GameObject particle = Instantiate(particlePuff, shootRay.origin + shootRay.direction * range, Quaternion.identity);
        Destroy(particle, 1f);
        gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
    }
}
