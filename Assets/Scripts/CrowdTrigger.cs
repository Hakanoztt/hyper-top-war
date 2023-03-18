using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrowdTrigger : MonoBehaviour {
    public TextMeshProUGUI text;
    public int multiplyNum;
    Player player;
    public ParticleSystem effect;
    void Start() {
        player = Player.instance;
        text.text = "x" + multiplyNum.ToString();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            var crowdCount = player.stackManager.playerCrowd.Count + 1;
            for (int i = 0; i < (multiplyNum * crowdCount) - crowdCount; i++) {
                player.stackManager.Add();
            }
            var _instantiatedObj = Instantiate(effect, player.transform.position, player.transform.rotation);
            Destroy(_instantiatedObj, 2f);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.SetActive(false);

        }

    }
}
