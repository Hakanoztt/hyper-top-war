using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour {
    public static Player instance;

    public MovementModule movementModule;
    public FireModule fireModule;
    public StackManager stackManager;
    public AnimationModule animationModule;
    public StateControl stateControl;
    public PlayerLevelModule playerLevelModule;

    public UIManager uIManager;
    public GameController gameController;

    public Cam cam;
    public enum State { Idle, Fire, Crash, Crashed, GoingFinish, Finish, Failed, Success };
    public State state;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    void Start() {
        movementModule.Init(this);
        fireModule.Init(this);
        stackManager.Init(this);
        animationModule.Init(this);
        stateControl.Init(this);
        playerLevelModule.Init(this);
    }
    void Update() {
        movementModule.Update();
        stateControl.Update();
        playerLevelModule.Update();
        fireModule.Update();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("StopFireTrigger")) {
            animationModule.SetBool("NormalRun", true);
            state = State.GoingFinish;
        }
        if (other.CompareTag("FinishLine")) {
            animationModule.SetBool("NormalRun", false);
            animationModule.SetBool("isMoving", false);
        }
    }
    [Serializable]
    public class FireModule {
        Player player;
        public List<Bullet> bulletList;
        public List<Transform> weapons;
        public Bullet bulletPrefab;
        public Transform bulletStartPos;
        public Transform finishBulletStartPos;
        public int bulletAmountAtStart;
        public float fireDelay;
        AudioSource audioSource;

        public float finishFireSpeed;

        public int finalShootCount;

        public FinishBullet finishBullet;
        public enum GunType { oneGun, doubleGun, quadGun };
        public GunType gunType;
        public void Init(Player p) {
            player = p;
            bulletList = new List<Bullet>();
            audioSource = player.GetComponent<AudioSource>();

            for (int i = 0; i < bulletAmountAtStart; i++) {
                Bullet newBullet = Instantiate(bulletPrefab);
                bulletList.Add(newBullet);
                newBullet.gameObject.SetActive(false);
            }
            player.StartCoroutine(Shoot());
        }
        public IEnumerator Shoot() {
            while (true) {
                yield return new WaitForSeconds(fireDelay);
                Fire();
            }
        }
        public void Fire() {

            if (player.state == State.Fire) {
                audioSource.Play();
                switch (gunType) {
                    case GunType.oneGun:
                        Bullet bullet = GetBulletFromPool();
                        bullet.transform.position = weapons[0].transform.position;
                        bullet.gameObject.SetActive(true);
                        break;
                    case GunType.doubleGun:
                        for (int i = 0; i < 2; i++) {
                            Bullet newBullet = GetBulletFromPool();
                            newBullet.transform.position = weapons[i].transform.position;
                            newBullet.gameObject.SetActive(true);
                        }
                        break;
                    case GunType.quadGun:
                        for (int i = 0; i < 4; i++) {
                            Bullet newBullet2 = GetBulletFromPool();
                            newBullet2.transform.position = weapons[i].transform.position;
                            newBullet2.gameObject.SetActive(true);
                            newBullet2.state = Bullet.State.forward;
                            if (i == 2) {
                                newBullet2.state = Bullet.State.RightCross;
                            } else if (i == 3) {
                                newBullet2.state = Bullet.State.leftCross;
                            }
                        }
                        break;
                }
            }
        }
        public void ChangeGunType() {
            if (gunType == GunType.oneGun) {
                gunType = GunType.doubleGun;
            } else if (gunType == GunType.doubleGun) {
                gunType = GunType.quadGun;
            }
        }
        public Bullet GetBulletFromPool() {
            for (int i = 0; i < bulletList.Count; i++) {
                if (!bulletList[i].gameObject.activeSelf) {
                    return bulletList[i];
                }
            }
            Bullet newBullet = Instantiate(bulletPrefab);
            bulletList.Add(newBullet);
            newBullet.gameObject.SetActive(false);
            return newBullet;
        }
        public void ReduceFireDelay(float f) {
            fireDelay -= f;
        }
        public IEnumerator FinishFire() {
            player.animationModule.SetBool("Shooting", true);
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < finalShootCount; i++) {
                player.animationModule.AnimatorSpeedUp(0.01f);
                Instantiate(finishBullet, finishBulletStartPos.position, Quaternion.identity);
                finishFireSpeed -= 0.015f;
                yield return new WaitForSeconds(finishFireSpeed);
            }
            if (finalShootCount >= Mathf.RoundToInt(player.gameController.requiedScore / 100)) {
                player.state = State.Success;
            } else {
                player.state = State.Failed;
            }
        }
        public void Update() {
            finalShootCount = Mathf.RoundToInt(player.uIManager.scoreManager.score / 100);
        }
    }
    [Serializable]
    public class MovementModule {
        Player player;
        public float moveSpeed;
        public float sideSpeed;

        public float minX;
        public float maxX;
        public bool tapToPlay = false;
        float _lastFrameFingerPositionX;
        float _moveFactorX;
        public void Init(Player p) {
            player = p;
        }
        public void Update() {
            SwerweMovement();
        }
        public void SwerweMovement() {
            if (Input.GetMouseButtonDown(0) && player.state == State.Idle) {
                player.state = State.Fire;
            }
            if (player.state == State.Fire) {
                player.transform.Translate(new Vector3(_moveFactorX * Time.deltaTime * sideSpeed, 0, moveSpeed * Time.deltaTime));
                if (Input.GetMouseButtonDown(0)) {
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                } else if (Input.GetMouseButton(0)) {
                    _moveFactorX = Input.mousePosition.x - _lastFrameFingerPositionX;
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                }
                if (Input.GetMouseButtonUp(0)) {
                    _moveFactorX = 0;
                }
                // moveFactorX = Mathf.Clamp(moveFactorX,  minX, maxX); ??
            }
        }

    }
    [Serializable]
    public class StackManager {
        public List<Player> playerCrowd;
        public List<Transform> spawnPoints;
        public Player PlayerPrefab;
        Player player;
        public void Init(Player p) {
            player = p;
            playerCrowd = new List<Player>();
        }
        public void Add() {
            var lastShooterIndex = playerCrowd.Count - 1;
            var position = spawnPoints[lastShooterIndex + 1].position;
            Player newPlayer = Instantiate(PlayerPrefab, position, Quaternion.identity, player.transform);
            newPlayer.fireModule.fireDelay = player.fireModule.fireDelay;
            newPlayer.fireModule.gunType = player.fireModule.gunType;
            newPlayer.state = State.Fire;
            playerCrowd.Add(newPlayer);
            player.cam.ZoomOutCamera();
        }
    }
    [Serializable]
    public class AnimationModule {
        public Animator animator;
        Player player;
        public void Init(Player p) {
            player = p;
            if (player.state != State.Fire) {
                player.state = State.Idle;
            }
        }
        public void SetBool(string anim, bool input) {
            animator.SetBool(anim, input);
            var crowd = player.stackManager.playerCrowd;
            for (int i = 0; i < crowd.Count; i++) {
                crowd[i].animationModule.SetBool(anim, input);
            }
        }
        public void AnimatorSpeedUp(float f) {
            animator.speed += f;
        }
        public IEnumerator SetBool(string anim, bool input, float delay) {
            yield return new WaitForSeconds(delay);
            animator.SetBool(anim, input);
        }
    }

    [Serializable]
    public class StateControl {
        Player player;
        public Transform finishPos;
        bool control = false;
        public void Init(Player p) {
            player = p;
        }
        public void Update() {
            ControlState();
        }
        void ControlState() {
            switch (player.state) {
                case State.Idle:
                    break;
                case State.Fire:
                    player.animationModule.SetBool("isMoving", true);
                    break;
                case State.Crash:
                    player.animationModule.SetBool("BoxTrigger", true);
                    var crowd = player.stackManager.playerCrowd;
                    for (int i = 0; i < crowd.Count; i++) {
                        crowd[i].animationModule.SetBool("BoxTrigger", true);
                        if (crowd[i].transform.localScale.z > 0) {
                            crowd[i].transform.localScale -= new Vector3(0.01f, 0, 0.01f);
                        } else {
                            crowd[i].gameObject.SetActive(false);
                        }
                    }
                    if (player.cam != null) {
                        player.cam.CrashZoomOut();
                    }
                    player.uIManager.scoreManager.TextSetActive(false);

                    for (int i = 0; i < crowd.Count; i++) {
                        if (!crowd[i].gameObject.activeSelf) {
                            player.state = State.Crashed;
                        }
                    }
                    break;
                case State.Crashed:
                    break;
                case State.GoingFinish:
                    if (finishPos != null) {
                        crowd = player.stackManager.playerCrowd;
                        for (int i = 0; i < crowd.Count; i++) {
                            crowd[i].GetComponent<Player>().animationModule.SetBool("Idle", true);
                            if (crowd[i].transform.localScale.z > 0) {
                                crowd[i].transform.localScale -= new Vector3(0.020f, 0.0010f, 0.020f);
                            } else {
                                crowd[i].gameObject.SetActive(false);
                            }
                        }

                        player.transform.position = Vector3.MoveTowards(player.transform.position, finishPos.transform.position, (player.movementModule.moveSpeed * Time.deltaTime));
                        if (player.transform.position == finishPos.position) {
                            player.state = State.Finish;
                        }
                    }
                    break;
                case State.Finish:
                    player.uIManager.scoreManager.TextSetActive(false);
                    if (player.gameController.requiedScore > player.uIManager.scoreManager.score) {
                        player.state = State.Failed;
                        control = true;
                    }
                    if (player.fireModule.finishBullet != null && !control) {
                        player.StartCoroutine(player.fireModule.FinishFire());
                        control = true;
                    }

                    break;
                case State.Failed:
                    player.animationModule.SetBool("Lose", true);
                    crowd = player.stackManager.playerCrowd;
                    for (int i = 0; i < crowd.Count; i++) {
                        crowd[i].GetComponent<Player>().animationModule.SetBool("Idle", true);
                        if (crowd[i].transform.localScale.z > 0) {
                            crowd[i].transform.localScale -= new Vector3(0.010f, 0.005f, 0.010f);
                        } else {
                            crowd[i].gameObject.SetActive(false);
                        }
                    }
                    break;
                case State.Success:
                    player.animationModule.SetBool("Victory", true);
                    break;
                default:
                    break;
            }
        }
    }
    [Serializable]
    public class PlayerLevelModule {
        Player player;

        public ParticleSystem transformationEffect;

        public int requiedScoreForLevel2;
        public int requiedScoreForLevel3;
        public List<GameObject> levelSkins;
        public float reduceAmount;
        public enum Level { Level1, Level2, Level3 };
        public Level level;
        bool changed = false;

        private int _requiedScore;
        public void Init(Player p) {
            player = p;
            level = Level.Level1;
            _requiedScore = requiedScoreForLevel2;
        }
        public void Update() {
            LevelControl();
            LevelUp();
        }
        public void LevelControl() {
            switch (level) {
                case Level.Level1:
                    break;
                case Level.Level2:
                    player.animationModule.animator = levelSkins[1].GetComponent<Animator>();
                    levelSkins[0].gameObject.SetActive(false);
                    levelSkins[1].gameObject.SetActive(true);

                    var crowd = player.stackManager.playerCrowd;
                    for (int i = 0; i < crowd.Count; i++) {
                        crowd[i].playerLevelModule.levelSkins[0].gameObject.SetActive(false);
                        crowd[i].playerLevelModule.levelSkins[1].gameObject.SetActive(true);
                        crowd[i].animationModule.animator = crowd[i].playerLevelModule.levelSkins[1].GetComponent<Animator>();
                    }
                    break;
                case Level.Level3:
                    player.animationModule.animator = levelSkins[2].GetComponent<Animator>();
                    levelSkins[1].gameObject.SetActive(false);
                    levelSkins[2].gameObject.SetActive(true);

                    crowd = player.stackManager.playerCrowd;
                    for (int i = 0; i < crowd.Count; i++) {
                        crowd[i].playerLevelModule.levelSkins[1].gameObject.SetActive(false);
                        crowd[i].playerLevelModule.levelSkins[2].gameObject.SetActive(true);
                        crowd[i].animationModule.animator = crowd[i].playerLevelModule.levelSkins[2].GetComponent<Animator>();
                    }
                    break;
            }
        }
        public void LevelUp() {
            if (_requiedScore <= player.uIManager.scoreManager.score) {
                if (level == Level.Level1) {
                    changed = false;
                    if (!changed) {
                        Instantiate(transformationEffect, player.transform.position + new Vector3(0, 1, 1.5f), Quaternion.identity);
                        changed = true;
                    }
                    level = Level.Level2;
                    player.fireModule.ReduceFireDelay(reduceAmount);
                } else if (level == Level.Level2) {
                    if (changed) {
                        Instantiate(transformationEffect, player.transform.position + new Vector3(0, 1, 1.5f), Quaternion.identity);
                        changed = false;
                    }
                    level = Level.Level3;
                    player.fireModule.ChangeGunType();
                }
                _requiedScore = requiedScoreForLevel3;
            }
        }
    }
}
