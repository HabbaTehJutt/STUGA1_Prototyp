using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray){
            Debug.Log(collider);
        }
        }
    }
}
