using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public bool CanMove = true;

    public float Stamina = 100;
    public Slider StaminaSlider;
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -20f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private float rotationX = 0;
    private float targetSpeed;
    public float currentSpeed;

    private AudioSource audioSource;
    public AudioClip[] walkSound;

    float staminaTimer = 0;
    float refreshTimer = 0;
    float spendTimer = 0;

    private void Awake()
    {
        Instance = this;
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        if (CanMove)
        {
            HandleJump();
            HandleCameraRotation();
            HandleMovement();
        }

    }

    public void Step(int isRunning)
    {
        if (!audioSource) return;
        if (isRunning == 1 && MathF.Round(currentSpeed) == runSpeed)
        {

            audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(walkSound[UnityEngine.Random.Range(0, walkSound.Length)]);
        }
        else if (isRunning == 0)
        {
            if (MathF.Round(currentSpeed) == walkSpeed)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(walkSound[UnityEngine.Random.Range(0, walkSound.Length)]);

            }

        }

    }

    public void HandleJump()
    {
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            currentVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleMovement()
    {
        bool isGrounded = characterController.isGrounded;

        // Сброс вертикальной скорости при нахождении на земле
        if (isGrounded && currentVelocity.y < 0)
        {
            currentVelocity.y = -2f;
        }

        // Определение целевой скорости (ходьба/бег)

        // targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
        {
            targetSpeed = runSpeed;
        }
        else targetSpeed = walkSpeed;

        // Плавное изменение скорости (ускорение/замедление)


        // Получение ввода и расчет направления
        float horizontal = Input.GetAxisRaw("Horizontal"); // Используем Raw для мгновенного отклика
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude <= 0) currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);
        else currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Преобразуем локальное направление в мировые координаты
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (currentSpeed > walkSpeed && Input.GetKey(KeyCode.LeftShift))
        {

            if (Stamina > 0)
            {
                spendTimer += Time.deltaTime;
                if (spendTimer > 0.1f)
                {
                    Stamina -= 0.5f;
                    spendTimer = 0;
                }

                staminaTimer = 0;
            }

        }

        if (currentSpeed < runSpeed && !Input.GetKey(KeyCode.LeftShift))
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer >= 5 && Stamina < 100)
            {
                refreshTimer += Time.deltaTime;
                if (refreshTimer > 0.1f)
                {
                    Stamina += 3;
                    refreshTimer = 0;
                }

            }
        }

        if (StaminaSlider) StaminaSlider.value = Stamina;

        // Перемещение
        Vector3 move = moveDirection.normalized * currentSpeed * Time.deltaTime;
        characterController.Move(move);

        // Гравитация
        currentVelocity.y += gravity * Time.deltaTime;
        characterController.Move(currentVelocity * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        float rotationY = Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation *= Quaternion.Euler(0, rotationY, 0);
    }
}