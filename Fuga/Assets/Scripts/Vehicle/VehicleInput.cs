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
    public float cooldown;
    private float time;
    private bool seated;


	private void Update()
	{
		if(drived)
			GetPlayerInput();
        else
        {
            car.steering = 0;
            car.throttle = 0;
            car.braking = true;
        }
        if (seated)
            playerTransform.position = Vector3.Lerp(playerTransform.position,driverSeat.position, 1f);
        if (seated && Input.GetKeyDown(KeyCode.E) && time<Time.time)
        {
            Toggle();
            playerTransform.position = driverGetOut.position;
        }
		
	}
	private void GetPlayerInput()
	{
		car.steering = Input.GetAxisRaw("Horizontal");
		car.throttle = Input.GetAxis("Vertical");
		car.braking = Input.GetButton("Breaking");
	}

    private void Toggle()
    {
        time = Time.time;
        gameObject.GetComponent<BoxCollider>().enabled = !gameObject.GetComponent<BoxCollider>().enabled;
        seated = !seated;
        drived = !drived;
        playerTransform.GetComponent<CapsuleCollider>().enabled = !playerTransform.GetComponent<CapsuleCollider>().enabled;
        playerTransform.transform.GetComponent<Rigidbody>().isKinematic = !playerTransform.transform.GetComponent<Rigidbody>().isKinematic;
    }
    
    public bool is_Ineractable { get => true; set => throw new System.NotImplementedException(); }

    public void Interact(PlayerInteraction playerInteraction)
    {
        if (is_Ineractable&&!seated&&time<Time.time)
        {
            FpsController player = playerInteraction.transform.GetComponent<FpsController>();
            playerTransform = player.transform;
            Toggle();
        }
    }
    
    public String GetInteractionPrompt() => "Press E to enter";
    
}
