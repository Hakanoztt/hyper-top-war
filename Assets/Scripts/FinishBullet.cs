using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishBullet : MonoBehaviour {
    public float bulletSpeed;
    void Update() {
        Movement();
        ScaleUp();
    }

    void Movement() {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }
    void ScaleUp() {
        transform.localScale += new Vector3(2f, 2f, 2f);
    }
}
