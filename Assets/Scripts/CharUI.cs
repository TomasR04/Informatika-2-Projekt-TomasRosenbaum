using UnityEngine;

public class CharUI : MonoBehaviour
{
    public GameObject character;
    public void Heal(Item medkit) 
    { 
        var pc = character.GetComponent<PlayableControler>();
        pc.Heal(medkit);
    }

    public void RefilMagazine()
    {
        Debug.Log("Refilling magazine from UI");
        character.GetComponent<PlayableControler>().RefilMagazine();
    }
    public void AddMagazine()
    {

    }
}
