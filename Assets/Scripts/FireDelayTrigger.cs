using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireDelayTrigger : MonoBehaviour {
    Player player;
    public float reducedFireDelay;
    public float moveSpeed;
    public bool isMoving;
    public ParticleSystem effect;
    float dir;
    void Start() {
        dir = 1;
        player = Player.instance;
    }
    private void Update() {
        Movement();
    }
    void Movement() {
        if (isMoving) {
            transform.Translate(Vector3.left * dir * moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            player.fireModule.ReduceFireDelay(reducedFireDelay);
            for (int i = 0; i < player.stackManager.playerCrowd.Count; i++) {
                player.stackManager.playerCrowd[i].fireModule.ReduceFireDelay(reducedFireDelay);
            }
            var _instantiatedObj = Instantiate(effect, player.transform.position, player.transform.rotation);
            Destroy(_instantiatedObj, 2f);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.SetActive(false);

        }
        if (other.CompareTag("EndOfTrigger")) {
            dir = -1;
        }
        if (other.CompareTag("HeadOfTrigger")) {
            dir = 1;
        }
    }
}
