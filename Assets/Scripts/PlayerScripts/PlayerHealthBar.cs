using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBarLegacy : MonoBehaviour
{
    /*
    public GameObject heartPrefab;
    private PlayerTracker pTracker;

    List<PlayerHealth> hearts = new List<PlayerHealth>();

    // Start is called before the first frame update
    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        pTracker = player.GetComponent<PlayerTracker>();
    }

    private void Start()
    {
        DrawHearts();
    }

    private void OnEnable()
    {
        pTracker.OnPlayerDamaged += DrawHearts;
    }

    private void OnDisable()
    {
        pTracker.OnPlayerDamaged -= DrawHearts;
    }

    public void DrawHearts()
    {
        ClearHearts();

        float maxHealthRemainder = pTracker.startingHealth % 4;
        int heartsToMake = (int)((pTracker.startingHealth / 4) + maxHealthRemainder);

        for(int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for(int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(pTracker.currentHealth - (i*4), 0, 4);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        PlayerHealth heartComponent = newHeart.GetComponent<PlayerHealth>();
        heartComponent.SetHeartImage(HeartStatus.Empty);

        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
