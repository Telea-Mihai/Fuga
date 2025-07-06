using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Car Properties")] public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;

    public Transform steeringWheel;
    public float steeringWheelRotationMultiplier;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;
    private Vector3 steeringDefaultRot;

    public float vInput, hInput, braking;
    public bool started;

    [SerializeField] private AudioSource engine;
    [SerializeField] private AudioSource startSound;
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        steeringDefaultRot = steeringWheel.transform.rotation.eulerAngles; 

        wheels = GetComponentsInChildren<WheelControl>();
    }
    
    void Update()
    {
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); 
        
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor) * (started ? 1 : 0);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);
        
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
        float steeringAngle = hInput * steeringRange * steeringWheelRotationMultiplier;

        if (started)
        {
            float targetPitch = Mathf.Clamp(1f + Mathf.Abs(vInput) * 0.5f + rigidBody.linearVelocity.magnitude / maxSpeed, 0.8f, 2.5f);
            engine.pitch = Mathf.Lerp(engine.pitch, targetPitch, Time.deltaTime * 3f);

        }

        steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, Quaternion.Euler(steeringDefaultRot.x, steeringDefaultRot.y , steeringDefaultRot.z + steeringAngle), 0.5f);
    }

    public void startCar()
    {
        StopAllCoroutines();
        StartCoroutine(startSequence());
    }

    public void stopCar()
    {
        StopAllCoroutines();
        StartCoroutine(stopSequence());
    }

    IEnumerator startSequence()
    {
        startSound.Play();
        yield return new WaitForSeconds(2f);
        started = true;
        engine.loop = true;
        engine.Play();
    }
    
    IEnumerator stopSequence()
    {
        started = false;

        float fadeDuration = 2f;
        float elapsed = 0f;
        float initialPitch = engine.pitch;
        float initialVolume = engine.volume;

        engine.loop = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            engine.pitch = Mathf.Lerp(initialPitch, 0.5f, t);
            engine.volume = Mathf.Lerp(initialVolume, 0f, t);

            yield return null;
        }

        engine.Stop();
        engine.pitch = 1f;
        engine.volume = 1f;
    }

}