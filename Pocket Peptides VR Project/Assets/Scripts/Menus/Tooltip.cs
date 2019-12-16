using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance; // static methods can be called anywhere with Tooltip.methodName

    // [SerializeField]
    private Camera uiCamera = null; // we are using screen space overlay, leave this as null

    private Text tooltipText;
    private RectTransform backgroundRectTransform;

    private Vector3 offset;
    private float onButtonTime;  // how long does the mouse hover on  button
    private float offButtonTime; // how long does the mouse hover off button
    private bool  isShowOn;      // trigger flags
    private bool  isHideOn = true;
    private string currentButtonName; // for comparing button names

    private void Awake()
    {
        instance = this;
        backgroundRectTransform = transform.Find("Background Image").GetComponent<RectTransform>();
        tooltipText = transform.Find("Text").GetComponent<Text>();
        //offset = new Vector3(-25, 15, 0);
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition + offset, uiCamera, out localPoint);
        transform.localPosition = localPoint;
        showButtonTooltip(); // only shows ui buttons' tooltips once mouse over them
    }

    private void ShowTooltip(string tooltipString)
    {
        // gameObject.transform.Find("Background Image").gameObject.SetActive(true);
        // gameObject.transform.Find("Text").gameObject.SetActive(true);
        transform.parent.GetComponent<Animator>().SetTrigger("Show Tooltip");
            
        transform.SetAsLastSibling();

        tooltipText.text = tooltipString;
        float textPaddingSize = 4f; 
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        backgroundRectTransform.sizeDelta = backgroundSize;
        offset = new Vector3(-backgroundSize.x / 2 - 5, backgroundSize.y / 2 + 4, 0);
    }

    private void HideTooltip()
    {
        // gameObject.transform.Find("Background Image").gameObject.SetActive(false);
        // gameObject.transform.Find("Text").gameObject.SetActive(false);
        transform.parent.GetComponent<Animator>().SetTrigger("Hide Tooltip");   
    }

    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }

    private void showButtonTooltip()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> resultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, resultList);
        if (resultList.Count > 0)
        {
            foreach (var UI in resultList)
            {
                if (UI.gameObject.transform.parent.name == "Button Panel" && UI.gameObject.name.Contains("Button"))
                {
                    if (UI.gameObject.name == currentButtonName)
                    {
                        onButtonTime += Time.deltaTime;
                        offButtonTime = 0;
                        if (onButtonTime > 1)
                        {
                            isHideOn = false;
                            if (!isShowOn)
                            {
                                ShowTooltip(UI.gameObject.name.Replace("Button", ""));
                                currentButtonName = UI.gameObject.name;
                                isShowOn = true;
                            }
                        }
                    }
                    else
                    {
                        onButtonTime = 0;
                        isShowOn = false;
                        if (!isHideOn)
                        {
                            HideTooltip();
                            isHideOn = true;
                        }

                    }
                    currentButtonName = UI.gameObject.name;
                }
                else
                {
                    offButtonTime += Time.deltaTime;
                    if (offButtonTime > 0.1)
                    {
                        onButtonTime = 0;
                        isShowOn = false;
                        if (!isHideOn)
                        {
                            HideTooltip();
                            isHideOn = true;
                        }
                    }
                    currentButtonName = null;
                }
            }
        }
    }
}
