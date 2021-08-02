// Generate the environment based on an offset from the camera. 
// Add objects not generated in this script to the scrollObjects list in the inspector.

using System.Collections.Generic;
using UnityEngine;
public class GenerateEnvironment : GenericSingleton<GenerateEnvironment>
{
    [SerializeField]
    float bounds = 10f;
    [SerializeField]
    float chanceToSpawnWaterfall;
    [SerializeField]
    float distanceBetweenSlingers;
    [SerializeField]
    int wallsToSpawnBeforeWaterfall;
    int spawnedWalls = 0;

    [SerializeField] PlayerController player;
    [SerializeField] CameraController cam;
    [SerializeField] List<GameObject> scrollObjects;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject currWall;
    [SerializeField] GameObject currSlinger;
    [SerializeField] GameObject ground;

    List<GameObject> objectsToDestroy = new List<GameObject>();

    private void Awake()
    {
        // Initialize start objects cause the rest of the generated objects are based on the location of the previous objects
        Initialize();
    }

    private void Update()
    {      
        ScrollObjects();
        CheckBounds();
    }

    private void Initialize()
    {
        if (!currWall)
            currWall = SpawnScrollingObject(prefabs[0], prefabs[0].transform.position);
        if (!currSlinger)
            currSlinger = SpawnScrollingObject(prefabs[1], new Vector2(
                                                  Random.Range(-1f, 1f) * Camera.main.orthographicSize - prefabs[0].transform.localScale.x / 2,
                                                  player.transform.position.y + distanceBetweenSlingers));  
        else
            currSlinger.transform.position = new Vector2(GetObjectPosWithOffset(prefabs[1]), transform.position.y);
    }

    float GetObjectPosWithOffset(GameObject o)
    {
        // Get random slinger x-pos with an offset from the wall
        if (o == prefabs[1])
            return Random.Range(-1f, 1f) * Camera.main.orthographicSize * Camera.main.aspect - (prefabs[0].transform.localScale.x + prefabs[1].transform.localScale.x / 2);
        else
            return 0;
    }

    // Scroll the objects to be moved downwards relative to the player velocity
    public void ScrollObjects()
    {
        // Ground is destroyed earlier than other objects
        if (ground && ground.transform.position.y < cam.transform.position.y - cam.Height - 10f)
        {
            objectsToDestroy.Add(ground);
        }

        // Check if objects are out of bounds and add to a list of destruction
        foreach (GameObject go in scrollObjects)
        {
            if (go.transform.position.y > cam.transform.position.y - cam.Height - bounds)
                go.transform.position -= new Vector3(0, player.Velocity.y, 0) * Time.deltaTime;
            else
                objectsToDestroy.Add(go);
                
        }

        // Destroy all items in the list of destruction
        foreach (GameObject go in objectsToDestroy)
        {
            scrollObjects.Remove(go);
            Destroy(go);
        }
    }
    
    // Spawn objects when inside spawn bounds (camera bounds + offset)
    void CheckBounds()
    {
        if (currWall.transform.position.y < cam.transform.position.y + cam.Height + bounds)
        {
            Vector2 pos = currWall.transform.position;
            currWall = SpawnScrollingObject(prefabs[0], new Vector2(pos.x, pos.y + currWall.transform.localScale.y));

            if(spawnedWalls > wallsToSpawnBeforeWaterfall)
                SpawnObjectByChance(prefabs[2], chanceToSpawnWaterfall);
        }
        if (currSlinger.transform.position.y < cam.transform.position.y + cam.Height + bounds)
        {
            Vector2 pos = currSlinger.transform.position;
            currSlinger = SpawnScrollingObject(prefabs[1], new Vector2(Random.Range(-1f, 1f) * Camera.main.orthographicSize - prefabs[0].transform.localScale.x, pos.y + distanceBetweenSlingers));
        } 
    }

    GameObject SpawnScrollingObject(GameObject objectToSpawn, Vector2 positionToSpawn)
    {
        GameObject go = Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity);
        scrollObjects.Add(go);
        return go;
    }

    void SpawnObjectByChance(GameObject objectToSpawn, float chanceToSpawn)
    {
        int random = Random.Range(0, 101);

        if (random > chanceToSpawn)
            return;
        else
        {   // If the object is a waterfall the side which it spawns is also randomized
            if(objectToSpawn = prefabs[2])
            {
                random = Random.Range(0, 101);

                if (random > 50)
                    Instantiate(prefabs[2], Vector3.up + Vector3.left * prefabs[2].transform.position.x, Quaternion.identity);
                else
                    Instantiate(prefabs[2], Vector3.up + Vector3.right * prefabs[2].transform.position.x, Quaternion.identity);
            }   
            else
                Instantiate(objectToSpawn, objectToSpawn.transform);
        }
    }
}
