using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 5f;
    public float gravidade = -18f;
    public float forcaPulo = 6f;

    private CharacterController controller;
    private Vector2 inputMovimento;
    private Vector3 velocidadeVertical;

    void Start() => controller = GetComponent<CharacterController>();

    void Update()
    {
        Mover();
        AplicarGravidadeEPulo();
    }

    public void OnMove(InputValue valor) => inputMovimento = valor.Get<Vector2>();

    public void OnJump(InputValue valor)
    {
        if (controller.isGrounded && valor.isPressed)
            velocidadeVertical.y = Mathf.Sqrt(forcaPulo * -2f * gravidade);
    }

    void Mover()
    {
        Vector3 direcao = (transform.right * inputMovimento.x + transform.forward * inputMovimento.y).normalized;
        controller.Move(direcao * velocidade * Time.deltaTime);
    }

    void AplicarGravidadeEPulo()
    {
        if (controller.isGrounded && velocidadeVertical.y < 0)
            velocidadeVertical.y = -2f;

        velocidadeVertical.y += gravidade * Time.deltaTime;
        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}