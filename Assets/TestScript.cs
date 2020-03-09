using Network;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            transform.Translate(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D))
            transform.Translate(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.Space))
            NetworkManager.SpawnGameObject(prefabToSpawn, Vector3.zero, Quaternion.identity);
    }
}
