using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;



public class InfoTabsScript : MonoBehaviour {
    private EntityInfoTabScript entityInfoTabScript;
    [SerializeField] private InteractionLogTextScript interactionlog;

    private void Awake() {
        entityInfoTabScript = GameObject.FindGameObjectWithTag("EnemyInfoTab").GetComponent<EntityInfoTabScript>();
        interactionlog = GameObject.FindGameObjectWithTag("Interactionlog").GetComponent<InteractionLogTextScript>();
    }

    public void RefreshEnemyInfo(EntityController entity) {
        if (entity != null) {
            entityInfoTabScript.LoadStats(
                entity.GetEntityName(),
                entity.GetHealth().ToString(),
                entity.GetAttackDamage().ToString(),
                entity.GetMaxHealth().ToString(),
                entity.getStatus().ToString()
            );
        }
    }

    public void ShowInteractionlog() {
        interactionlog.ShowWindow();
    }


    public void StandardLogMessage(String message) {
        interactionlog.AddTextToLog(message);
    }

    public void HitLogMessage(EntityController attacker, EntityController defender) {
        StandardLogMessage(attacker.GetEntityName() + " attacked " + defender.GetEntityName() + " for " + attacker.GetAttackDamage() + " DMG!");
    }
}
