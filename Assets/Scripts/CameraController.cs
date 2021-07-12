// Camera class used to retrieve dimensions in world space
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float offsetYFromPlayer;
    Camera cam;
    [SerializeField]
    PlayerController player;
    
    private void Update()
    {
        if(player.Velocity.y > -30f)
            transform.position = new Vector3(transform.position.x, player.transform.position.y + offsetYFromPlayer, transform.position.z);
        else
        {
            transform.position -= new Vector3(transform.position.x, player.Velocity.y, transform.position.z) * Time.deltaTime;
            GameManager.Instance.LoadScene(1, "Main");
        }         
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public float Height
    {
        get
        {
            return cam.orthographicSize;
        }
    }

    public float Width
    {
        get 
        {
            return transform.position.y + Height * cam.aspect;
        }
    }
}
