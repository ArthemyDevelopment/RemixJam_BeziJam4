using UnityEngine;

public class DemonHandController : MonoBehaviour
{
    public void OnOpenHand()
    {
        ChaosEffectsManager.current.ApplyEffect();
    }
}
