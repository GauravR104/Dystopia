using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerControls controls;
    public bool canInteractWithNPC = false;
    private bool canInteractWithJob = false;
    public Dialogue currentDialogue;
    public string npcName;
  //  public SceneLoader sceneLoader;

    private string jobName;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.UI.Select.performed += x => InteractWithNPC();
        controls.UI.Select.performed += x => InteractWithJob();

    }
    private void OnEnable()
    {
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Disable();
    }

    public void InteractWithNPC()
    {
        if (canInteractWithNPC)
        {
            //SoundManager.ins.PlaySfx("Accept");
            //UIManager.ins.SetMessageObject(false, "", "");
            DialogueManager.instance.StartDialogue(currentDialogue, npcName);
          //  GetComponent<PlayerMovement>().canMove = false;
            canInteractWithNPC = false;
        }
    }

    public void InteractWithJob()
    {
        if (canInteractWithJob)
        {
          //  if (SoundManager.ins != null)
            //{
               // SoundManager.ins.PlaySfx("Accept");
           // }

            //sceneLoader.LoadJobScene(jobName);
            canInteractWithJob = false;
        }
    }


    public void CanIntertactWithJob(bool interact, string job)
    {
        canInteractWithJob = interact;
        jobName = job;
    }
}
