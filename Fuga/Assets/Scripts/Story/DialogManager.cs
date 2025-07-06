using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.03f;

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isTyping = false;
    public bool inDialog;
    public AudioSource voice;
    public float voicePitch;
    public AudioClip voiceClip; 

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(List<string> lines)
    {
        inDialog = true;
        dialogueQueue.Clear();
        foreach (string line in lines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (isTyping) return;

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string nextLine = dialogueQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeLine(nextLine));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;

            if (char.IsLetterOrDigit(letter))
            {
                voice.pitch = voicePitch + Random.Range(-0.1f, 0.1f);
                voice.PlayOneShot(voiceClip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        inDialog = false;
    }
}