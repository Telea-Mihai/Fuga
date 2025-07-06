using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public Text dialogueText;
    public GameObject dialoguePanel;

    private Queue<string> lines = new Queue<string>();

    public void StartDialogue(string[] dialogueLines)
    {
        dialoguePanel.SetActive(true);
        lines.Clear();

        foreach (string line in dialogueLines)
        {
            lines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string nextLine = lines.Dequeue();
        dialogueText.text = nextLine;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}