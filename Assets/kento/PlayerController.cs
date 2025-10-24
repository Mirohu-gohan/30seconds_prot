using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField,Header("歩きスピード")]     private float speed;
    [SerializeField,Header("ダッシュスピード")] private float dash_speed;
    [SerializeField,Header("ブリンクスピード")] private float brink_speed;

    [SerializeField] private float currentSpeed;
    private Vector2 inputVer;

    private bool isBrink = false;
    private float mangitude;

    Rigidbody rb;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isBrink)
        {
            inputVer = context.ReadValue<Vector2>();
        }
    }

    public void OnDash(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            currentSpeed = speed * dash_speed;
        }
        else if (context.canceled)
        {
            currentSpeed = speed;
        }
    }

    public void OnBrink(InputAction.CallbackContext context)
    {
        if (context.performed )
        {
           isBrink = true;
            Brink();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        currentSpeed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        if (isBrink && mangitude > 0)
        {
            isBrink = false ;
        }
    }

    void Move()
    {
        if(!isBrink) {return; } 

        Vector3 move = new Vector3(inputVer.x, 0f, inputVer.y) * currentSpeed * Time.deltaTime;
        //transform.position += move;
        rb.MovePosition(rb.position + move);

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = rot;
        }
    }

    void Brink()
    {
        rb.AddForce(transform.forward * brink_speed, ForceMode.VelocityChange);
        currentSpeed = 0;

        mangitude = rb.linearVelocity.magnitude;
    }
}
