using System;
using UnityEngine;
using TMPro;
public class VehicleInput : MonoBehaviour, IInteractable
{
	public bool drived;
	public Car car;

    private Transform playerTransform;
    public Transform driverSeat;
    public Transform driverGetOut;
    public Transform cameraPosition;
    public Vector3 fpPos;
    public Vector3 tppos;
    private bool tp = false;
    public float cooldown;
    private float time;
    private bool seated;
    public LayerMask carLayer;

    private FpsController player = null;
    private Transform playerCam;


	private void Update()
	{
		if(drived)
			GetPlayerInput();
        else
        {
            car.hInput = 0f;
            car.vInput = 0f;
        }

		if (seated)
		{
            playerTransform.position = Vector3.Lerp(playerTransform.position,driverSeat.position, 1f);
		}
       

        if (seated && Input.GetKeyDown(KeyCode.C))
        {
	        tp = !tp;
	        cameraPosition.GetComponent<SpringJoint>().connectedAnchor = tp ? tppos : fpPos;
	        if (tp)
		        cameraPosition.GetComponent<SphereCollider>().excludeLayers = carLayer;
	        else
		        cameraPosition.GetComponent<SphereCollider>().excludeLayers = ~carLayer;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
	        if (car.started)
		        car.stopCar();
	        else
				car.startCar();
        }

        if (player != null)
        {
	        player.camXOffset = transform.rotation.eulerAngles.y/2f;
	        Look();
        }

	}
	private void GetPlayerInput()
	{
		car.hInput = Input.GetAxisRaw("Horizontal");
		car.vInput = Input.GetAxis("Vertical");
	}

	private float xRotation, desiredX;
	private void Look()
	{
		float mouseX = Input.GetAxis("Mouse X") * player.sensitivity * Time.fixedDeltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * player.sensitivity * Time.fixedDeltaTime;

		// Rotate up/down (pitch)
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		// Rotate left/right (yaw) with camXOffset added only once
		desiredX += mouseX;
    
		// Apply rotations with camXOffset statically added to orientation
		playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX + transform.rotation.eulerAngles.y, 0);
	}

    private void Toggle()
    {
        time = Time.time;
        // gameObject.GetComponent<BoxCollider>().enabled = !gameObject.GetComponent<BoxCollider>().enabled;
        seated = !seated;
        drived = !drived;
        playerTransform.GetComponent<CapsuleCollider>().enabled = !playerTransform.GetComponent<CapsuleCollider>().enabled;
        playerTransform.transform.GetComponent<Rigidbody>().isKinematic = !playerTransform.transform.GetComponent<Rigidbody>().isKinematic;
        player.controlsCamera = !player.controlsCamera;
        if (seated)
	        playerCam.parent = cameraPosition;
        else playerCam.parent = player.transform;
    }
    
    public bool is_Ineractable { get => true; set => throw new System.NotImplementedException(); }

    public void Interact(PlayerInteraction playerInteraction)
    {
	    if (seated && time<Time.time)
	    {
		    Toggle();
		    playerTransform.position = driverGetOut.position;
		    player = null;
		    car.stopCar();
		    return;
	    }
        if (is_Ineractable&&!seated&&time<Time.time)
        {
            player = playerInteraction.transform.GetComponent<FpsController>();
            playerCam = player.playerCam;
            playerTransform = player.transform;
            cameraPosition.GetComponent<SpringJoint>().connectedAnchor = tp ? tppos : fpPos;
            if (tp)
	            cameraPosition.GetComponent<SphereCollider>().excludeLayers = carLayer;
            else
	            cameraPosition.GetComponent<SphereCollider>().excludeLayers = ~carLayer;
            playerCam.parent = cameraPosition;
            playerCam.localPosition = Vector3.zero;
            Toggle();
        }
    }
    
    public String GetInteractionPrompt() => seated ? "Press E to exit" : "Press E to enter";
    
}
