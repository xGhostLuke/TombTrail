using System;
using System.Runtime.CompilerServices;

using TMPro;

using Unity.VisualScripting;

using UnityEditor.Rendering.Canvas.ShaderGraph;

using UnityEngine;


public class EntityInfoTabScript : WindowController {



    [SerializeField] private TextMeshProUGUI EntityDamageText;
    [SerializeField] private TextMeshProUGUI EntityHealthText;
    [SerializeField] private TextMeshProUGUI EntityNameText;
    [SerializeField] private TextMeshProUGUI EntityStatusText;

    private void Awake() {
        HideWindow();
    }

    public void LoadStats(string name, string health, string damage, string maxhealth, string status) {
        EntityNameText.text = name;
        EntityDamageText.text = damage;
        EntityHealthText.text = health + " / " + maxhealth;
        EntityStatusText.text = status;
    }

    public void ShowWindow(string name, string health, string damage, string maxhealth, string status) {
        SetActive(true);
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        LoadStats(name, health, damage, maxhealth, status);
    }
}
