using DG.Tweening;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    public string npcName;
    public string jobName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Can Talk");
            other.GetComponent<PlayerInteraction>().currentDialogue = dialogue;
            other.GetComponent<PlayerInteraction>().npcName = npcName;
            other.GetComponent<PlayerInteraction>().jobName = jobName;
            other.GetComponent<PlayerInteraction>().canInteractWithNPC = true;
            Vector3 targetPosition = other.transform.position;
            targetPosition.y = transform.position.y;
            transform.DOLookAt(targetPosition, 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInteraction>().currentDialogue = null;
            other.GetComponent<PlayerInteraction>().npcName = "";
            other.GetComponent<PlayerInteraction>().canInteractWithNPC = false;
        }
    }
}
