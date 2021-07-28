// Generate the environment based on an offset from the camera. 
// Add objects not generated in the inspector to the scrollObjects list
// Also add the last wall and slinger to currWall and currSlinger in the inspector (may fix automatically later)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GenerateEnvironment : GenericSingleton<GenerateEnvironment>
{
    [SerializeField]
    float bounds = 10f;
    [SerializeField]
    PlayerController player;
    [SerializeField]
    CameraController cam;
    [SerializeField]
    List<GameObject> scrollObjects;
    [SerializeField]
    GameObject[] prefabs;
    [SerializeField]
    GameObject currWall;
    [SerializeField]
    GameObject currSlinger;
    [SerializeField]
    GameObject ground;

    List<GameObject> objectsToDestroy = new List<GameObject>();

    [SerializeField]
    float distanceBetweenSlingers;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {      
        if(player.Velocity.y != 0)
            ScrollObjects();

        CheckBounds();
    }

    private void Initialize()
    {
        if(!currWall)
           currWall = SpawnObject(prefabs[0], prefabs[0].transform.position);

        if (!currSlinger)
            currSlinger = SpawnObject(prefabs[1], new Vector2(
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
        foreach (GameObject go in scrollObjects)
        {
            if (go.transform.position.y > cam.transform.position.y - cam.Height - bounds)
                go.transform.position -= new Vector3(0, player.Velocity.y, 0) * Time.deltaTime;
            else
            {
                objectsToDestroy.Add(go);
            }
                
        }
        foreach (GameObject go in objectsToDestroy)
        {
            scrollObjects.Remove(go);
            Destroy(go);
        }
    }
    
    // Spawn objects when inside spawn bounds
    void CheckBounds()
    {
        if (ground && ground.transform.position.y < cam.transform.position.y - cam.Height - 10f)
        {
            objectsToDestroy.Add(ground);
        }
        if (currWall.transform.position.y < cam.transform.position.y + cam.Height + bounds)
        {
            Vector2 pos = currWall.transform.position;
            currWall = SpawnObject(prefabs[0], new Vector2(pos.x, pos.y + currWall.transform.localScale.y));
        }

        if (currSlinger.transform.position.y < cam.transform.position.y + cam.Height + bounds)
        {
            Vector2 pos = currSlinger.transform.position;
            currSlinger = SpawnObject(prefabs[1], new Vector2(Random.Range(-1f, 1f) * Camera.main.orthographicSize - prefabs[0].transform.localScale.x, pos.y + distanceBetweenSlingers));
        } 
    }

    GameObject SpawnObject(GameObject objectToSpawn, Vector2 positionToSpawn)
    {
        GameObject go = Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity);
        scrollObjects.Add(go);
        return go;
    }
}
