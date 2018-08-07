using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

namespace Final
{
    public class Player : MonoBehaviour
    {
        public static Player player;

        Rigidbody rb;

        public float movementSpeed, rotationSpeed, jumpForce, healthMax;

        [HideInInspector]
        public float camUpAngle, health;

        public bool onGround;

        public LayerMask groundMask;

        public bool hasKey;

        public PostProcessVolume volume;
        Vignette vignette;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            player = this;
            hasKey = false;
        }

        // Use this for initialization
        void Start()
        {
            DisableCursor();

            health = healthMax;

            volume.profile.TryGetSettings<Vignette>(out vignette);
            vignette.enabled.value = true;
            vignette.intensity.value = 0;
        }

        // Update is called once per frame
        void Update()
        {
            onGround = OnGround();
            RotateCam();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DisableCursor();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onGround)
                {
                    Jump();
                }
            }

        }

        void FixedUpdate()
        {
            Movement();
        }

        void Movement()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 modInput = input * Time.deltaTime * movementSpeed;

            rb.MovePosition(transform.position + transform.forward * modInput.y + transform.right * modInput.x);
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, Vector3.up));
        }

        void RotateCam()
        {
            camUpAngle -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            Quaternion rot = Camera.main.transform.localRotation;
            rot = Quaternion.Euler(Mathf.Clamp(camUpAngle, -90, 90), 0, 0);
            Camera.main.transform.localRotation = rot;
        }

        void DisableCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

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

        public void Kill()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

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

        void AdjustVignette ()
        {
            vignette.intensity.value = Mathf.InverseLerp(0, 0.5f, 0.5f - (health * 0.0075f));
            print("V. Intensity: " + vignette.intensity.value + ", Health: " + health);
        }
        public void GetKey ()
        {
            hasKey = true;
        }
    }
}

