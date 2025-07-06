using UnityEngine;

public class StartStop : MonoBehaviour, IInteractable
{
    public Car car;

    public void Interact(PlayerInteraction playerInteraction)
    {
        if(car.started)
            car.stopCar();
        else car.startCar();
    }
    
    public string GetInteractionPrompt()
    {
        return car.started ? "Stop engine" : "Start engine";
    }
}
