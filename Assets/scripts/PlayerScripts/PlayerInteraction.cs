using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configurações de Interação")]
    [SerializeField] private float alcance = 3f;
    [SerializeField] private LayerMask camadasInterativas;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TMP_Text textoAvisoInteracao;
    [SerializeField] private TMP_Text textoAvisoInventarioCheio;
    [SerializeField] private float duracaoAvisoInventarioCheio = 2f;

    [Header("Input System")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Visualização na Mão / Ecrã")]
    [SerializeField] private Transform itemHolder;

    private Inventory inventory;
    private int slotSelecionado = 0; 
    private GameObject itemVisualAtual;
    private Coroutine esconderAvisoInventarioCoroutine;

    public int SlotSelecionado => slotSelecionado;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();

        if (inventory != null)
        {
            inventory.OnInventoryChanged += AtualizarItemNaMao;
        }

        AtualizarItemNaMao();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= AtualizarItemNaMao;
        }
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.Disable();
    }

    private void Update()
    {
        VerificarSelecaoHotbar();
        VerificarDropItem();
        ProcessarRaycastInteracao();
    }

    private void VerificarSelecaoHotbar()
    {
        if (Keyboard.current == null) return;

        int slotAnterior = slotSelecionado;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) slotSelecionado = 0;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) slotSelecionado = 1;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) slotSelecionado = 2;
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) slotSelecionado = 3;
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) slotSelecionado = 4;

        if (slotAnterior != slotSelecionado)
        {
            AtualizarItemNaMao();
        }
    }

    public void AtualizarItemNaMao()
    {
        if (itemVisualAtual != null)
        {
            Destroy(itemVisualAtual);
        }

        if (inventory == null || itemHolder == null) return;
        if (slotSelecionado < 0 || slotSelecionado >= inventory.Slots.Count) return;

        InventorySlot slot = inventory.Slots[slotSelecionado];

        if (!slot.IsEmpty && slot.Item.EquipmentPrefab != null)
        {
            Vector3 escalaOriginal = slot.Item.EquipmentPrefab.transform.localScale;

            itemVisualAtual = Instantiate(slot.Item.EquipmentPrefab, itemHolder);
            itemVisualAtual.transform.localPosition = Vector3.zero;
            itemVisualAtual.transform.localRotation = Quaternion.identity;
            itemVisualAtual.transform.localScale = escalaOriginal;

            Collider col = itemVisualAtual.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            PickupItem pickup = itemVisualAtual.GetComponent<PickupItem>();
            if (pickup != null) Destroy(pickup);

            Rigidbody rb = itemVisualAtual.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }

    private void VerificarDropItem()
    {
        if (Keyboard.current == null || inventory == null) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            LargarItemSelecionado();
        }
    }

    private void LargarItemSelecionado()
    {
        if (slotSelecionado < 0 || slotSelecionado >= inventory.Slots.Count) return;

        InventorySlot slot = inventory.Slots[slotSelecionado];
        if (slot.IsEmpty) return;

        ItemData itemParaLargar = slot.Item;

        if (itemParaLargar.EquipmentPrefab != null)
        {
            Vector3 posDrop = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
            GameObject itemDropado = Instantiate(itemParaLargar.EquipmentPrefab, posDrop, Quaternion.identity);

            PickupItem pickup = itemDropado.GetComponent<PickupItem>();
            if (pickup == null) pickup = itemDropado.AddComponent<PickupItem>();

            Collider col = itemDropado.GetComponent<Collider>();
            if (col == null) col = itemDropado.AddComponent<SphereCollider>();
            col.enabled = true;

            Rigidbody rb = itemDropado.GetComponent<Rigidbody>();
            if (rb == null) rb = itemDropado.AddComponent<Rigidbody>();
            
            rb.isKinematic = false;
            rb.useGravity = true;

            inventory.RemoveFromSlot(slotSelecionado, 1);
        }
    }

    private void ProcessarRaycastInteracao()
    {
        Transform origem = (cameraTransform != null) ? cameraTransform : transform;
        RaycastHit hit;

        if (Physics.Raycast(origem.position, origem.forward, out hit, alcance, camadasInterativas))
        {
            PickupItem pickupItem = hit.collider.GetComponent<PickupItem>();

            if (pickupItem != null)
            {
                if (textoAvisoInteracao != null)
                {
                    textoAvisoInteracao.gameObject.SetActive(true);
                    textoAvisoInteracao.text = "Pressionar [E] para apanhar";
                }

                bool interactPressionado = (interactAction != null && interactAction.action.WasPressedThisFrame()) || (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

                if (interactPressionado)
                {
                    int sobra = inventory.AddItem(pickupItem.ItemData, pickupItem.Quantity);

                    if (sobra <= 0)
                    {
                        Destroy(pickupItem.gameObject);
                    }
                    else
                    {
                        pickupItem.SetQuantity(sobra);
                        MostrarAvisoInventarioCheio();
                    }
                }
                return;
            }
        }

        if (textoAvisoInteracao != null)
        {
            textoAvisoInteracao.gameObject.SetActive(false);
        }
    }

    private void MostrarAvisoInventarioCheio()
    {
        if (textoAvisoInventarioCheio == null) return;

        textoAvisoInventarioCheio.text = "Inventário cheio";
        textoAvisoInventarioCheio.gameObject.SetActive(true);

        if (esconderAvisoInventarioCoroutine != null)
            StopCoroutine(esconderAvisoInventarioCoroutine);

        esconderAvisoInventarioCoroutine = StartCoroutine(EsconderAvisoInventario());
    }

    private IEnumerator EsconderAvisoInventario()
    {
        yield return new WaitForSeconds(duracaoAvisoInventarioCheio);
        textoAvisoInventarioCheio.gameObject.SetActive(false);
        esconderAvisoInventarioCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Transform origem = (cameraTransform != null) ? cameraTransform : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origem.position, origem.forward * alcance);
    }
}