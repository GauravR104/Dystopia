using DG.Tweening;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    public string npcName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("Can Talk");

            other.GetComponent<PlayerInteraction>().currentDialogue = dialogue;
            other.GetComponent<PlayerInteraction>().npcName = npcName;
            other.GetComponent<PlayerInteraction>().canInteractWithNPC = true;

            /* UIManager.ins.SetMessageObject(true,npcName, "Talk");
             
             GameManager.Instance.npcIndex = npcIndex;*/

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
            /* UIManager.ins.SetMessageObject(false,"","");
            
             GameManager.Instance.npcIndex = 0;*/
        }
    }
}
