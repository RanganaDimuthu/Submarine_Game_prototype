using UnityEngine;
using UnityEngine.Events;

public class QuizManager : MonoBehaviour
{
    [SerializeField] ESGIslandController q1, q2, q3, q4;
    [SerializeField] UnityEvent Allanwserd;
    bool done = false;
    private void Update()
    {
        if((q1.isAnswered &&  q2.isAnswered && q3.isAnswered && q4.isAnswered) && done == false)
        {
            done = true;
            Allanwserd.Invoke();
        }
    }
}
