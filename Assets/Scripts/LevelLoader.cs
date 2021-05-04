using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSpecificLevel(int level)
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(LoadLevel(level));
    }

    private IEnumerator LoadLevel (int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
