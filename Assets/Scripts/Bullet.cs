using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float bulletSpeed;
    public float destroyTime;
    Player player;
    public enum State { forward, leftCross, RightCross }
    public State state;
    private void Awake() {
        player = Player.instance;
    }
    void Update() {
        Movement();
        StartCoroutine(Destroy(destroyTime));
    }
    IEnumerator Destroy(float destroyTime) {
        yield return new WaitForSeconds(destroyTime);
        gameObject.SetActive(false);
        gameObject.transform.position = player.fireModule.bulletStartPos.position;
    }
    void Movement() {
        switch (state) {
            case State.forward:
                transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
                break;
            case State.RightCross:
                var dir = new Vector3(0.1f, 0, 1);
                transform.Translate(dir.normalized * bulletSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(dir.normalized);
                break;
            case State.leftCross:
                var dir2 = new Vector3(-0.1f, 0, 1);
                transform.Translate(dir2.normalized * bulletSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(dir2.normalized);
                break;
            default:
                break;
        }
    }

    void LookAt() {
        transform.LookAt(transform.forward);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Shootable")) {
            var shootable = other.GetComponent<Shootable>();
            shootable.TakeDamage(shootable.damageAmount);
            gameObject.SetActive(false);
        }
    }

}
