using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public float realValue;
    public float displayValue;

    public GameObject player;
    public PlayerTracker pTracker;

    public float maxHealth;
    public float currentHealth;


    // Start is called before the first frame update
    void Awake()
    {
        realValue = 1;
        displayValue = realValue;

        player = GameObject.FindWithTag("Player");
        pTracker = player.GetComponent<PlayerTracker>();
    }

    void Start()
    {
        maxHealth = pTracker.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = displayValue;

        currentHealth = pTracker.currentHealth;

        realValue = (currentHealth/maxHealth);

        if (displayValue != realValue)
        {
            displayValue = Mathf.Lerp(displayValue, realValue, 0.05f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            realValue -= 0.1f;
        }
    }

    
}
