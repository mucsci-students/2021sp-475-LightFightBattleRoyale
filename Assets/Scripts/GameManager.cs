using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public Text placement, placementF;
    private float countdownTime = 20;
    private float gameTime = 180;
    private int playersAlive = 0;
    private int place;

    // Arrays
    private GameObject[] walls;
    private GameObject[] spawnPoints; 
    // first spawnpoint will be player's, rest are ai
    private GameObject[] aliveIcons;
    private GameObject[] deathIcons;
    private GameObject[] trails;

    // List of possible playable characters
    [Header("Character Prefabs")]
    public GameObject char1Prefab;
    public GameObject char2Prefab;
    public GameObject char3Prefab;

    // List of trail colors
    [Header("Trail Colors")]
    public Color trailColor1;
    public Color trailColor2;
    public Color trailColor3;
    public Color trailColor4;
    public Color trailColor5;
    public Color trailColor6;
    public Color trailColor7;
    public Color trailColor8;
    private Color[] trailColorList;

    // List of menu GameObjects
    private GameObject[] pauseObjects;
    private GameObject[] endObjects;
    private GameObject[] finishObjects;
    private GameObject[] playerStatusObjects;

    // List of ability icons
    private GameObject[] swiftIcons;
    private GameObject[] covertIcons;
    private GameObject[] phaserIcons;

    //public GameObject Char1, Char2, Char3, selected;
    //private GameObject buttonGameObject;
    //private Queue<GameObject> otherChars = new Queue<GameObject>();

    private void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("Barrier");
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        aliveIcons = GameObject.FindGameObjectsWithTag("AliveIcons");
        deathIcons = GameObject.FindGameObjectsWithTag("DeathIcons");
        trailColorList = new Color[]{trailColor1, trailColor2, trailColor3, trailColor4, trailColor5, trailColor6, trailColor7, trailColor8};

        // Get a list of pause menu's and immediately hide the menu
        pauseObjects = GameObject.FindGameObjectsWithTag("PauseObject");
        endObjects = GameObject.FindGameObjectsWithTag("EndObject");
        finishObjects = GameObject.FindGameObjectsWithTag("FinishObject");
        playerStatusObjects = GameObject.FindGameObjectsWithTag("PlayerStatuses");
        HidePaused();
        HideEnd();
        HideFinish();

        SetupAbilityIcons();

        // Create player
        string selectedChar = PlayerPrefs.GetString("selectedChar");
        int selectedPlayerCount = PlayerPrefs.GetInt("selectedPlayerCount");
        if (selectedPlayerCount == null)
        {
            selectedPlayerCount = 4;
        }
        if (selectedPlayerCount == 4)
        {
            for (int i = 4; i < playerStatusObjects.Length; i++)
            {
                playerStatusObjects[i].SetActive(false);
            }
        } else if (selectedPlayerCount == 6)
        {
            for (int i = 6; i < playerStatusObjects.Length; i++)
            {
                playerStatusObjects[i].SetActive(false);
            }
        }
        print ("Players: " + selectedPlayerCount);
        Vector3 spawnPoint = spawnPoints[0].transform.position;
        Quaternion spawnRotation = spawnPoints[0].transform.rotation;
        playersAlive++;
        GameObject playerInstance = null;
        if(selectedChar == "char1") {
            playerInstance = (GameObject)Instantiate(char1Prefab, spawnPoint, spawnRotation);
            playerInstance.GetComponent<SpeedSkillScript>().isPlayer = true;
            playerStatusObjects[0].GetComponent<TextMeshProUGUI>().text = "Swift (You)";
            foreach (GameObject i in swiftIcons)
            {
                i.SetActive(true);
            }
            Destroy(playerInstance.transform.Find("SightCone").gameObject);
        }
        else if(selectedChar == "char2") {
            playerInstance = (GameObject)Instantiate(char2Prefab, spawnPoint, spawnRotation);
            playerInstance.GetComponent<InvisSkillScript>().isPlayer = true;
            playerStatusObjects[0].GetComponent<TextMeshProUGUI>().text = "Covert (You)";
            foreach (GameObject i in covertIcons)
            {
                i.SetActive(true);
            }
        }
        else if(selectedChar == "char3") {
            playerInstance = (GameObject)Instantiate(char3Prefab, spawnPoint, spawnRotation);
            playerInstance.GetComponent<PhaseSkillScript>().isPlayer = true;
            playerStatusObjects[0].GetComponent<TextMeshProUGUI>().text = "Phaser (You)";
            foreach (GameObject i in phaserIcons)
            {
                i.SetActive(true);
            }
        }
        playerInstance.GetComponent<CollisionScript>().playerID = 0;
        Destroy(playerInstance.transform.Find("SightCone").gameObject);
        playerInstance.name = "Player";

        // Change character trail color
        ParticleSystem.MainModule playerPS = playerInstance.transform.Find("Trail").gameObject.GetComponent<ParticleSystem>().main;
        playerPS.startColor = trailColorList[0];

        GameObject[] enemies = new GameObject[selectedPlayerCount - 1];

        // Load in the enemies
        for(int i = 0; i < selectedPlayerCount - 1; ++i) {
            // Get the next spawn point, need to shift by 1 because the player used one
            spawnPoint = spawnPoints[i+1].transform.position;
            spawnRotation = spawnPoints[i+1].transform.rotation;
            
            // Randomly choose which character the enemy will be
            int enemyChar = Random.Range(1, 4);
            enemies[i] = null;
            if(enemyChar == 1) {
                enemies[i] = (GameObject)Instantiate(char1Prefab, spawnPoint, spawnRotation, spawnPoints[i+1].transform);
                playerStatusObjects[i+1].GetComponent<TextMeshProUGUI>().text = "Swift";
            } else if(enemyChar == 2) {
                enemies[i] = (GameObject)Instantiate(char2Prefab, spawnPoint, spawnRotation, spawnPoints[i+1].transform);
               playerStatusObjects[i+1].GetComponent<TextMeshProUGUI>().text = "Covert";
            } else if(enemyChar == 3) {
                enemies[i] = (GameObject)Instantiate(char3Prefab, spawnPoint, spawnRotation, spawnPoints[i+1].transform);
                playerStatusObjects[i+1].GetComponent<TextMeshProUGUI>().text = "Phaser";
            }
            enemies[i].name = "Enemy" + (i+1);
            enemies[i].GetComponent<CollisionScript>().playerID = i+1;

            // Remove camera and player control scripts from enemy
            Destroy(enemies[i].GetComponent<FirstPersonController>());
            Destroy(enemies[i].transform.Find("Camera").gameObject);

            // Add enemy AI script
            enemies[i].AddComponent<AIController>();
            if (enemyChar==1)
            {
                enemies[i].GetComponent<AIController>().type = "Swift";
            } else if (enemyChar == 2)
            {
                enemies[i].GetComponent<AIController>().type = "Covert";
            } else if (enemyChar == 3)
            {
                enemies[i].GetComponent<AIController>().type = "Phaser";
            }
            // Change trail color
            ParticleSystem.MainModule enemyPS = enemies[i].transform.Find("Trail").gameObject.GetComponent<ParticleSystem>().main;
            enemyPS.startColor = trailColorList[i+1];

            playersAlive++;
        }

        int playerColliderCount = 0;
        // Loop through and add colliders
        for(int i = 0; i < enemies.Length; i++) {
            int enemyColliderCount = 0;
            for (int j = 0; j < enemies.Length; j++) {
                if(enemies[i] != enemies[j]) {
                    enemies[i].GetComponentInChildren<ParticleSystem>().trigger.SetCollider(enemyColliderCount++, enemies[j].transform.Find("SightCone").GetComponent<Collider>());
                }
            }
            playerInstance.GetComponentInChildren<ParticleSystem>().trigger.SetCollider(playerColliderCount++, enemies[i].transform.Find("SightCone").GetComponent<Collider>());
        }
        playerInstance.GetComponentInChildren<ParticleSystem>().trigger.SetCollider(playerColliderCount++, playerInstance.transform.Find("SightCone").GetComponent<Collider>());

        // THIS NEEDS TO GO LAST
        trails = GameObject.FindGameObjectsWithTag("Trail");
        print(trails.Length);
        StartCoroutine("Countdown");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void PlayerHasDied (int playerIndex)
    {   
        if (aliveIcons[playerIndex].GetComponent<Image>().enabled == true)
        {
            aliveIcons[playerIndex].GetComponent<Image>().enabled = false;
            deathIcons[playerIndex].GetComponent<Image>().enabled = true;
        }
        playersAlive--;  
        if (playerIndex == 0)
        {
            place = playersAlive + 1;
            ShowEnd();   
        }     
    }

    private void SetupAbilityIcons()
    {
        swiftIcons = GameObject.FindGameObjectsWithTag("Swift Icons");
        covertIcons = GameObject.FindGameObjectsWithTag("Covert Icons");
        phaserIcons = GameObject.FindGameObjectsWithTag("Phaser Icons");

        HideAbilityIcons();
    }
    
    private void EndGame()
    {
        Time.timeScale = 0;
        ShowFinish();
        print("Game has ended!");
    }

    private IEnumerator DetectEndOfGame()
    {
        yield return new WaitWhile(() => (playersAlive > 1));
        EndGame();
    }

    private IEnumerator Countdown()
    {
        foreach (GameObject trail in trails)
        {
            trail.GetComponent<ParticleSystem>().Stop();
        }
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        countdownText.text = "GO!";
        // Start other stuff here
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        StartCoroutine("GameTimer");
        StartCoroutine("DetectEndOfGame");
        DropTheWalls();
    }

    private void DropTheWalls()
    {
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
        foreach (GameObject trail in trails)
        {
            trail.GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            float minutes = Mathf.Floor(gameTime / 60);
            float seconds = Mathf.RoundToInt(gameTime%60);
            string strMinutes = minutes.ToString();
            string strSeconds = seconds.ToString();
            if(seconds < 10) 
            {
                strSeconds = "0" + Mathf.RoundToInt(seconds).ToString();
            }  
            timerText.text = strMinutes.ToString() + ":" + strSeconds.ToString();
            yield return new WaitForSeconds(1f);
            gameTime--;
        }
    }

    public void TogglePause() {
        if(Time.timeScale == 1) {
            Time.timeScale = 0;
            ShowPaused();
        }
        else if(Time.timeScale == 0) {
            Time.timeScale = 1;
            HidePaused();
        }
    }

    public void ShowPaused() {
        foreach(GameObject g in pauseObjects) {
            g.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HidePaused() {
        foreach(GameObject g in pauseObjects) {
            g.SetActive(false);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetCursorAndTime() {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowEnd() {
        switch (place)
        {
            case 2:
                placement.text = "You Got " + place.ToString() + "nd";
                break;
            case 3:
                placement.text = "You Got " + place.ToString() + "rd";
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                placement.text = "You Got " + place.ToString() + "th";
                break;
        }
        foreach(GameObject g in endObjects) {
            g.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideEnd() {
        foreach(GameObject g in endObjects) {
            g.SetActive(false);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowFinish()
    {
        if (place != 1)
        {
            switch (place)
            {
                case 2:
                    placementF.text = "You Got " + place.ToString() + "nd";
                    break;
                case 3:
                    placementF.text = "You Got " + place.ToString() + "rd";
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    placementF.text = "You Got " + place.ToString() + "th";
                    break;
            }
        }
        HideEnd();
        foreach(GameObject g in finishObjects) {
            g.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideFinish() {
        foreach(GameObject g in finishObjects) {
            g.SetActive(false);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HideAbilityIcons ()
    {
        foreach (GameObject i in swiftIcons)
        {
            i.SetActive(false);
        }
        foreach (GameObject i in covertIcons)
        {
            i.SetActive(false);
        }
        foreach (GameObject i in phaserIcons)
        {
            i.SetActive(false);
        }
    }
}
