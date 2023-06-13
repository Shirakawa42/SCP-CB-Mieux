using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandScript : MonoBehaviour
{
    public RectTransform hand;
    private HashSet<GameObject> targets = new HashSet<GameObject>();
    private GameObject currentTarget;
    private bool isShowing = false;

    private void UpdatePosition()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(currentTarget.transform.position);
        if (screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height)
        {
            screenPoint.x = Screen.width / 2;
            screenPoint.y = Screen.height / 2;
        }
        hand.position = screenPoint;
    }

    private void GetTarget()
    {
        float minDistance = float.MaxValue;
        foreach (GameObject target in targets)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
            float distance = Vector3.Distance(screenPoint, new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (distance < minDistance)
            {
                minDistance = distance;
                currentTarget = target;
            }
        }
    }

    private void Show()
    {
        isShowing = true;
        hand.gameObject.SetActive(true);
    }

    private void Hide()
    {
        isShowing = false;
        hand.gameObject.SetActive(false);
    }

    public void AddTarget(GameObject target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(GameObject target)
    {
        targets.Remove(target);
        if (currentTarget == target)
            currentTarget = null;
    }

    void Update()
    {
        targets.RemoveWhere(item => item == null);
        if (targets.Count > 0)
        {
            GetTarget();
            UpdatePosition();
            if (!isShowing)
                Show();
        }
        else if (isShowing)
            Hide();
        if (Input.GetMouseButtonDown(0) && !Globals.isPaused && !Globals.isInventoryOpen)
            if (currentTarget != null)
                currentTarget.GetComponent<HandTarget>().Click(this);
    }
}
