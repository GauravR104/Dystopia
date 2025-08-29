using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static Dialogue;
using DG.Tweening;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public TMP_FontAsset choiceFont;
    public TMP_FontAsset withoutChoiceFont;

    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject[] choiceButtons;
    private Dialogue.DialogueNode currentNode;
    public static DialogueManager instance;
    public Animator animator;
    public PlayerControls controls;

    public PlayerMovement player;

    private Dialogue dialogue;

    public Button nextButton;

    public GameObject dialoguePanel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void StartDialogue(Dialogue dialogue, string name="")
    {
        this.dialogue = dialogue;
        nameText.text = name;
        currentNode = dialogue.nodes[0];
        //animator.SetBool("IsOpen", true);
        dialoguePanel.SetActive(true);
        DisplayCurrentNode(currentNode);

        //GameManager.Instance.playerAction = PlayerDialogueAction.PlayerStartTalking;
     
    }

    public void SelectChoice(int choiceIndex)
    {
        Dialogue.Choice selectedChoice = currentNode.choices[choiceIndex];

        switch(selectedChoice.dialogueType)
        {
            case DialogueType.NextDialogue:
                EndDialogue();
                break;
            case DialogueType.CloseDialogue:
                currentNode = dialogue.FindNodeById(selectedChoice.nextNodeId);
                choiceButtons[0].transform.parent.gameObject.SetActive(false);
                DisplayCurrentNode(currentNode);
                break;
            case DialogueType.StartJob:
                break;
        }

        if (selectedChoice.closesDialogue)
        {
            EndDialogue();
        }
        else if (selectedChoice.movesToNextNode)
        {
            currentNode = dialogue.FindNodeById(selectedChoice.nextNodeId);
            choiceButtons[0].transform.parent.gameObject.SetActive(false);
            DisplayCurrentNode(currentNode);
        }
        
    }

    private void DisplayCurrentNode(DialogueNode dialogueNode)
    {
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(DisplayTextRoutine(dialogueNode));    
    }

    IEnumerator DisplayTextRoutine(DialogueNode dialogueNode)
    {
        dialogueText.text = "";

        yield return new WaitForSeconds(1f);
        if(currentNode.choices.Length == 0)
        {
          //  dialogueText.font = withoutChoiceFont;
        }
        else
        {
            //dialogueText.font = choiceFont;
        }
        foreach (char letter in dialogueNode.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.05f);

        if (currentNode.choices.Length == 0)
        {
            choiceButtons[0].transform.parent.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);

            EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        }
        else
        {
            choiceButtons[0].transform.parent.gameObject.SetActive(true);
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < dialogueNode.choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TMP_Text>().text = dialogueNode.choices[i].text;
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }

            StartCoroutine(EnableButtonSelection());
        }
    }

    IEnumerator EnableButtonSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForSeconds(1f);
        EventSystem.current.SetSelectedGameObject(choiceButtons[0]);
    }

    private void EndDialogue()
    {
        // animator.SetBool("IsOpen", false);

        dialoguePanel.SetActive(false);
        //player.canMove = true;

        dialogueText.text = "";

        if (choiceButtons[0].transform.parent.gameObject.activeSelf)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(false);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = "";
            }
        }

       


        EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseDialogue()
    {
        nextButton.gameObject.SetActive(false);
        EndDialogue();
    }
}

