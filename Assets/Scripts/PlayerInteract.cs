using UnityEngine;

public class PlayerInteract : MonoBehaviour {


    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)){
        float interactRange = 2f;
        Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray){
                Debug.Log(collider);
             }
        }
    }
}
