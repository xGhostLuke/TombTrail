using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Shows a conversation and reports the chosen answers, the quest rules live in NpcController.
/// The root covers the whole screen and catches every click, so picking up or dropping
/// items is impossible during a talk even though those clicks do not ask whose turn it is.
/// </summary>
public class DialogueWindowScript : WindowController {
    private const int NumberKeys = 9;

    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button answerTemplate;
    [SerializeField] private string leaveLabel = "Leave";

    private readonly List<GameObject> answerButtons = new List<GameObject>();
    private readonly List<Action> choices = new List<Action>();
    private DialogueObject dialogue;
    private Func<DialogueAnswer, string> resolveAnswer;
    private bool isOpen;
    private bool canLeaveWithKey;
    private int lastToggleFrame = -1;

    private void Awake() {
        HideWindow();
    }

    private void Update() {
        // The frame check stops the E that opened the window from also closing it.
        if (!isOpen || Time.frameCount == lastToggleFrame) {
            return;
        }
        Keyboard keyboard = Keyboard.current;
        for (int i = 0; i < Mathf.Min(choices.Count, NumberKeys); i++) {
            if (keyboard[Key.Digit1 + i].wasPressedThisFrame) {
                choices[i]();
                return;
            }
        }
        // Only plain text can be left with E. An offer needs a real choice, so nobody accepts one by tapping E twice.
        if (canLeaveWithKey && ( keyboard.eKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame )) {
            Close();
        }
    }

    /// <summary>
    /// True during a conversation and in the frame it ends, so the E that closes the
    /// window cannot start the same conversation again.
    /// </summary>
    public bool BlocksPlayer() {
        return isOpen || Time.frameCount == lastToggleFrame;
    }

    /// <summary>
    /// Opens a conversation.
    /// </summary>
    /// <param name="answerHandler">Carries out an answer and returns the node to continue with, empty to end</param>
    public void Open(string speaker, DialogueObject conversation, string startNode, Func<DialogueAnswer, string> answerHandler) {
        dialogue = conversation;
        resolveAnswer = answerHandler;
        speakerText.text = speaker;
        isOpen = true;
        lastToggleFrame = Time.frameCount;
        ShowWindow();
        ShowNode(startNode);
    }

    private void ShowNode(string nodeName) {
        DialogueNode node = dialogue.FindNode(nodeName);
        if (node == null) {
            Debug.LogError(dialogue.name + " has no node '" + nodeName + "'.", dialogue);
            Close();
            return;
        }
        bodyText.text = node.text;
        ClearAnswers();
        canLeaveWithKey = node.answers.Length == 0;
        if (canLeaveWithKey) {
            AddAnswer(leaveLabel, Close);
            return;
        }
        foreach (DialogueAnswer answer in node.answers) {
            AddAnswer(answer.text, () => Choose(answer));
        }
    }

    private void Choose(DialogueAnswer answer) {
        string next = resolveAnswer(answer);
        if (string.IsNullOrEmpty(next)) {
            Close();
        } else {
            ShowNode(next);
        }
    }

    private void AddAnswer(string label, Action onChosen) {
        Button button = Instantiate(answerTemplate, answerTemplate.transform.parent);
        button.gameObject.SetActive(true);
        // The number tells keyboard players which key picks this answer.
        button.GetComponentInChildren<TextMeshProUGUI>().text = ( choices.Count + 1 ) + ". " + label;
        button.onClick.AddListener(() => onChosen());
        answerButtons.Add(button.gameObject);
        choices.Add(onChosen);
    }

    private void ClearAnswers() {
        foreach (GameObject button in answerButtons) {
            Destroy(button);
        }
        answerButtons.Clear();
        choices.Clear();
    }

    private void Close() {
        isOpen = false;
        lastToggleFrame = Time.frameCount;
        ClearAnswers();
        HideWindow();
    }
}
