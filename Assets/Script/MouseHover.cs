using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isHover;
    public static MouseHover mouseHover;
    void Start()
    {
        mouseHover = this;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }
}
