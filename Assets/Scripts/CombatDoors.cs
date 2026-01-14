using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatDoors : MonoBehaviour
{
    public RoomSpawnManager mySpawner;

    private Coroutine doorCoroutine;

    private Animator anim;

    private Collider2D col;
    private Rigidbody2D rb;

    private bool closed;

    public float doorTime;

    private bool isOpening;
    private bool isClosing;

    void OnEnable()
    {
        mySpawner.CloseDoors += CloseMyDoors;
        mySpawner.OpenDoors += OpenMyDoors;
    }
    void OnDisable()
    {
        mySpawner.CloseDoors -= CloseMyDoors;
        mySpawner.OpenDoors -= OpenMyDoors;
    }

    // Start is called before the first frame update
    void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        col.enabled = closed;

        Anim();
    }

    private void CloseMyDoors()
    {
        doorCoroutine = StartCoroutine(DoorCloseAnim());
    }

    private IEnumerator DoorCloseAnim()
    {
        isClosing = true;
        yield return new WaitForSeconds(doorTime);
        isClosing = false;
        closed = true;
    }

    private void OpenMyDoors()
    {
        doorCoroutine = StartCoroutine(OpenDoorAnim());
    }

    private IEnumerator OpenDoorAnim()
    {
        isOpening = true;
        yield return new WaitForSeconds(doorTime);
        isOpening = false;
        closed = false;
    }

    private void Anim()
    {
        anim.SetBool("isOpening", isOpening);
        anim.SetBool("isClosing", isClosing);
    }
}
