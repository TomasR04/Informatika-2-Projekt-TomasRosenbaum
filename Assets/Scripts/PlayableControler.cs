using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Assets.Scripts;

public class PlayableControler : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public CharacterController controller;
    public CharacterInventory inventory;
    public GameObject charUI;
    TMPro.TextMeshProUGUI charNameText;
    TMPro.TextMeshProUGUI healthText;
    TMPro.TextMeshProUGUI speedText;
    TMPro.TextMeshProUGUI aimingText;
    TMPro.TextMeshProUGUI medicineText;


    public string charName = "Boøek";
    public int health = 100;
    public int speed = 10;
    public int aiming = 10;
    public int medicine = 10;
    public bool canShoot = true;
    bool isReloading = false;
    public float stopRange = 5f;

    Vector3 destinationPoint;
    public List<GameObject> visibleTargets = new List<GameObject>();
    public GameObject currentTarget;



    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        inventory = GetComponent<CharacterInventory>();

        /*charUI = GameObject.Find("CharacterUI");
        Debug.Log(charUI);
        if (charUI != null)
        {
            
            charNameText = charUI.transform.Find("Name Text").GetComponent<TMPro.TextMeshProUGUI>();
            healthText = charUI.transform.Find("Health Text").GetComponent<TMPro.TextMeshProUGUI>();
            speedText = charUI.transform.Find("Speed Text").GetComponent<TMPro.TextMeshProUGUI>();
            aimingText = charUI.transform.Find("Aim Text").GetComponent<TMPro.TextMeshProUGUI>();
            medicineText = charUI.transform.Find("Med Text").GetComponent<TMPro.TextMeshProUGUI>();
        }*/


    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        charUI = GameObject.Find("CharacterUI");
        inventory = GetComponent<CharacterInventory>();
        inventory.reloading += OnReloading;
        inventory.reloadingDone += OnRealodingDone;
        //Debug.Log(charUI);
        if (charUI != null)
        {

            charNameText = charUI.transform.Find("Name Text").GetComponent<TMPro.TextMeshProUGUI>();
            healthText = charUI.transform.Find("Health Text").GetComponent<TMPro.TextMeshProUGUI>();
            speedText = charUI.transform.Find("Speed Text").GetComponent<TMPro.TextMeshProUGUI>();
            aimingText = charUI.transform.Find("Aim Text").GetComponent<TMPro.TextMeshProUGUI>();
            medicineText = charUI.transform.Find("Med Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
    }
    void OnRealodingDone()
    {
        visibleTargets.Clear();
        currentTarget = null;
        isReloading = false;
        animator.Play("Idle");
        animator.SetBool("Aim", false);
        if (inventory.equipedItem.itemType == Item.ItemType.shortWeapon)
        {
            //Debug.Log("Using short weapon");
            animator.SetBool("HasShortGun", true);
            animator.SetBool("HasLongGun", false);

        }
        else if (inventory.equipedItem.itemType == Item.ItemType.longWeapon)
        {
            animator.SetBool("HasLongGun", true);
            animator.SetBool("HasShortGun", false);

        }
        
        
    }
    void OnReloading()
    {
        
        isReloading = true;
        animator.Play("Reloading");
        //animator.SetBool("Aim", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }
        if (destinationPoint!=null)
        {
            CheckArrival();
        }
        ScanForTargets();
        if (currentTarget != null && inventory.equipedItem!=null)
        {
            Gun gun = (Gun)inventory.equipedItem;
            if (canShoot && Vector3.Distance(transform.position, currentTarget.transform.position)<=gun.range)
            {
                
                Vector3 directionToTarget = currentTarget.transform.position - transform.position;
                directionToTarget.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                if (!animator.GetBool("Walk"))
                {
                    CheckArrival();
                    agent.ResetPath();
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                    animator.SetBool("Aim", true);
                    OnAimed();
                }
                
                if (inventory.equipedItem.itemType == Item.ItemType.shortWeapon)
                {
                    //Debug.Log("Using short weapon");
                    animator.SetBool("HasShortGun", true);
                    animator.SetBool("HasLongGun", false);
                    
                }
                else if (inventory.equipedItem.itemType == Item.ItemType.longWeapon)
                {
                    animator.SetBool("HasLongGun", true);
                    animator.SetBool("HasShortGun", false);
                    
                }
            }
            else
            {
                animator.SetBool("Aim", false);
            }
        }

    }
    void CheckArrival()
    {
        if (Vector3.Distance(transform.position, destinationPoint) < 0.5f)
        {
            agent.velocity = Vector3.zero;
            animator.SetBool("Walk", false);
        }
    }
    void ScanForTargets()
    {
        visibleTargets.Clear();
        for (int i = -90; i <= 90; i ++)
        {
            Vector3 dir = Quaternion.Euler(0, i, 0) * transform.forward;
            Ray ray = new Ray(transform.position + Vector3.up, dir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Enemy"))
                {
                    //Debug.Log("Spotted enemy: " + hitObject.name);
                    if (!visibleTargets.Contains(hitObject))
                    {
                        visibleTargets.Add(hitObject);
                    }
                }
            }
        }
        if (visibleTargets.Count > 0)
        {
            foreach (GameObject target in visibleTargets)
            {
                if (currentTarget == null)
                {
                    currentTarget = target;
                }
                else
                {
                    float currentDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
                    float newDistance = Vector3.Distance(transform.position, target.transform.position);
                    if (newDistance < currentDistance)
                    {
                        currentTarget = target;
                    }
                }
            }
        }
        else
        {
            currentTarget = null;
            animator.SetBool("Aim", false);
        }
    }
    public void OnAimed()
    {
        //Debug.Log("Firing at target: " + currentTarget.name);
        Gun gun = (Gun)inventory.equipedItem;
        gun.ShotsFired = OnShotsFired;
        gun.UseItem();
    }
    public void Selected()
    {
        charNameText.text = "Name: " + charName;
        healthText.text = "Health: " + health.ToString();
        speedText.text = "Speed: " + speed.ToString();
        aimingText.text = "Aiming: " + aiming.ToString();
        medicineText.text = "Medicine: " + medicine.ToString();

        charUI.SetActive(true);

    }
    public void OnShotsFired()
    {
        ZombieControler zombie = currentTarget.GetComponent<ZombieControler>();
        float chanceToHit = aiming - Vector3.Distance(transform.position, currentTarget.transform.position);
        float roll = Random.Range(0f, 50f);
        float result = chanceToHit + roll;
        Debug.Log("-----------------------------");
        Debug.Log("Šance: " +chanceToHit);
        Debug.Log("Hod: " + roll);
        Debug.Log("Výsledek: " + result);
        if (result > 50)
        {
            Debug.Log("Critical Hit!");
            zombie.Die();
            visibleTargets.Clear();
            currentTarget = null;
            return;
        }
        else if (result > 20)
        {
            Debug.Log("Hit the target!");
            zombie.ReciveHit();
        }
        else
        {
            Debug.Log("Missed the shot!");
        }
    }


    public void Deselected()
    {
        charUI.SetActive(false);
    }
    public void MoveTo(Vector3 destination)
    {
        destinationPoint = destination;
        agent.SetDestination(destinationPoint);
        animator.SetBool("Walk", true);
    }
    
}
