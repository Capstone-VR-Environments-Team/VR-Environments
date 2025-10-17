using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenTriggered && other.CompareTag("GameController"))
        {
            hasBeenTriggered = true;
            EndTrial();
        }
    }

    void EndTrial()
    {
        Debug.Log("Trial ended!");
        GetComponent<Renderer>().material.color = Color.green;
        //save data
        //load next scene
    }
}
