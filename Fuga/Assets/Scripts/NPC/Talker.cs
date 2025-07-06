using UnityEngine;
using System.Collections.Generic;

public class Talker : MonoBehaviour, IInteractable
{
    public List<string> talkAnimations;
    public Animator animator;
    public float voicePitch;
    private AudioSource voice;

    [TextArea(2, 5)]
    public List<string> dialogueLines;

    private void Awake()
    {
        List<Rigidbody> rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        voice = gameObject.GetComponent<AudioSource>();
    }
    
    public void Interact(PlayerInteraction playerInteraction)
    {
        if (talkAnimations.Count > 0)
        {
            string anim = talkAnimations[Random.Range(0, talkAnimations.Count)];
            animator.Play(anim);
        }

        DialogManager.Instance.voice = voice;
        DialogManager.Instance.StartDialogue(dialogueLines);
        DialogManager.Instance.voicePitch = voicePitch;
    }

    public string GetInteractionPrompt()
    {
        return "Press E to talk";
    }
}
