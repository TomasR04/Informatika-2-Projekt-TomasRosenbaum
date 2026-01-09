using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : MonoBehaviour
{
    public int baseRadius = 50;
    public GlobalControl globalControl;
    public int money = 0;
    public List<GameObject> characters;

    void Start()
    {
        globalControl.treeSpawned += OnTreeSpawned;
    }

    private void Awake()
    {
        globalControl.treeSpawned += OnTreeSpawned;
    }
    // Update is called once per frame
    

    public void OnTreeSpawned()
    {
        //Debug.Log("Tree spawned, clearing area around base.");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, baseRadius);
        List<GameObject> treesInRange = new List<GameObject>();
        foreach (var hitCollider in hitColliders)
        {
            
            if (hitCollider.CompareTag("Tree"))
            {
                //Debug.Log("Tree found within base radius: " + hitCollider.gameObject.name);
                treesInRange.Add(hitCollider.gameObject);
            }
        }
        
        foreach (var tree in treesInRange)
        {
            Destroy(tree);
        }


    }
}
