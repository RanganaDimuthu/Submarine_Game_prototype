using UnityEngine;
using UnityEngine.Events;

public class TimeBasedEvents : MonoBehaviour
{
    [SerializeField] float Duraction;
    [SerializeField] UnityEvent start, End;

    void Start()
    {
        start.Invoke();
        Invoke("EndingTime", Duraction);
    }

    private void EndingTime()
    {
        End.Invoke();
    }
}
