using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnvironment : GenericSingleton<GenerateEnvironment>
{
    [SerializeField]
    List<GameObject> object_list;
    [SerializeField]
    GameObject[] prefabs;
    [SerializeField]
    Transform currWallPos, currSlingerPos;

    private void Start()
    {
        Initialize();
    }
    private void Update()
    {
        // Scroll objects downward
        MoveObjects();
        // Check if objects are out of bounds
        CheckBounds(); 
    }

    void Initialize()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Procedural"))
        {
            object_list.Add(go);
        }
    }
    
    // Scroll the objects to be moved downwards in the speed of the player velocity
    public void MoveObjects()
    {
        foreach (GameObject go in object_list)
        {
            go.transform.position += new Vector3(0, PlayerController.Instance.Velocity.y, 0) * Time.deltaTime;
        }
    }
    
    // Checks if objects is out of bounds based on the camera width and height
    void CheckBounds()
    {
        List<GameObject> destroy_list = new List<GameObject>();
        List<GameObject> add_list = new List<GameObject>();

        

        foreach (GameObject go in object_list)
        {
            if (go.transform.position.y < CameraController.Instance.Height * 1.2f)
            {
                destroy_list.Add(go);
                add_list.Add(Instantiate(prefabs[0], new Vector2(currWallPos.position.x, currWallPos.position.y + 12f), currWallPos.rotation));
            }
        }

        foreach (GameObject go in destroy_list)
        {
            object_list.Remove(go);
            Destroy(go);
        }
    }
}
