using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider WheelCollider;
    
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelCollider.GetWorldPose(out position, out rotation);
        rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = Quaternion.Lerp(wheelModel.transform.rotation, rotation, 0.3f);
    }
}