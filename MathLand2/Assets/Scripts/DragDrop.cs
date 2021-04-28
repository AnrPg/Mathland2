using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private float mass;
    [SerializeField] private float gravityScale;
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int[] expressionAnimations;
    //private Vector3 canvasScaleFactor;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Animator animator;
    private Float floatComponent;
    private Rigidbody2D rigidbody2DComponent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        animator = transform.GetComponentInChildren<Animator>();
        floatComponent = GetComponent<Float>();
        rigidbody2DComponent = GetComponent<Rigidbody2D>();
        //canvasScaleFactor = GetComponent<RectTransform>().localScale;
        floatComponent.FloatAround = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        int randExpression = UnityEngine.Random.Range(1, expressionAnimations.Length);
        animator?.SetInteger("Expression", randExpression);
        floatComponent.FloatAround = false;
        if (GetComponent<Rigidbody2D>() != null)
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        float canvasScalerFactor = canvas.GetComponent<RectTransform>().localScale.x;//canvas.GetComponent<CanvasScaler>().scaleFactor;
        transform.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        //animator?.SetInteger("Expression", 0);
        
        if (GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
            rigidbody2DComponent = GetComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().mass = this.mass;
            GetComponent<Rigidbody2D>().gravityScale = this.gravityScale;
            GetComponent<Rigidbody2D>().sharedMaterial = this.physicsMaterial;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        floatComponent.FloatAround = true;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        int randExpression = UnityEngine.Random.Range(1, expressionAnimations.Length);
        animator?.SetInteger("Expression", randExpression);
    }
}
