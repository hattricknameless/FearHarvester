using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour {
    
    public static MobManager mobManager;
    private TileManager tileManager;
    private AudioSource deathAudio;
    [SerializeField] private List<MobController> mobs = new();

    private void Awake() {
        mobManager = this;
        tileManager = TileManager.tileManager;
        deathAudio = GetComponent<AudioSource>();

        FindAllMobs();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log("Next Turn Triggered");
            NextTurn();
        }
    }

    private void FindAllMobs() {
        MobController[] mobControllers = FindObjectsOfType<MobController>();
        foreach (MobController mob in mobControllers) {
            mobs.Add(mob);
        }
    }

    public void DestroyMob(MobController mob) {
        deathAudio.Play();
        mobs.Remove(mob);
        Destroy(mob.gameObject);
        if (mobs.Count == 0) {
            tileManager.GameEnd(true);
        }
    }

    public event Action OnNextTurn;
    public void NextTurn() {
        OnNextTurn?.Invoke();
    }
}
