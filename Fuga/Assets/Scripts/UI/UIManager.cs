using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image itemDot;
    public Text interactionName;
    private PlayerInteraction interaction;
    private FpsController fpsController;

    private Vector3 defaultScale;

    private void Start()
    {
        fpsController = GetComponent<FpsController>();
        interaction = GetComponent<PlayerInteraction>();
        
        defaultScale = itemDot.transform.localScale;
    }

    private void Update()
    {
        if (interaction.hovering)
        {
            itemDot.transform.localScale = Vector3.Lerp(itemDot.transform.localScale, defaultScale * 1.5f, 0.5f);
        }
        else
        {
            itemDot.transform.localScale = Vector3.Lerp(itemDot.transform.localScale, defaultScale, 0.5f);
        }
    }
}
