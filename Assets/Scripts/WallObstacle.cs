using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    [SerializeField]
    GameObject water;
    [SerializeField]
    List<GameObject> waterFall;
    [SerializeField]
    float timer;
    [SerializeField]
    float spawnInterval = 0.1f;
    float m_timer = 0;
    float time;

    private void OnEnable()
    {
        time = Time.time;
        m_timer = 0;
    }
    private void Update()
    {
        if (waterFall.Count != 0)
        {
            foreach (GameObject waterBlock in waterFall)
            {
                waterBlock.transform.position -= new Vector3(0, 0.5f * Time.deltaTime * 60, 0);
            }
        }
 
        Spawner();
    }

    void Spawner()
    {
        m_timer += Time.deltaTime;

        if(m_timer < timer)
        {
            if (Time.time > time)
            {
                GameObject newWater = Instantiate(water, transform);
                waterFall.Add(newWater);
                time += spawnInterval;
            }
        }
        else
        {
            List<GameObject> itemsToDestroy = new List<GameObject>();
            foreach(GameObject waterBlock in waterFall)
            {
                itemsToDestroy.Add(waterBlock);
            }

            foreach(GameObject item in itemsToDestroy)
            {
                waterFall.Remove(item);
                Destroy(item);
            }
        }       
    }
}
