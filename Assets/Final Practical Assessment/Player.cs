using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

namespace Final
{
    public class Player : MonoBehaviour
    {
        //singleton instance
        public static Player player;
        //reference to physics component
        Rigidbody rb;
        //self explanatory variables
        public float movementSpeed, rotationSpeed, jumpForce, healthMax;
        //camUp tracks vertical rotation of the camera. health tracks ... health
        [HideInInspector]
        public float camUpAngle, health;
        //used to track whether the player is on the ground
        public bool onGround;
        //helps optimise the OnGround raycast
        public LayerMask groundMask;
        //stage tracking
        public bool hasKey, hasPrincess;
        //variables for modifying post processing
        public PostProcessVolume volume;
        Vignette vignette;

        private void Awake()
        {
            //get the physis, assign the singleton and make sure that the stages are false
            rb = GetComponent<Rigidbody>();
            player = this;
            hasKey = false;
            hasPrincess = false;
        }

        // Use this for initialization
        void Start()
        {
            //disable and lock the cursor
            DisableCursor();
            //set health to the max
            health = healthMax;
            //get the vignette from the post process volume controller, and store it 
            volume.profile.TryGetSettings<Vignette>(out vignette);
            //make sure vignette is enabled
            vignette.enabled.value = true;
            //Set intensity is 0, aka off
            vignette.intensity.value = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //Get whether the player is on the ground
            onGround = OnGround();
            //Rotate the camera according to mouse position
            RotateCam();

            //if the player presses tab, make sure the cursor is invisible and locked
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DisableCursor();
            }

            //if the player presses space and is on the ground, they can jump. 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onGround)
                {
                    Jump();
                }
            }

            //Get mouse click
            if (Input.GetMouseButtonDown(0))
            {
                //raycast from the camera, forward. 
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10))
                {
                    //if we hit an enemy, do nothing. Because i havent implemented weapons yet.
                    Enemy e = hit.transform.GetComponent<Enemy>();
                    if (e != null)
                    {

                    }
                }
            }

        }
        //On physics update, evaluate movement. 
        void FixedUpdate()
        {
            Movement();
        }

        //... movement
        void Movement()
        {
            //Get input from w,a,s,d and arrow keys. 
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //modify the input with movement speed and then balance for framerate
            Vector2 modInput = input * Time.deltaTime * movementSpeed;
            //use the physics component to rotate and move the player.
            rb.MovePosition(transform.position + transform.forward * modInput.y + transform.right * modInput.x);
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, Vector3.up));
        }
        //camera rotation is separate from bodily for vertical rotation. 
        void RotateCam()
        {
            //use camUpAngle variable to calculate the angle the camera should be rotated to.
            camUpAngle -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            //Get the current rotation
            Quaternion rot = Camera.main.transform.localRotation;
            //apply our angle in euler, clamping from -90 to 90
            rot = Quaternion.Euler(Mathf.Clamp(camUpAngle, -90, 90), 0, 0);
            //apply rotation to camera
            Camera.main.transform.localRotation = rot;
        }
        //set cursor invisible, then lock to the middle of the screen. 
        void DisableCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //apply an upward force to the player. 
        void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
        //cast a ray downward looking for ground. return whether we found it. 
        bool OnGround()
        {
            Ray ray = new Ray(Camera.main.transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2.1f, groundMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //if we die, reload the scene
        public void Kill()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        //change health depending on input. clamp health between nothing and max. 
        //Shake the camera, then decide whether we should die, and adjust vignette based on health. 
        public void Damage(float amount)
        {
            print("Damaging player: " + amount);
            health -= amount;
            health = Mathf.Clamp(health, 0, healthMax);
            CameraShake.shake.StartShake(0.1f);
            if (health <= 0)
            {
                Kill();
            }
            AdjustVignette();
        }
        //do the damage thing, but in reverse. 
        public void Heal(float amount)
        {
            health += amount;
            health = Mathf.Clamp(health, 0, healthMax);
            if (health <= 0)
            {
                Kill();
            }
            AdjustVignette();
        }
        //set vignette using health. I did this entirely by guess and check, but it works. 
        void AdjustVignette ()
        {
            vignette.intensity.value = Mathf.InverseLerp(0, 0.5f, 0.5f - (health * 0.0075f));
            print("V. Intensity: " + vignette.intensity.value + ", Health: " + health);
        }
    }
}

