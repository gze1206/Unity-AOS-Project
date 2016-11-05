using UnityEngine;
using System.Collections;

public class DeadLine : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Crash");
            col.GetComponent<Character>().StartCoroutine(col.GetComponent<Character>().Death(DeathNote.Fallen));
        }
    }
}
