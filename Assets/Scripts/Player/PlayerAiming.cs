using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAiming : MonoBehaviour
{
    public GameObject dotPrefab;              // Circle sprite prefab
    public int dotCount = 20;                 // How many dots in the line
    public float dotSpacing = 0.5f;           // Distance between each dot
    public float minDotScale = 0.2f;          // Smallest dot size at the end
    public float maxDotScale = 1.0f;          // Largest dot size at the start
    public GameObject projectilePrefab;       // The thing to throw
    public float projectileSpeed = 10f;

    private List<GameObject> dots = new List<GameObject>();
    private Camera cam;
    private bool isAiming = false;

    private PlayerControls inputActions;

    void Awake()
    {
        inputActions = new PlayerControls();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Gameplay.Aim.started += ctx => StartAiming();
        inputActions.Gameplay.Aim.canceled += ctx => StopAiming();
        inputActions.Gameplay.Fire.performed += ctx => OnFire();
    }

    void OnDisable()
    {
        inputActions.Disable();

        inputActions.Gameplay.Aim.started -= ctx => StartAiming();
        inputActions.Gameplay.Aim.canceled -= ctx => StopAiming();
        inputActions.Gameplay.Fire.performed -= ctx => OnFire();
    }

    void Start()
    {
        cam = Camera.main;
        CreateDots();
    }

    void Update()
    {
        if (isAiming)
        {
            UpdateDots();
        }
    }

    void StartAiming()
    {
        isAiming = true;
        EnableDots();
    }

    void StopAiming()
    {
        isAiming = false;
        DisableDots();
    }

    void OnFire()
    {
        if (!isAiming) return;

        Vector3 target = GetMouseWorldPosition();
        ThrowProjectile(target);
        DisableDots();
        isAiming = false;
    }


    void CreateDots()
    {
        for (int i = 0; i < dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.transform.parent = this.transform; // optional
            dot.SetActive(false);
            dots.Add(dot);
        }
    }

    void EnableDots()
    {
        foreach (GameObject dot in dots)
        {
            dot.SetActive(true);
        }
    }

    void DisableDots()
    {
        foreach (GameObject dot in dots)
        {
            dot.SetActive(false);
        }
    }

    void UpdateDots()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GetMouseWorldPosition();

        // Keep Y the same (top-down logic)
        endPos.y = startPos.y;

        Vector3 direction = (endPos - startPos).normalized;
        float totalDistance = Vector3.Distance(startPos, endPos);

        for (int i = 0; i < dots.Count; i++)
        {
            float distance = i * dotSpacing;

            if (distance <= totalDistance)
            {
                Vector3 pos = startPos + direction * distance;
                pos.y = startPos.y;
                dots[i].transform.position = pos;

                float t = (float)i / (dots.Count - 1);
                float scale = Mathf.Lerp(maxDotScale, minDotScale, t);
                dots[i].transform.localScale = new Vector3(scale, scale, scale);

                dots[i].SetActive(true); // show visible dot
            }
            else
            {
                dots[i].SetActive(false); // hide beyond mouse position
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue()); // if using new input system
        Plane groundPlane = new Plane(Vector3.up, transform.position); // XZ plane at player's Y

        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        return transform.position; // fallback
    }

    void ThrowProjectile(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        // Offset the spawn position slightly in the throw direction
        float spawnOffset = 1.0f; // tweak this value as needed
        Vector3 spawnPos = transform.position + direction * spawnOffset;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}
