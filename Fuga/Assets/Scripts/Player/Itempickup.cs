using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itempickup : MonoBehaviour
{
    private GameObject player;
    public Transform holdPos;
    public float throwForce = 500f; 
    public float pickUpRange = 5f; 
    private float rotationSensitivity = 1f; 
    private GameObject heldObj; 
    private Rigidbody heldObjRb;
    private bool canDrop = true; 
    public LayerMask Layermask;

    public float springStrenght;
    public float springDamper;
    private Transform origin;
    
    private FpsController controller;
    private SpringJoint spring;
    private PlayerInteraction playerInteraction;
    
    private float oldAng, oldLin;
    private float time=0;

    private void Start()
    {
        player = transform.gameObject;
        controller = player.GetComponent<FpsController>();
        origin = controller.playerCam;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && time+0.5f < Time.time)
        {
            if (heldObj == null) 
            {
                time = Time.time;
                RaycastHit hit;
                if (Physics.Raycast(origin.position, origin.TransformDirection(Vector3.forward), out hit, pickUpRange, Layermask))
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
            else
            {
                if(canDrop == true)
                {
                    time = Time.time;
                    StopClipping(); 
                    DropObject();
                }
            }
        }
        if (heldObj != null) 
        {
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDrop == true) 
            {
                StopClipping();
                ThrowObject();
            }

        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) 
        {
            heldObj = pickUpObj; 
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            spring = heldObj.AddComponent<SpringJoint>();
            spring.damper = springDamper;
            spring.spring = springStrenght;
            spring.connectedBody = holdPos.GetComponent<Rigidbody>();
            oldAng = heldObjRb.angularDamping;
            oldLin = heldObjRb.linearDamping;
            heldObjRb.linearDamping = 1.5f;
            heldObjRb.angularDamping = 1.5f;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            playerInteraction.canInteract = false;
        }
    }
    public void DropObject()
    {
        if (heldObj == null)
            return;
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj = null; 
        Destroy(spring);
        playerInteraction.canInteract = true;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            canDrop = false;
            controller.controlsCamera = false;
            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
            controller.controlsCamera = true;
        }
    }
    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        Destroy(spring);
        heldObjRb.linearDamping = oldLin;
        heldObjRb.angularDamping = oldAng;
        heldObjRb.AddForce(origin.forward * throwForce, ForceMode.Impulse);
        heldObj = null;
        playerInteraction.canInteract = true;
    }
    void StopClipping() 
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); 
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}