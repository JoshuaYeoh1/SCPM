using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Current;

    void Awake()
    {
        if(!Current) Current=this;        
    }
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void LockMouse(bool toggle)
    {
        if(toggle)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        Cursor.visible = !toggle;
    }
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("Raycast")]
    public bool canClick=true;
    public LayerMask layers;
    public float clickRange=5;
    public bool ignoreTimescale;

    [Header("Leniency")]
    public float clickRadius=.01f;
    Vector2 startClickPos, endClickPos;
    float lastClickedTime;
    public float minSwipeDistance = 100; // distance for a tap to be considered a swipe
    public float minSwipeTime = 0.25f; // time for a tap to be considered a swipe

    void Update()
    {
        CheckClick();
    }

    void CheckClick()
    {
        if(!canClick) return;

        if(!ignoreTimescale && Time.timeScale==0) return;

        // Check if the current pointer event is over a UI element
        if(IsPointerOverUI(Input.mousePosition)) return;

        if(Input.GetMouseButtonDown(0))
        {
            startClickPos = Input.mousePosition; // Record the start position and time of the tap
            lastClickedTime = Time.time;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            endClickPos = Input.mousePosition;

            float swipeDistance = Vector2.Distance(startClickPos, endClickPos); // Calculate the distance moved and time taken

            if(swipeDistance<minSwipeDistance && Time.time-lastClickedTime < minSwipeTime) // Check if tapped
            {
                DoSphereCast();
            }
            else // Check if swiped
            {
                Vector2 swipeVector = endClickPos-startClickPos;
                Vector2 swipeDirection = swipeVector.normalized; //Debug.Log("Swiped in direction: " + swipeDirection);
            }
        }
    }

    List<RaycastResult> raycastResults = new List<RaycastResult>();

    bool IsPointerOverUI(Vector2 touchPos)
    {
        PointerEventData eventDataPos = new PointerEventData(EventSystem.current);

        eventDataPos.position = touchPos;

        EventSystem.current.RaycastAll(eventDataPos, raycastResults);

        if(raycastResults.Count>0) // if more than 0, then UI is touched
        {
            foreach(RaycastResult result in raycastResults)
            {
                if(result.gameObject.tag!="TouchField") return true; // ignore UI elements with this tag
            }
        }

        return false;
    }

    void DoSphereCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.SphereCast(ray, clickRadius, out RaycastHit hit, clickRange, layers, QueryTriggerInteraction.Collide))
        {
            Collider other = hit.collider;
            
            Rigidbody otherRb = other.attachedRigidbody;

            if(otherRb)
            {
                EventManager.Current.OnClick(otherRb.gameObject);
            }
        }
    }

}
