using UnityEngine;

public class AntiRoll : MonoBehaviour
{
	public Suspension right;

	public Suspension left;

	public float antiRoll = 5000f;

	private Rigidbody bodyRb;

	private void Awake()
	{
		bodyRb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		StabilizerBars();
	}

	private void StabilizerBars()
	{
		float num = 1f;
		float num2 = 1f;
		num2 = ((!right.grounded) ? 1f : right.lastCompression);
		num = ((!left.grounded) ? 1f : left.lastCompression);
		float num3 = (num - num2) * antiRoll;
		if (right.grounded)
		{
			bodyRb.AddForceAtPosition(right.transform.up * (0f - num3), right.transform.position);
		}
		if (left.grounded)
		{
			bodyRb.AddForceAtPosition(left.transform.up * num3, left.transform.position);
		}
	}
}
