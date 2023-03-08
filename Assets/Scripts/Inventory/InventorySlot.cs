using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem item;

    private RectTransform rectTransform;

    public void DoubleClick()
    {
        if (item != null)
        {
            if (item.Equiped == false)
            {
                item.UseOrEquip();
            }
            else if (item.Equiped == true)
            {
                item.OnUnequip();
            }

        }
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
            Debug.Log("Opening Tooltip");
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

    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }
}
