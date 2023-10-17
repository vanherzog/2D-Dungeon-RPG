using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float runMultiplier = 1.5f;

    InputActions playerControls;

    Rigidbody2D rb;
    Collider2D col;
    Animator anim;

    Vector2 moveVector;
    bool isGod = false;
    bool isSprinting = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        playerControls = new InputActions();         
    }

    private void OnEnable()
    {
        playerControls.Player.Sprint.performed += _ => isSprinting = true;
        playerControls.Player.Sprint.canceled += _ => isSprinting = false;
        playerControls.Player.GodMode.performed += _ => ToggleGodMode();

        playerControls.Enable();
    }

    private void OnDisable() => playerControls.Disable();

    void ToggleGodMode()
    {
        isGod = !isGod;
        col.enabled = !isGod;
    }

    private void FixedUpdate()
    {
        moveVector = playerControls.Player.Move.ReadValue<Vector2>();
        if (isGod) moveVector *= 5f;
        if (isSprinting) moveVector *= runMultiplier;

        rb.velocity = speed * moveVector;
    }
   
    private void Update() => SetAnimations();

    private void SetAnimations()
    { 
        if (moveVector != Vector2.zero)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("MoveX", moveVector.x);
            anim.SetFloat("MoveY", moveVector.y);
        }
        else
            anim.SetBool("IsMoving", false);
    }
}