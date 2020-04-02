using UnityEngine;
using System.Collections;

//made by Linus Edlund
public class GunShoot : MonoBehaviour
{
    
    public bool canFullauto;
    public bool canBurst;
    public bool canSemi;
    public bool canSpread;
    public bool canExplode;
    private bool ableToFullAuto;
    private bool ableToBurst;
    private bool ableToSemi;
    private bool ableToSpread;
    private bool ableToExplode;
    public float damage = 10f; //Hur mycket vapnet skadar
    public float fireRate = 15f; //Hur snabbt den kan skjuta: Mest for full auto!
    public float impactForce = 30f; //Hur mycket kraft en smäll har!
    private bool isBursting = false;
    private bool isSpreading = false;
    private int currentFireMode;
    private int maxAmountFireModes;
    private int fullAutoNumber;
    private int burstNumber;
    private int semiNumber;
    private int spreadNumber;
    private int explosionNumber;

    public int maxAmmo = 10; //Max Ammo
    public int currentAmmo; //Hur mycket ammo som är kvar
    public float reloadTime = 1f; //Hur lång tid det tar att reloada
    public int BurstAmount = 3; //hur många skott burst skjuter
    private Vector3 spread;
    
    private bool isReloading = false;
    //private Quaternion direction;

    Transform Cam;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bulletLook;
    public ParticleSystem explosionMuzzleFlash;
    public ParticleSystem explosionBullet;
    
    
    public GameObject Gun;
    public GameObject impactEffect;
    private Ray ray;
    private RaycastHit hit;

    public Transform barrel;

    private float NextTimeToShoot = 0.01f;
    

    public Animator animator;

    private void Start()
    {
        Cam = Camera.main.transform;
        if (currentAmmo == -1)
            currentAmmo = maxAmmo;
        CheckFireModes();         //se vilka firemodes vapnet kan använda
        TheFirstFireMode();       //Sätter det primära firemodet
        CountFireModes();         //räknar antalet Firemodes som kan användas
        FireModeNumbers();        //sätter number på alla firemodes beroende om den kan använda den eller inte
        
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)    //Om current ammo är lika med eller mindre än 0 så starter den
        {
            StartCoroutine(Reload());
            return;
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            NextFireMode();
        }
        if (currentFireMode == fullAutoNumber) 
        {
            canFullauto = true;
            canBurst = false;
            canSemi = false;
            canSpread = false;
            canExplode = false;
            if (canFullauto == true)
            {
                if (Input.GetButton("Fire1") && Time.time >= NextTimeToShoot)
                {
                    NextTimeToShoot = Time.time + 1f / fireRate;
                    Shoot();
                }

            }
        }
        ray = new Ray(Cam.position, Cam.forward);
        

        Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.green);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Direction of ray from gun to the hit point.
            Vector3 redDir = hit.point - Gun.transform.position;
            Debug.DrawRay(Gun.transform.position, redDir * 10f, Color.red);
        }


        if (currentFireMode == burstNumber)
        {
            canFullauto = false;
            canBurst = true;
            canSemi = false;
            canSpread = false;
            canExplode = false;
            if (canBurst == true)
            {
                if (Input.GetButtonDown("Fire1") && isBursting == false)
                {
                    StartCoroutine(BurstFire());
                    StartCoroutine(BurstDelay());
                }

            }
        }

        if(currentFireMode == semiNumber)
        {
            canFullauto = false;
            canBurst = false;
            canSemi = true;
            canSpread = false;
            canExplode = false;
            if (canSemi == true)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }

            }
        }
        if (currentFireMode == spreadNumber)
        {
            canFullauto = false;
            canBurst = false;
            canSemi = false;
            canSpread = true;
            canExplode = false;
            if (canSpread == true)
            {
                if (Input.GetButtonDown("Fire1") && isSpreading == false)
                {
                    StartCoroutine(SpreadShootFire());
                }

            }
        }
        if (currentFireMode == explosionNumber)
        {
            canFullauto = false;
            canBurst = false;
            canSemi = false;
            canSpread = false;
            canExplode = true;
            if (canExplode == true)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    explosionShoot();
                }

            }
        }


    }

    IEnumerator Reload ()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        animator.SetBool("Reloading", true);                    //Sätter igång animation när man reloadar då ammo är 0 eller mindre.

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Play();
        //bulletLook.Play();
        currentAmmo--;



        if (Physics.Raycast(ray, out hit, Mathf.Infinity))       //Shooting with Raycasts
        {

            Debug.Log(hit.transform.name);    //Skriver vad den träffar
            barrel.transform.LookAt(hit.point); //riktar var den ska åka
            Instantiate(bulletLook, barrel.position, barrel.rotation);
            
        }
        else
        {
            Instantiate(bulletLook, barrel.position, Cam.rotation);
        }
    }
    //void Shoot2()
    //{
    //    muzzleFlash.Play();
    //    //bulletLook.Play();
    //    currentAmmo--;

        

    //    if (Physics.Raycast(ray, out hit, Mathf.Infinity))       //Shooting with Raycasts
    //    {
            
    //        Debug.Log(hit.transform.name);    //Skriver vad den träffar
    //        barrel.transform.LookAt(hit.point); //riktar var den ska åka
    //        Instantiate(bulletLook, barrel.position, barrel.rotation);
    //        Target target = hit.transform.GetComponent<Target>();      //Skadar Target
    //        if (target != null)
    //        {
    //            //target.TakeDamage(damage);
    //        }

    //        if (hit.rigidbody != null)
    //        {
    //            hit.rigidbody.AddForce(-hit.normal * impactForce);
    //        }

    //        GameObject ImpactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));  //Gör så det blir tryck på vapen.
    //        Destroy(ImpactGO, 2f);
    //    }
    //}
    //void Shoot3()
    //{
    //    muzzleFlash.Play();

    //    currentAmmo--;

    //    RaycastHit hit;

    //    if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, range))       //Shooting with Raycasts
    //    {
    //        Debug.Log(hit.transform.name);    //Skriver vad den träffar

    //        Target target = hit.transform.GetComponent<Target>();      //Skadar Target
    //        if(target != null)
    //        {
    //            //target.TakeDamage(damage);
    //        }

    //        if(hit.rigidbody != null)
    //        {
    //            hit.rigidbody.AddForce(-hit.normal * impactForce);
    //        }

    //        GameObject ImpactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));  //Gör så det blir tryck på vapen.
    //        Destroy(ImpactGO, 2f);
    //    }
    //}

    void BuckShoot()//vi måste göra så när man skjuter åker partiklar i den riktning skottet flyger
    {
        muzzleFlash.Play();

        currentAmmo--;


        for (int i = 0; i < 8; i++)
        {
            spread = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
            ray = new Ray(Cam.position, Cam.forward + spread);


            if (Physics.Raycast(ray, out hit, Mathf.Infinity))       //Shooting with Raycasts
            {
                Debug.Log(hit.transform.name);    //Skriver vad den träffar
                barrel.transform.LookAt(hit.point); //riktar var den ska åka
                Instantiate(bulletLook, barrel.position, barrel.rotation);
                
            }
            else
            {
                Quaternion direction = Quaternion.Euler(spread * 100);
                Instantiate(bulletLook, barrel.position, Cam.rotation * direction);

            }

        }

    }
    //void BuckShoot2()//vi måste göra så när man skjuter åker partiklar i den riktning skottet flyger
    //{
    //    muzzleFlash.Play();

    //    currentAmmo--;


    //    for (int i = 0; i < 8; i++)
    //    {
    //    spread = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));


    //        ray = new Ray(Cam.position, Cam.forward + spread);


    //    if (Physics.Raycast(ray, out hit, Mathf.Infinity))       //Shooting with Raycasts
    //        {
    //            Debug.Log(hit.transform.name);    //Skriver vad den träffar
    //            barrel.transform.LookAt(hit.point); //riktar var den ska åka
    //            Instantiate(bulletLook, barrel.position, barrel.rotation);
    //            Target target = hit.transform.GetComponent<Target>();      //Skadar Target

    //            if (target != null)
    //            {
    //                //target.TakeDamage(damage);
    //            }

    //        if (hit.rigidbody != null)
    //            {
    //                hit.rigidbody.AddForce(-hit.normal * impactForce);
    //            }

    //            GameObject ImpactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));  //Gör så det blir tryck på vapen.
    //            Destroy(ImpactGO, 2f);
    //        }
    //        else
    //        {
    //            Quaternion direction = Quaternion.Euler(spread * 100);
    //            Instantiate(bulletLook, barrel.position, Cam.rotation * direction);

    //        }

    //    }

    //}
    //void BuckShoot3()
    //{
    //    muzzleFlash.Play();

    //    currentAmmo--;

    //    RaycastHit hit;
    //    for (int i = 0; i < 8; i++)
    //    {
    //        spread = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
    //        //direction = Quaternion.Euler(spread);
    //        //Instantiate(bullet, barrel.position, direction);

    //        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward + spread, out hit, range))       //Shooting with Raycasts
    //        {
    //            Debug.Log(hit.transform.name);    //Skriver vad den träffar
    //            Target target = hit.transform.GetComponent<Target>();      //Skadar Target
    //            if (target != null)
    //            {
    //                //target.TakeDamage(damage);
    //            }

    //            if (hit.rigidbody != null)
    //            {
    //                hit.rigidbody.AddForce(-hit.normal * impactForce);
    //            }

    //            GameObject ImpactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));  //Gör så det blir tryck på vapen.
    //            Destroy(ImpactGO, 2f);
    //        }
    //    }
    //}

    void explosionShoot()
    {
        explosionMuzzleFlash.Play();
        //bulletLook.Play();
        currentAmmo--;



        if (Physics.Raycast(ray, out hit, Mathf.Infinity))       //Shooting with Raycasts
        {

            Debug.Log(hit.transform.name);    //Skriver vad den träffar
            barrel.transform.LookAt(hit.point); //riktar var den ska åka
            Instantiate(explosionBullet, barrel.position, barrel.rotation);

        }
        else
        {
            Instantiate(explosionBullet, barrel.position, Cam.rotation);
        }
    }
    void NextFireMode()
    {
        currentFireMode++;
        if (currentFireMode == maxAmountFireModes)
        {
            currentFireMode = 0;
        }
    }
    
    IEnumerator BurstFire()
    {
        isBursting = true;
        for (int i = 0; i < BurstAmount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.1f); //borde göra om till variabel så man kan välja tid mellan burst

        } 
    }

    IEnumerator BurstDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isBursting = false;
    }
    IEnumerator SpreadShootFire()
    {
        isSpreading = true;
        Debug.Log(isSpreading);
        BuckShoot();
        yield return new WaitForSeconds(1.5f);
        isSpreading = false;
        Debug.Log(isSpreading);
    }
    void CheckFireModes()
    {
        if (canFullauto == true)
        {
            ableToFullAuto = true;
        }
        if (canBurst == true)
        {
            ableToBurst = true;
        }
        if (canSemi == true)
        {
            ableToSemi = true;
        }
        if(canSpread == true)
        {
            ableToSpread = true;
        }
        if (canExplode == true)
        {
            ableToExplode = true;
        }
    }
    void TheFirstFireMode()
    {
        if (canFullauto == true)
        {
            canBurst = false;
            canSemi = false;
            canSpread = false;
            canExplode = false;
        }
        if (canBurst == true)
        {
            canFullauto = false;
            canSemi = false;
            canSpread = false;
            canExplode = false;
        }
        if (canSemi == true)
        {
            canFullauto = false;
            canBurst = false;
            canSpread = false;
            canExplode = false;
        }
        if (canSpread == true)
        {
            canFullauto = false;
            canBurst = false;
            canSemi = false;
            canExplode = false;
        }
        if (canExplode == true)
        {
            canFullauto = false;
            canBurst = false;
            canSemi = false;
            canSpread = false;
        }
    }
    void CountFireModes()
    {
        if (ableToFullAuto == true)
        {
            maxAmountFireModes++;
        }
        if (ableToBurst == true)
        {
            maxAmountFireModes++;
        }
        if(ableToSemi == true)
        {
            maxAmountFireModes++;
        }
        if(ableToSpread == true)
        {
            maxAmountFireModes++;
        }
        if(ableToExplode == true)
        {
            maxAmountFireModes++;
        }
    }
    void FireModeNumbers()
    {
        if(ableToFullAuto == true)
        {
            fullAutoNumber = 0;
        }
        if(ableToBurst == true)
        {
            burstNumber = fullAutoNumber + 1;
        }
        if(ableToSemi == true)
        {
            semiNumber = burstNumber + 1;
        }
        if(ableToSpread == true)
        {
            spreadNumber = semiNumber + 1;
        }
        if(ableToExplode == true)
        {
            explosionNumber = spreadNumber + 1;
        }

        RemoveTheNumbers();
    }

    void RemoveTheNumbers()
    {
        if (ableToFullAuto == false)
        {
            fullAutoNumber = -5;
            burstNumber--;
            semiNumber--;
            spreadNumber--;
            explosionNumber--;
        }
        if (ableToBurst == false)
        {
            burstNumber = -5;
            
        }
        if (ableToSemi == false)
        {
            semiNumber = -5;
        }
        if(ableToSpread == false)
        {
            spreadNumber = -5;
        }
        if (ableToExplode == false)
        {
            explosionNumber = -5;
        }
    }
    
}

