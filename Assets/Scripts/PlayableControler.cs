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
    
    public bool canShoot = true;
    bool isReloading = false;
    public float stopRange = 5f;
    public float sightRange = 100f;
    public float detectRadius = 10f;

    Vector3 destinationPoint;
    public List<GameObject> visibleTargets = new List<GameObject>();
    public GameObject currentTarget;

    bool selected = false;

    public BaseControl baseControl;
    public GlobalControl globalControl;



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
        CollectItems();

    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //charUI = GameObject.Find("CharacterUI");
        
        inventory = GetComponent<CharacterInventory>();
        inventory.reloading += OnReloading;
        inventory.reloadingDone += OnRealodingDone;
        //Debug.Log(charUI);
        
        CollectItems();
        if (baseControl == null)
        {
            baseControl = GameObject.Find("Base").GetComponent<BaseControl>();
        }
        if (globalControl == null)
        {
            globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
        }
        charUI = globalControl.charUI;
        if (charUI != null)
        {

            charNameText = charUI.transform.Find("Name Text").GetComponent<TMPro.TextMeshProUGUI>();
            healthText = charUI.transform.Find("Health Text").GetComponent<TMPro.TextMeshProUGUI>();
            speedText = charUI.transform.Find("Speed Text").GetComponent<TMPro.TextMeshProUGUI>();
            aimingText = charUI.transform.Find("Aim Text").GetComponent<TMPro.TextMeshProUGUI>();

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
        if (destinationPoint != null)
        {
            CheckArrival();
        }
        ScanForTargets();
        if (currentTarget != null && inventory.equipedItem != null)
        {
            Gun gun = (Gun)inventory.equipedItem;
            if (canShoot && Vector3.Distance(transform.position, currentTarget.transform.position) <= gun.range)
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
                    //Debug.Log("Using long weapon");

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
        for (int i = -90; i <= 90; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i, 0) * transform.forward;
            Ray ray = new Ray(transform.position + Vector3.up, dir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, sightRange))
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

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                if (!visibleTargets.Contains(hitCollider.gameObject))
                {
                    if (HasLighOfSightTo(hitCollider.gameObject))
                        visibleTargets.Add(hitCollider.gameObject);
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
    bool HasLighOfSightTo(GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - transform.position;
        directionToTarget.y += 1.5f; // Adjust for height if necessary
        Ray ray = new Ray(transform.position + Vector3.up, directionToTarget.normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectRadius))
        {
            if (hit.collider.gameObject == target)
            {
                return true; // Clear line of sight
            }
        }
        return false; // Obstructed line of sight
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

        charUI.SetActive(true);
        selected = true;

    }
    public void OnShotsFired()
    {
        ZombieControler zombie = currentTarget.GetComponent<ZombieControler>();
        
        float chanceToHit = aiming - Vector3.Distance(transform.position, currentTarget.transform.position);
        float roll = Random.Range(0f, 50f);
        float result = chanceToHit + roll;

        if (result > 50)
        {

            zombie.Die();
            visibleTargets.Clear();
            currentTarget = null;
            return;
        }
        else if (result > 10)
        {

            zombie.ReciveHit();
        }
        
    }
    public void ReciveHit()
    {
        health -= 30;
        if (selected)
        {
            Selected();
        }
        if (health <= 0)
        {
            Die();
        }

    }
    public void Die()
    {
        gameObject.tag = "Eliminated";
        animator.SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
        agent.isStopped = true;
        
        this.enabled = false;
    }

    public void Deselected()
    {
        selected = false;
        charUI.SetActive(false);
    }
    public void MoveTo(Vector3 destination)
    {

        destinationPoint = destination;
        agent.SetDestination(destinationPoint);
        animator.SetBool("Walk", true);
    }

    void CollectItems()
    {       
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hitCollider in hitColliders)
        {
            Item item = hitCollider.GetComponent<Item>();
            if (item != null)
            {
                Transform parent = hitCollider.transform.parent;
                if (parent == null) { 
                    inventory.AddItem(item);                   
                }
            }
        }
    }

    public void Heal(Item medkit)
    {
        if (medkit == null || medkit.itemType != Item.ItemType.medicine)
        {
            Debug.Log("Invalid medkit.");
            return;
        }
        health += medkit.value;
        if (health > 100)
        {
            health = 100;
        }
        if (selected)
        {
            Selected();
        }
    }

    public void RefilMagazine()
    {
        Debug.Log("Refilling magazine in playable controler");
        int ammo = baseControl.GetAllAmmoOfType(inventory.equipedItem.GetComponent<Gun>().magazine.magazineType.ToString());
        int returnAmmo = 0;
        if (ammo > 0)
        {
            returnAmmo = inventory.RefilMagazineAndReturn(ammo);
        }
        baseControl.SetAmmoOfTypeTo(inventory.equipedItem.GetComponent<Gun>().magazine.magazineType.ToString(), returnAmmo);
        if (globalControl != null)
        {
            globalControl.Refiled();
        }

    }
    public void PlaceOnNavMesh()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        NavMeshHit hit;

        // hledá NavMesh v okolí aktuální pozice
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            transform.position = hit.position;
            agent.enabled = true;
        }
        else
        {
            Debug.LogWarning($"{name} – nelze najít NavMesh poblíž pozice {transform.position}");
        }
    }

}
