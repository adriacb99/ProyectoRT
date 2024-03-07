using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    [SerializeField] float gravity;

    public void AttractPlayer(GameObject player)
    {
        Debug.Log("El personaje está siendo atraído");

        Transform playerTransform = player.transform;
        Rigidbody playerRigidBody = playerTransform.GetComponent<Rigidbody>();
        

        Vector3 gravityUp = (playerTransform.position - transform.position).normalized;
        Vector3 playerUp = playerTransform.up;

        playerRigidBody.AddForce(gravityUp * gravity);

        Quaternion playerRotation = Quaternion.FromToRotation(playerUp, gravityUp) * playerTransform.rotation;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, playerRotation, 50*Time.deltaTime);

    }
    
}
