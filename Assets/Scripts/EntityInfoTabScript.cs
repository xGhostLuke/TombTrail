using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class EntityInfoTabScript : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI EnemyDamageText;
    [SerializeField] private TextMeshProUGUI EnemyHealthText;
    [SerializeField] private TextMeshProUGUI EnemyNameText;
    private GameObject EnemyInfoWindow;


    private void Awake() {
        EnemyInfoWindow = GameObject.FindGameObjectWithTag("EnemyInfoTab");
    }
    private void Start() {
        EnemyInfoWindow.SetActive(false);
    }

    // Update is called once per frame
    private void Update() {

    }

    private void LoadStats(string name, string health, string damage, string maxhealth) {
        EnemyNameText.text = name;
        EnemyDamageText.text = damage;
        EnemyHealthText.text = maxhealth + " / " + health;
    }

    public void ShowWindow(string name, string health, string damage, string maxhealth) {
        EnemyInfoWindow.SetActive(true);
        LoadStats(name, health, damage, maxhealth);
    }

    public void HideWindow() {
        EnemyInfoWindow.SetActive(false);
    }
}
