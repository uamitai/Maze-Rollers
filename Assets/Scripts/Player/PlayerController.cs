//the playercontroller script influences the player's movement
//takes the player input and calculates velocity every frame
//controlls the position and rotation of the player, as well as rotates the camera and rolls the ball


using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] public float speed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float camLimit;
    [SerializeField] private float rollSpeed;

    [Header("")]
    [SerializeField] private Transform model;
    [SerializeField] public float staminaSpeed;
    [SerializeField] private float speedMultiplier;

    private float stamina;
    private Rigidbody rb;
    private float mouseX, rotation;
    private Vector3 x, z, vel;
    private const float maxStamina = 100;

    // Start is called before the first frame update
    void Start()
    {
        stamina = maxStamina;
        mouseSensitivity = RoomManager.singleton.mouseSensitivity * 2;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Player.UI.pauseOn || !GetComponent<Player>().isAlive)
        {
            //player cannot move on pause
            return;
        }

        GetInput();

        //combine axes for velocity
        vel = (x + z).normalized * speed; 

        //sprint
        if(Input.GetButton("Jump") && vel != Vector3.zero && stamina >= 5)
        {
            vel *= speedMultiplier;
            stamina -= 2 * staminaSpeed * Time.deltaTime;
        }
        else
        {
            stamina += staminaSpeed * Time.deltaTime;
        }

        rotation = Mathf.Clamp(rotation, -camLimit, camLimit); //clamp rotation
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);        //clamp stamina
    }

    void FixedUpdate()
    {
        if(Player.UI.pauseOn || !GetComponent<Player>().isAlive)
        {
            //player cannot move on pause
            return;
        }

        Move();

        //updare stamina meter
        Player.UI.SetStamina(stamina / maxStamina);
    }

    void GetInput()
    {
        //movement inputs
        x = transform.right * Input.GetAxisRaw("Horizontal");
        z = transform.forward * Input.GetAxisRaw("Vertical");

        //rotation inputs
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotation -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
    }

    void Move()
    {
        //move
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);

        //roll ball
        model.Rotate(new Vector3(vel.z, 0f, -vel.x) * rollSpeed * vel.magnitude * Time.fixedDeltaTime, Space.World);

        //rotate camera
        Player.localPlayer.cam.localRotation = Quaternion.Euler(rotation, 0f, 0f);

        //rotate player
        transform.Rotate(Vector3.up * mouseX);
    }
}
