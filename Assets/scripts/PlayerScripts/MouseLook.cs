using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float sensibilidade = 0.1f;
    public float limiteVertical = 80f;

    private float rotacaoX = 0f;
    private Transform playerTransform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerTransform = transform.parent;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue() * sensibilidade;

        playerTransform.Rotate(0f, delta.x, 0f);

        rotacaoX -= delta.y;
        rotacaoX = Mathf.Clamp(rotacaoX, -limiteVertical, limiteVertical);
        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}