using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpeedShootable : Shootable {
    public float reduceAmount;
    bool buffed = false;
    private new void Awake() {
        base.Awake();
    }
    private void Update() {
        DeathBuff();
    }
    public override void DeathBuff() {
        if (!health.isAlive && !buffed) {
            player.fireModule.ReduceFireDelay(reduceAmount);
            uIManager.scoreManager.IncreaseScore(health.maxHealth);
            for (int i = 0; i < player.stackManager.playerCrowd.Count; i++) {
                player.stackManager.playerCrowd[i].fireModule.ReduceFireDelay(reduceAmount);
            }
            buffed = true;
            SetActive(false);
        }
    }
}
