using UnityEngine;
using System.Collections;

public class ScrMoveCamera : MonoBehaviour
{
    public float speed = 3;
    CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.Equals) ? 1 : Input.GetKey(KeyCode.Minus) ? -1 : 0);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        controller.Move(moveDirection * Time.deltaTime);
    }

}
