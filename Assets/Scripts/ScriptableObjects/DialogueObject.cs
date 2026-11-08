using System;

using Scripts;

using UnityEngine;

/// <summary>
/// One answer the player can pick in a conversation.
/// </summary>
[Serializable]
public class DialogueAnswer {
    public string text;
    [Tooltip("Name of the node to continue with. Empty ends the conversation.")]
    public string next;
    public DialogueAction action;
}

/// <summary>
/// One step of a conversation. Without answers the window offers a plain "Leave".
/// </summary>
[Serializable]
public class DialogueNode {
    public string name;
    [TextArea] public string text;
    public DialogueAnswer[] answers = Array.Empty<DialogueAnswer>();
}

/// <summary>
/// A conversation as a list of named nodes. It holds only text: the progress lives
/// in the Quest of the scene, so playing never changes this asset.
/// </summary>
[CreateAssetMenu(fileName = "DialogueObject", menuName = "Scriptable Objects/DialogueObject")]
public class DialogueObject : ScriptableObject {
    public DialogueNode[] nodes = Array.Empty<DialogueNode>();

    public DialogueNode FindNode(string nodeName) {
        return Array.Find(nodes, node => node.name == nodeName);
    }

    // Unity calls this on every change in the Inspector, so a mistyped name shows up before playing.
    private void OnValidate() {
        foreach (DialogueNode node in nodes) {
            foreach (DialogueAnswer answer in node.answers) {
                if (!string.IsNullOrEmpty(answer.next) && FindNode(answer.next) == null) {
                    Debug.LogError(name + ": node '" + node.name + "' points to unknown node '" + answer.next + "'.", this);
                }
            }
        }
    }
}
