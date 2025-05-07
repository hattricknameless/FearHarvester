using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    
    public static TutorialManager tutorialManager;
    private TileManager tileManager;
    private Canvas canvas;
    [SerializeField] private List<string> dialogues;
    [SerializeField] private int dialoguePointer = 0;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip successAudio;
    [SerializeField] private AudioClip failAudio;

    private void Awake() {
        tutorialManager = this;
        tileManager = TileManager.tileManager;
        canvas = GetComponent<Canvas>();
        audioSource = GetComponent<AudioSource>();

        tileManager.OnGameEnd += GameEnd;
    }

    private void Start() {
        StartCoroutine(TutorialDialogue());
    }

    private IEnumerator TutorialDialogue() {
        if (SceneManager.GetActiveScene().buildIndex != 0) {
            canvas.enabled = false;
            OnTutorialEnd?.Invoke();
            yield break;
        }
        tutorialText.text = dialogues[dialoguePointer++];
        yield return null;
        for (int i = 1; i < dialogues.Count; i++) {
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            audioSource.PlayOneShot(clickAudio);
            tutorialText.text = dialogues[dialoguePointer++];
            yield return null;
        }
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        audioSource.PlayOneShot(clickAudio);
        OnTutorialEnd?.Invoke();
        canvas.enabled = false;
        yield break;
    }

    private void GameEnd(bool result) {
        StartCoroutine(GameEndScreen(result));
    }

    private IEnumerator GameEndScreen(bool result) {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (result && currentSceneIndex + 1 == SceneManager.sceneCountInBuildSettings){
            tutorialText.text = "Congrats You Have Completed The Game!!\nLeft Click to Start Over";
            audioSource.PlayOneShot(successAudio);
        }
        else if (result) {
            tutorialText.text = "Level Cleared!!\nGo to the Next Level";
            audioSource.PlayOneShot(successAudio);
        }
        else {
            tutorialText.text = "Try again";
            audioSource.PlayOneShot(failAudio);
        }
        canvas.enabled = true;
        yield return null;
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        if (result) {
            if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings) {
                SceneManager.LoadSceneAsync(currentSceneIndex + 1, LoadSceneMode.Single);
            }
            else {
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            }
        }
        else {
            SceneManager.LoadSceneAsync(currentSceneIndex, LoadSceneMode.Single);
        }
    }

    public Action OnTutorialEnd;
}