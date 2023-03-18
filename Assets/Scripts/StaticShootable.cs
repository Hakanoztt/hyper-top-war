using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticShootable : Shootable {
    bool buffed;
    public new void Awake() {
        base.Awake();
        buffed = false;
    }

    private void Update() {
        DeathBuff();
    }
    public override void DeathBuff() {
        if (!health.isAlive && !buffed) {
            buffed = true;
            uIManager.scoreManager.IncreaseScore(health.maxHealth);
            SetActive(false);
        }
    }


}
