using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    float standardTimeScale;

    // Start is called before the first frame update
    void Start()
    {
        standardTimeScale = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestFreeze(float freezeDuration)
    {
        StartCoroutine(DoFreeze(freezeDuration));
    }

    private IEnumerator DoFreeze(float freezeDuration)
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
        yield return new WaitForSecondsRealtime(freezeDuration);
        Time.timeScale = standardTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
