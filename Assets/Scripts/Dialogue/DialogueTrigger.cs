using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public int npcIndex;
    public string npcName;

    GameObject player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
           /* UIManager.ins.SetMessageObject(true,npcName, "Talk");
            other.GetComponent<PlayerInteraction>().currentDialogue = dialogue;
            other.GetComponent<PlayerInteraction>().npcName = npcName;
            other.GetComponent<PlayerInteraction>().canInteract = true;
            GameManager.Instance.npcIndex = npcIndex;*/

            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y;
            transform.DOLookAt(targetPosition, 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           /* UIManager.ins.SetMessageObject(false,"","");
            other.GetComponent<PlayerInteraction>().currentDialogue = null;
            other.GetComponent<PlayerInteraction>().npcName = "";
            other.GetComponent<PlayerInteraction>().canInteract = false;
            GameManager.Instance.npcIndex = 0;*/
        }
    }

}
