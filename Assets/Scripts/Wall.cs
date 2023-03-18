using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Wall : MonoBehaviour
{
    public ParticleSystem effect;
    public Transform effectPos;
    public List<Rigidbody> parts;
    public TextMeshPro text;
    public TextMeshPro text2;
    public Transform forcePos;
    void Explosion() { 
        text.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
        for (int i = 0; i < parts.Count; i++) {
            var rb = parts[i];
            rb.isKinematic = false;
            Vector3 direction = rb.transform.position - forcePos.position;
            rb.AddForceAtPosition(direction.normalized * Random.Range(150,500   ), forcePos.position);
            parts[i].gameObject.AddComponent<WallParts>();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("FinishBullet")) {
            Explosion();
            other.gameObject.SetActive(false);
            var _instantiatedObj = Instantiate(effect, effectPos.position, effectPos.rotation);
            Destroy(_instantiatedObj.gameObject, 2f);
        }
    }
}
