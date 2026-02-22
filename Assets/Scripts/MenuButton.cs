using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    Dictionary<int, string> sceneMap;

    // Start is called before the first frame update
    void Awake()
    {
        sceneMap = new Dictionary<int, string> {
            {1 , "Scene1"},
            {2 , "EpicScene"}
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartClick(int scene)
    {
        SceneManager.LoadScene(sceneMap[scene]);
    }


}
