using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //TODO: respawn with reposition instead of Scene-Reload

        if (other.gameObject.name == "DeadZone")
            SceneManager.LoadScene(3);
    }
}