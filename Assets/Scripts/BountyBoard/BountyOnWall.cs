using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BountyOnWall : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 scale;
   [SerializeField] Vector3 largerScale;
    [SerializeField] float expandSpeed = 1f;
    [SerializeField] float shrinkSpeed = 1f;
   public bool highlighted = false;
    private void Start()
    {
        scale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

        highlighted = true;
    }


    private void Update()
    {
        if (highlighted)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, largerScale, expandSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, expandSpeed * Time.deltaTime);
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
    }
}
