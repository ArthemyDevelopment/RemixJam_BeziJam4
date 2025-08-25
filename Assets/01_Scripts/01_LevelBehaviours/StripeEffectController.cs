using System.Collections;
using System.Collections.Generic;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class StripeEffectController : MonoBehaviour
{

    [BoxGroup("Effect Settings")]public List<GameObject> Stripes = new List<GameObject>();
    [BoxGroup("Effect Settings")]public int StripesToActive;
    [BoxGroup("Effect Settings"), SerializeField]private int ticksToShift;
    [BoxGroup("DEBUG"), ReadOnly]private List<GameObject> curActiveStripes = new List<GameObject>();
    [BoxGroup("DEBUG"), SerializeField, ReadOnly]private int curTick;

    private Queue<GameObject> StripesOrder = new Queue<GameObject>();



    public void SetEffect(bool state)
    {
        switch (state)
        {
            case true:
                ShuffleListToQueue();
                TurnOnStripes();
                GameTickManager.current.OnGameTick += CheckStripesShift;
                break;
            case false:
                GameTickManager.current.OnGameTick -= CheckStripesShift;
                TurnOffStripes();
                break;
        }
    }


    void ShuffleListToQueue()
    {
        StripesOrder.Clear();
        StripesOrder = ScriptsTools.ShuffleList(Stripes);
    }

    void CheckStripesShift()
    {
        curTick++;

        if (curTick >= ticksToShift)
        {
            TurnOffStripes();
            curActiveStripes.Clear();
            TurnOnStripes();
            curTick = 0;
        }
    }

    void TurnOffStripes()
    {
        foreach (var stripe in curActiveStripes)
        {
            stripe.SetActive(false);
            StripesOrder.Enqueue(stripe);
        }
    }

    void TurnOnStripes()
    {
        for (int i = 0; i < StripesToActive; i++)
        {
            GameObject temp = StripesOrder.Dequeue();
            temp.SetActive(true);
            curActiveStripes.Add(temp);
        }
    }
    
}
