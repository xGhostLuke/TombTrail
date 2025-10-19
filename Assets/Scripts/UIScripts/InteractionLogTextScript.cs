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
    private void Awake() {
        logText = "";
        RefreshTextInUI();
        HideWindow();
    }

    public void AddTextToLog(string addText) {
        logText = logText + "\n" + addText;
        CheckTextLength();
        RefreshTextInUI();
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
