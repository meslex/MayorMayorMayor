using UnityEngine;
using UnityEngine.EventSystems;

public class PointEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public delegate void ButtonEnter();
    public static event ButtonEnter OnEnter;

    public delegate void ButtonExit();
    public static event ButtonExit OnExit;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name + " GameObject");
        OnEnter();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        OnExit();
        Debug.Log("Cursor Exiting " + name + " GameObject");
    }

}
