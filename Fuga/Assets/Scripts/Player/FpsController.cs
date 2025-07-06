
using UnityEngine;

public class FpsController : MonoBehaviour
{

    [Header("Asignables")]
    [SerializeField]public Transform playerCam;
    [SerializeField]private Transform orientation;
    [SerializeField]private Transform Headposition;
    
    private Rigidbody rb;

    [Header("Sensitivity")]
    public bool controlsCamera = true;
    private float xRotation;
    public float sensitivity = 50f;
    private float sensMultiplier = 1f;

    [Header("Movement")]
    public float moveSpeed = 500;

    public float moveMaxSpeed = 2f;
    private float maxSpeed = 20;
    public float sprintspeed=1000;
    public float sprintmaxspeed = 5f;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    [Tooltip("Crouch and Slide")]
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float crouchspeed;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    [Tooltip("Jumping")]
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 150f;

    [Header("Parkour")] public bool parkourEnabled;
    [Header("Detection")]
    //I could use a list/dictionary but I think I would complicate too much
    public Detection Climb;
    public Detection checkClimb;
    public Detection Vault;
    public Detection checkVault;
    [Header("The actual Movement")]
    public Vector2 offset= new Vector2(0.7f,0.3f);

    public bool IsParkour;
    private float chosenParkourMoveTime;

    public AnimationCurve parkour_aceleration;

    [Tooltip("Vaulting")]
    private bool CanVault;
    public float VaultTime; //how long the vault takes
    public Transform VaultEndPoint;
    [Tooltip("Climbing")]
    private bool CanClimb;
    public float ClimbTime; //how long the climb takes
    public Transform endPosition;

    [Header("Camera effects")]
    public bool CamEfEnabled = true;
    public AnimationCurve jumpMoition;
    public AnimationCurve crouchMoition;
    public float curveMultiplier;
    public float leanMotion;
    [Header("Animations")]
    public Animator gunContAnimation;
    public Animator CharacterModel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepClip;
    public float footstepDelay = 0.5f; // how often footsteps play when walking
    private float footstepTimer = 0f;
    
    private float[] curveTimes= new float[3];
    private float parkourSpeed=0;
    private Vector3 RecordedMoveToPosition; //the position of the vault end point in world space to move the player to
    private Vector3 RecordedStartPosition; // position of player right before vault

    //for cam effects
    private Vector3 defaultCamRot;
    private bool playJumpAnim, playCrouchAnim;
    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    private Vector3 oldvelocity;

    private Vector3 startpos;
    private RaycastHit vaultHit;
    private RaycastHit climbHit;
    private bool checkEnviorment=true;

    [HideInInspector]public float camXOffset;

    [System.Serializable] //Trick to show structs in inspectors!
    public struct Detection {
        public float lenght;
        public Transform origin;
        public LayerMask layer;
        public bool intersection;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        playerScale.y = GetComponent<CapsuleCollider>().height;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        if(controlsCamera)
            Look();

        if(parkourEnabled)
            CheckEnviorment();

      if(CanVault&&jumping)
        {
            print("Vault");
            oldvelocity = rb.linearVelocity/2f;
            checkEnviorment = false;
            rb.isKinematic = true;
            //calculate the endpoint

            RecordedMoveToPosition = vaultHit.point + new Vector3(vaultHit.normal.x * -offset.x, transform.localScale.y + offset.y, vaultHit.normal.z * -offset.x);

            endPosition.position = RecordedMoveToPosition;
            RecordedStartPosition = transform.position;
            IsParkour = true;
            chosenParkourMoveTime = VaultTime;
        }
      else if (CanClimb&&jumping)
        {
            print("Climb");
            oldvelocity = rb.linearVelocity/2;
            checkEnviorment = false;
            rb.isKinematic = true;

            RecordedMoveToPosition = climbHit.point + new Vector3(climbHit.normal.x * -offset.x, transform.localScale.y + offset.y, climbHit.normal.z * -offset.x);
            
            endPosition.localPosition = RecordedMoveToPosition;
            RecordedStartPosition = transform.position;
            IsParkour = true;
            chosenParkourMoveTime = ClimbTime;
        }
      
        HandleFootsteps();  

    }

    /// Find user input. Should put this in its own class but im lazy
    private void MyInput()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);
        //Character animation
        if (CharacterModel != null)
        {
            CharacterModel.SetFloat("Vertical", y);
            CharacterModel.SetFloat("Horizontal", x);
            CharacterModel.SetBool("Jump", jumping);
            CharacterModel.SetBool("Grounded", grounded);
        }
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void CheckEnviorment()
    {
        Vault.intersection = Physics.Raycast(Vault.origin.position, Vault.origin.forward,out vaultHit, Vault.lenght, Vault.layer, QueryTriggerInteraction.Ignore);
        checkVault.intersection = Physics.Raycast(checkVault.origin.position, checkVault.origin.forward, checkVault.lenght, checkVault.layer, QueryTriggerInteraction.Ignore);
        if (Vault.intersection && !checkVault.intersection&&!IsParkour&&checkEnviorment)
            CanVault = true;
        else
            CanVault = false;

        Climb.intersection = Physics.Raycast(Climb.origin.position, Climb.origin.forward,out climbHit, Climb.lenght, Climb.layer, QueryTriggerInteraction.Ignore);
        checkVault.intersection = Physics.Raycast(checkClimb.origin.position, checkClimb.origin.forward, checkClimb.lenght, checkClimb.layer, QueryTriggerInteraction.Ignore);
        if (Climb.intersection && !checkClimb.intersection&&!IsParkour&&checkEnviorment)
            CanClimb = true;
        else
            CanClimb = false;
    }
    
    private void StartCrouch()
    {
        transform.GetComponent<CapsuleCollider>().height = crouchScale.y;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }


    private void StopCrouch()
    {
        transform.GetComponent<CapsuleCollider>().height = playerScale.y;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        if (DialogManager.Instance.inDialog)
            return;
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping && !IsParkour && !rb.isKinematic) Jump();
        if (readyToJump)
        {
            startpos = transform.position;
        }

        if (IsParkour && curveTimes[0] < 1f && parkourEnabled)
        {
            parkourSpeed = parkour_aceleration.Evaluate(curveTimes[0]);
            curveTimes[0] += Time.deltaTime / chosenParkourMoveTime;
            transform.position = Vector3.Lerp(RecordedStartPosition, RecordedMoveToPosition, parkourSpeed);

            if (curveTimes[0]>= 1f)
            {
                IsParkour = false;
                curveTimes[0] = 0;
                rb.isKinematic = false;
                checkEnviorment = true;
                rb.linearVelocity = oldvelocity;
                Debug.Log(oldvelocity);
            }

        }
        //Decide on speed
            float curSpeed = moveSpeed;
            maxSpeed = moveMaxSpeed;


            if (sprinting)
            {
                curSpeed = sprintspeed;
                maxSpeed = sprintmaxspeed;
            }
            if (crouching)
                curSpeed = crouchspeed;

            //If sliding down a ramp, add force down so player stays grounded and also builds speed
            if (crouching && grounded && readyToJump)
            {
                rb.AddForce(Vector3.down * Time.deltaTime * 3000);
                return;
            }
            
            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (x > 0 && xMag > maxSpeed) x = 0;
            if (x < 0 && xMag < -maxSpeed) x = 0;
            if (y > 0 && yMag > maxSpeed) y = 0;
            if (y < 0 && yMag < -maxSpeed) y = 0;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Movement in air
            if (!grounded)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }



            // Movement while sliding
            if (grounded && crouching) multiplierV = 0f;

            //Apply forces to move player
            rb.AddForce(orientation.transform.forward * y * curSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * curSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.linearVelocity;
            if (rb.linearVelocity.y < 0.5f)
                rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    private void Look()
    {
        //Move camera
        playerCam.position = Headposition.position;
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        // Rotate up/down (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate left/right (yaw) with camXOffset added only once
        desiredX += mouseX;
    
        // Apply rotations with camXOffset statically added to orientation
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX + camXOffset, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX + camXOffset, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.linearVelocity.y;
            Vector3 n = rb.linearVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    private void CameraAnimations()
    {
        
    }
    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.linearVelocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }


    private void StopGrounded()
    {
        grounded = false;
    }
    
    private void HandleFootsteps()
    {
        if (!grounded || IsParkour || rb.linearVelocity.magnitude < 0.2f)
            return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            audioSource.pitch = 1f + Random.Range(-0.05f, 0.05f); 
            audioSource.PlayOneShot(footstepClip);
            footstepTimer = footstepDelay;
            
            if (sprinting) footstepTimer *= 0.6f;
            else if (crouching) footstepTimer *= 1.4f;
        }
    }

}
