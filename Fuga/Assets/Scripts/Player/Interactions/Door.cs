using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public GameObject door;


    public float time;
    public Quaternion rotationClosed;
    public Quaternion rotationOpen;
    
    public bool sliding = false;
    public Vector3 positionOpen;
    public Vector3 positionClose;

    private bool open = false;
    
    private Coroutine animCoroutine;

    public void Interact(PlayerInteraction interaction)
    {
        open = !open;

        animCoroutine = StartCoroutine(AnimateDoor());
    }
    
    private IEnumerator AnimateDoor()
    {
        float elapsed = 0f;
        float duration = time;

        Vector3 startPos = transform.position;
        Vector3 endPos = open ? positionOpen : positionClose;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = open ? rotationOpen : rotationClosed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (sliding)
                transform.position = Vector3.Lerp(startPos, endPos, t);
            else
                transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }
        
        if (sliding)
            transform.position = endPos;
        else
            transform.rotation = endRot;
    }
    public String GetInteractionPrompt() => open ? "Press E to Close" : "Press E to Open";
}
