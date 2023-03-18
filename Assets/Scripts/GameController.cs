using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {
    public FinishManager finishManager;
    [HideInInspector] public Player player;

    public int requiedScore;
    private void Awake() {
        player = Player.instance;
    }
    private void Start() {
        finishManager.Init(this);
    }
    [Serializable]
    public class FinishManager {
        GameController controller;
        public float finishDelayTime;
        public void Init(GameController c) {
            controller = c;
        }
        //public void Finish(bool input) {
        //    if (input) {
        //        controller.player.state = Player.State.WellFinish;
        //    } else {
        //        controller.player.state = Player.State.FailFinish;
        //    }
        //    controller.StartCoroutine(StopTheGame(finishDelayTime));
        //}
        public IEnumerator StopTheGame(float finishDelayTime) {
            yield return new WaitForSeconds(finishDelayTime);
            Time.timeScale = 0;
        }
    }
}



        

