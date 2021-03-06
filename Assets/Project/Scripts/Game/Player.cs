using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Visuals")]
    public Camera playerCamera;

    [Header("Gameplay")]
    public int initialHealth = 100;
    public int initialAmmo = 12;
    public GameObject bulletPrefab;
    public float knockbackForce = 10;
    public float hurtDuration = 0.5f;

    private int health;
    public int Health { get { return health; } }


    public int ammo;
    public int Ammo { get { return ammo; } }

    public bool killed;
    public bool Killed { get { return killed; } }

    private bool isHurt;
    // Start is called before the first frame update
    void Start()
    {
        health = initialHealth;
        ammo = initialAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ammo > 0 && Killed == false)
            {
                ammo--;
                GameObject bulletObject = ObjectPoolingManager.Instance.GetBullet(true);
                bulletObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward;
                bulletObject.transform.forward = playerCamera.transform.forward;
            }
        }
    }

    // check for collisions
    void OnTriggerEnter(Collider otherCollider)
    {
       
       if (otherCollider.GetComponent<AmmoCrate>() != null)
        {
            // collide with ammo crates
            AmmoCrate ammoCrate = otherCollider.GetComponent<AmmoCrate>();
            ammo += ammoCrate.ammo;

            Destroy(ammoCrate.gameObject);
        }

        else if (otherCollider.GetComponent<HealthCrate>() != null)
        {
            // collide with ammo crates
            HealthCrate healthCrate = otherCollider.GetComponent<HealthCrate>();
            health += healthCrate.health;

            Destroy(healthCrate.gameObject);
        }

        if (isHurt == false)
        {
            GameObject hazard = null;
             if (otherCollider.GetComponent<Enemy>() != null)
            {
                
            
                //touching enemies
                Enemy enemy = otherCollider.GetComponent<Enemy>();
                if (enemy.Killed == false)
                {
                    hazard = enemy.gameObject;
                    health -= enemy.damage;

                    //isHurt = true; 
                }
            }
             else if(otherCollider.GetComponent<Bullet>() != null)
            {
                Bullet bullet = otherCollider.GetComponent<Bullet>();
                if(bullet.ShotByPlayer == false)
                {
                    hazard = bullet.gameObject;
                    health -= bullet.damage;
                    bullet.gameObject.SetActive(false);

                }
            }
             if (hazard != null)
            {
                isHurt = true;
                // Perform the knockback effect.
                Vector3 hurtDirection = (transform.position - hazard.transform.position).normalized;
                Vector3 knockbackDirection = (hurtDirection + Vector3.up).normalized;
                GetComponent<ForceReceiver>().AddForce(knockbackDirection, knockbackForce);

                StartCoroutine(HurtRoutine());
            }

             if(health <= 0)
            {
                if(killed == false)
                {
                    killed = true;

                    OnKill();
                }
            }
        }


    }

    IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(hurtDuration);

        isHurt = false;
    }

    private void OnKill()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
    }
}
