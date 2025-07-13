using UnityEngine;
using System.Collections;

public class BlackScreenFader : MonoBehaviour
{
   
    public CanvasGroup Fader;
    public float fadeDuration = 1f;
    [Range(1, 10)] public float FinishfadeTime = 3.5f;
    bool isstarted = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var allInstances = Object.FindObjectsByType<BlackScreenFader>(FindObjectsSortMode.None);

        // More than one? Then destroy self
        if (allInstances.Length > 1)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
      

       if(isstarted == false)
       {
            StartCoroutine(FadeInAndOut(Fader, fadeDuration, FinishfadeTime)); 
            
            isstarted = true;
       }

        
    }
    public void RunAfadeAnimation()
    {
        StartCoroutine(FadeInAndOut(Fader, fadeDuration, FinishfadeTime));
    }
    IEnumerator FadeInAndOut(CanvasGroup group, float duration, float waitTime)
    {
        float t = 0f;

        // Fade In
        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
        group.alpha = 1f;
        
        // Wait fully visible
        yield return new WaitForSeconds(waitTime);

        t = 0f;

        // Fade Out
        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }
        group.alpha = 0f;
    }


}
