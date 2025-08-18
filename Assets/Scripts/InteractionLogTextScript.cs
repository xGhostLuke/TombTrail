using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionLogTextScript : MonoBehaviour {
    private string logText;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake() {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        logText = "";
        RefreshTextInUI();
    }

    public void AddTextToLog(string addText) {
        logText = logText + "\n" + addText;
        CheckTextLength();
        RefreshTextInUI();
    }

    private void CheckTextLength() {
        if (logText.Length > 1100) {
            int firstLineEnds = logText.IndexOf("\n");
            logText = logText.Substring(firstLineEnds + 1);
        }
    }

    private void RefreshTextInUI() {
        text.text = logText;
    }
}
