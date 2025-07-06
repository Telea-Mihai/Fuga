using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public enum StoryStage
    {
        Prologue,
        MeetFixer,
        CollectIDs,
        StealCar,
        CrossDanube,
        BorderForest,
        FreedomEnding,
        CaughtEnding
    }

    public StoryStage currentStage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AdvanceStory(StoryStage nextStage)
    {
        currentStage = nextStage;
        Debug.Log("Story advanced to: " + currentStage.ToString());


        if (currentStage == StoryStage.MeetFixer)
        {

        }
    }
}