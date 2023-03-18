using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public ScoreManager scoreManager;
    public SliderManager sliderManager;
    public TapToPlayModule tapToPlayModule;
    public PanelModule panelModule;
    Player player;
    private void Start() {
        player = Player.instance;
        scoreManager.Init(this);
        sliderManager.Init(this);
        tapToPlayModule.Init(this);
        panelModule.Init(this);
    }
    private void Update() {
        sliderManager.Update();
        tapToPlayModule.Update();
        panelModule.Update();
    }
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Serializable]
    public class ScoreManager {
        UIManager uIManager;
        public int score;
        public TextMeshPro scoreText;
        bool hundred = false;
        bool control = false;
        public void Init(UIManager u) {
            uIManager = u;
            UpdateScore();
            score = 0;
        }
        public void IncreaseScore(int amount) {
            score += amount;
            UpdateScore();
        }
        public void UpdateScore() {
            scoreText.text = score.ToString();
            if (score >= 10 && !control) {
                scoreText.transform.position += new Vector3(-0.10f, 0, 0);
                control = true;
            }
            if (score >= 100 && !hundred) {
                scoreText.transform.position += new Vector3(-0.10f, 0, 0);
                hundred = true;
            }
        }
        public void TextSetActive(bool input) {
            if (scoreText!=null) {
                scoreText.gameObject.SetActive(input);
            }
         
        }
    }

    [Serializable]
    public class SliderManager {
        public UIManager UIManager;
        public Slider slider;
        public float lerpSpeed;
        public void Init(UIManager u) {
            UIManager = u;
            slider.maxValue = UIManager.player.gameController.requiedScore;
            slider.minValue = 0;
        }
        public void Update() {
            var targetValue = UIManager.scoreManager.score;
            slider.value = Mathf.Lerp(slider.value, targetValue, lerpSpeed);
        }
    }

    [Serializable]
    public class TapToPlayModule {
        public GameObject tapToPlayPanel;
        UIManager UIManager;
        public void SetActiveFalse() {
            tapToPlayPanel.SetActive(false);
        }
        public void Init(UIManager u) {
            UIManager = u;
        }
        public void Update() {
            if (UIManager.player.state == Player.State.Fire) {
                tapToPlayPanel.SetActive(false);
            }
        }
    }
    [Serializable]
    public class PanelModule {
        public GameObject restartPanel;
        public GameObject winPanel;
        UIManager UIManager;
        public float failFinishDelay;
        public float crashDelay;
        public float winDelay;

        public TextMeshProUGUI howManyX;
        public void Init(UIManager u) {
            UIManager = u;
        }
        public IEnumerator Restart(float delay) {
            yield return new WaitForSeconds(delay);
            restartPanel.SetActive(true);
        }
        public IEnumerator Win(float delay) {
            yield return new WaitForSeconds(delay);
            winPanel.SetActive(true);
        }
        public void Update() {
            if (UIManager.player.state == Player.State.Failed) {
                UIManager.StartCoroutine(Restart(failFinishDelay));
                howManyX.text = UIManager.player.fireModule.finalShootCount.ToString();
            } else if (UIManager.player.state == Player.State.Crash) {
                UIManager.StartCoroutine(Restart(crashDelay));
            }
            if (UIManager.player.state == Player.State.Success) {
                howManyX.text = UIManager.player.fireModule.finalShootCount.ToString();
                UIManager.StartCoroutine(Win(winDelay));
            }
        }

    }
}
