using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayCycleStalker : MonoBehaviour
{
    public List<int> statesToWaitFor = new();

    public UnityEvent onCycleReady = new();

    private void CheckCycle()
    {
        bool found = false;
        for(int i = 0; i < statesToWaitFor.Count; i++)
        {
            if (GameplayCycle.GetState() == statesToWaitFor[i])
            {
                found = true;
                break;
            }
        }

        if(found)
        {
            statesToWaitFor.Clear();
            onCycleReady?.Invoke();
            gameObject.SetActive(false);
        }
    }
    public void AddTarget(int target)
    {
        statesToWaitFor.Add(target);
    }

    private void OnEnable()
    {
        GameplayCycle.OnCycleTaskFinished.AddListener(CheckCycle);
    }

    private void OnDisable()
    {
        GameplayCycle.OnCycleTaskFinished.RemoveListener(CheckCycle);
    }
}
