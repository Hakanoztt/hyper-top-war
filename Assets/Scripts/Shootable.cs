using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public interface ITakeDamage {
    public void TakeDamage(int i);
}
[Serializable]
public struct Health {
    public int CurrentHealth => _currentHealth;
    private int _currentHealth;
    public int maxHealth;
    bool _swipeLefttoText;

    public TMP_Text healthText;
    public bool isAlive => _currentHealth > 0;
    public void Init() {
        _swipeLefttoText = false;
        _currentHealth = maxHealth;
        UpdateHealth();
    }
    public void TakeDamage(int damage) {
        if (isAlive) {
            _currentHealth -= damage;
        }
        UpdateHealth();
    }
    public void UpdateHealth() {
        if (healthText != null) {
            healthText.text = _currentHealth.ToString();
        }
        if (_currentHealth >= 100 && !_swipeLefttoText) {
            healthText.transform.position += new Vector3(-0.20f, 0, 0);
            _swipeLefttoText = true;
        } else if (_currentHealth < 100 && _swipeLefttoText) {
            healthText.transform.position += new Vector3(0.20f, 0, 0);
            _swipeLefttoText = false;
        }
    }
}
public abstract class Shootable : MonoBehaviour, ITakeDamage {
    public Health health;
    public int damageAmount;
    public Player player;
    public UIManager uIManager;
    public ParticleSystem shootEffect;
    public abstract void DeathBuff();
    public virtual void Awake() {
        health.Init();
    }
    public virtual void TakeDamage(int i) {
        health.TakeDamage(i);
        var _instantiatedObj = Instantiate(shootEffect, transform.position + new Vector3(0, 1f, -1.25f), transform.rotation);
        Destroy(_instantiatedObj.gameObject,2f);
    }
    public virtual void SetActive(bool input) {
        gameObject.SetActive(input);
        //Effect
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            player.movementModule.moveSpeed = 0;
            player.state = Player.State.Crash;
            var crowd = player.stackManager.playerCrowd;
            for (int i = 0; i < crowd.Count; i++) {
                crowd[i].state = Player.State.Crash;
            }
        }
    }


}
