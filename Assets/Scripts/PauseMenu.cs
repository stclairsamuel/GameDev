using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputActionAsset InputActions;

    public GameObject player;

    public InputActionMap playerActions;
    public InputActionMap uiActions;

    public GameObject pauseFilter;
    public GameObject changeAbilitiesMenu;

    public List<GameObject> menues;

    public InputAction pauseAction;
    public InputAction continueAction;

    void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Disable();
    }
    void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        player = GameObject.FindWithTag("Player");

        playerActions = InputActions.FindActionMap("Player");
        uiActions = InputActions.FindActionMap("UI");

        pauseAction = InputActions.FindAction("Pause");
        continueAction = InputActions.FindAction("Continue");

        menues = new List<GameObject> {
            pauseFilter,
            changeAbilitiesMenu
        };

        pauseFilter.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
    }

    void TakeInput()
    {
        if (pauseAction.WasPerformedThisFrame())
        {
            Pause();
        }

        if (continueAction.WasPerformedThisFrame())
        {
            Continue();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        pauseFilter.SetActive(true);
        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Enable();
    }

    public void Continue()
    {
        if (pauseFilter.activeSelf)
        {
            Time.timeScale = 1;
            pauseFilter.SetActive(false);
            InputActions.FindActionMap("Player").Enable();
            InputActions.FindActionMap("UI").Disable();        
        }
        else
        {
            SwitchScreen(pauseFilter);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SwitchScreen(GameObject screen)
    {
        foreach (GameObject g in menues)
        {
            if (g == screen)
                g.SetActive(true);
            else
                g.SetActive(false);
        }
    }
    
}
