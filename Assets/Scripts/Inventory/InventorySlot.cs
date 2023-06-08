using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem item;
    public int Amount = 0;
    public int index;
    public Image dummyImage;
    public GameObject equippedIndicator;

    private RectTransform rectTransform;
    private float doubleClickInterval = 0.5f;
    private float lastClickTime = -1;
    private Image image;


    public void DoubleClick()
    {
        if (Time.time - lastClickTime < doubleClickInterval && item != null)
        {
            InventoryScript inventoryScript = transform.parent.parent.parent.GetComponent<InventoryScript>();
            inventoryScript.EquipUnequipSlot(item, index);
        }
        lastClickTime = Time.time;
    }

    private IEnumerator SmoothSize(float targetWidth, float targetHeight, float duration)
    {
        float time = 0;
        float startWidth = rectTransform.rect.width;
        float startHeight = rectTransform.rect.height;

        while (time < duration)
        {
            time += Time.deltaTime;
            float width = Mathf.Lerp(startWidth, targetWidth, time / duration);
            float height = Mathf.Lerp(startHeight, targetHeight, time / duration);
            rectTransform.sizeDelta = new Vector2(width, height);
            yield return null;
        }
    }

    public void OnPointerEnter()
    {
        if (item != null)
        {
            Debug.Log(item.Description);
        }
        StartCoroutine(SmoothSize(100, 100, 0.1f));
    }

    public void OnPointerExit()
    {
        if (item != null)
        {
            Debug.Log("Closing Tooltip");
        }
        StartCoroutine(SmoothSize(75, 75, 0.1f));
    }

    public void OnDragStart()
    {
        if (item != null)
        {
            Globals.isDragging = true;
            Globals.itemBeingDragged = item;
            Globals.amountBeingDragged = Amount;
            Globals.slotIndexBeingDragged = index;
            Globals.isEquipped = equippedIndicator.activeSelf;
            dummyImage.sprite = item.Icon;
            dummyImage.transform.GetChild(0).gameObject.SetActive(equippedIndicator.activeSelf);
            dummyImage.gameObject.SetActive(true);
            equippedIndicator.SetActive(false);
            item = null;
            Amount = 0;
            //image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        }
    }

    public void OnDrop()
    {
        if (Globals.isDragging)
        {
            if (item == null)
            {
                item = Globals.itemBeingDragged;
                item.slot = this;
                Amount = Globals.amountBeingDragged;
                image.sprite = item.Icon;
            }
            else if (item.name == Globals.itemBeingDragged.name)
            {
                Amount += Globals.amountBeingDragged;
            }
            else
            {
                InventoryItem tempItem = item;
                int tempAmount = Amount;
                item = Globals.itemBeingDragged;
                item.slot = this;
                Amount = Globals.amountBeingDragged;
                image.sprite = item.Icon;
                InventoryScript inventoryScript = transform.parent.parent.parent.GetComponent<InventoryScript>();
                inventoryScript.slots[Globals.slotIndexBeingDragged].item = tempItem;
                inventoryScript.slots[Globals.slotIndexBeingDragged].Amount = tempAmount;
                inventoryScript.slots[Globals.slotIndexBeingDragged].image.sprite = tempItem.Icon;
            }
            equippedIndicator.SetActive(Globals.isEquipped);
            Globals.isDragging = false;
            Globals.itemBeingDragged = null;
            Globals.amountBeingDragged = 0;
            Globals.slotIndexBeingDragged = 0;
            Globals.isEquipped = false;
            dummyImage.gameObject.SetActive(false);
        }
    }

    public void OnDrag()
    {
        dummyImage.transform.position = Input.mousePosition;
    }

    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        image = transform.GetComponent<Image>();
    }
}
