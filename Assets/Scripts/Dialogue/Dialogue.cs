using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueNode[] nodes;

    [System.Serializable]
    public class DialogueNode
    {
        public string id;
        public string text;
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public string text;
        public DialogueType dialogueType;
        public string nextNodeId;   
    }

    public DialogueNode FindNodeById(string nodeId)
    {
        foreach (DialogueNode node in nodes)
        {
            if (node.id == nodeId)
            {
                return node;
            }
        }
        return null;
    }
}

public enum DialogueType
{
    CloseDialogue,
    NextDialogue,
    StartJob
};
