using System;
using UnityEngine;
using System.Collections.Generic;

public class Radio : MonoBehaviour, IInteractable
{
    public List<AudioClip> audioClips;
    private AudioSource audioSource;
    private int index=-1;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        index++;
        if (index == audioClips.Count)
            index = -1;

        audioSource.Stop();
        
        if (index == -1)
            return;
        
        audioSource.clip = audioClips[index];
        audioSource.Play();

    }

    public string GetInteractionPrompt()
    {
        return index == -1 ? "Turn on" : index==audioClips.Count-1 ? "Turn off" :  "Change Station";
    }
}
