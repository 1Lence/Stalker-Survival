using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    public Vector3 playerMoveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;
    }
    
    void FixedUpdate(){
        rb.linearVelocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }
}