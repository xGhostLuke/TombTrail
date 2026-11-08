using System;
using System.Linq;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

public class InteractionLogTextScript : WindowController {
    private string logText;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject interactionlogWindow;
    [SerializeField] private GameObject interactionlogButton;
    [SerializeField] private int maxVisibleLines = 26;

    private void Awake() {
        logText = "";
        RefreshTextInUI();
        HideWindow();
    }

    public void AddTextToLog(string addText) {
        logText = logText + "\n" + addText;
        CheckTextLength();
        RefreshTextInUI();
        RemoveLinesBelowWindow();
    }

    /// <summary>
    /// The window holds lines, not characters: long entries wrap, so without this
    /// the newest text could end up below the window where nobody can read it.
    /// </summary>
    private void RemoveLinesBelowWindow() {
        text.ForceMeshUpdate();
        while (text.textInfo.lineCount > maxVisibleLines && logText.Contains("\n")) {
            logText = logText.Substring(logText.IndexOf("\n") + 1);
            RefreshTextInUI();
            text.ForceMeshUpdate();
        }
    }

    private void CheckTextLength() {
        if (logText.Length > 764) {
            int firstLineEnds = logText.IndexOf("\n");
            logText = logText.Substring(firstLineEnds + 1);
        }
    }

    private void RefreshTextInUI() {
        text.text = logText;
    }
}
