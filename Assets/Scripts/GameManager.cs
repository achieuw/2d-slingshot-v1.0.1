using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform playerSpawn;

    private void Awake()
    {
        //SpawnPlayer(playerSpawn.position);
    }
    // Update is called once per frame
    void Update()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void SpawnPlayer(Vector2 pos)
    {
        foreach(PlayerController player in FindObjectsOfType<PlayerController>())
        {
            Destroy(player);
        }
        Instantiate(playerPrefab, pos, Quaternion.identity);
    }

    public void LoadScene(float delay, string sceneName)
    {
        StartCoroutine(LoadSceneDelay(delay, sceneName));
    }

    private IEnumerator LoadSceneDelay(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }
}
