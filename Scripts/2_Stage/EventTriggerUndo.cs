using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BtnInfoAds))]
public class EventTriggerUndo : EventTrigger
{
    //============================================
    private BtnInfoAds btn;
    private UIObjects uiObj;
    private MainLogic ml;
    private Vector2 resetSize;
    //============================================
    private void Start()
    {
        btn = this.GetComponent<BtnInfoAds>();
        uiObj = UIObjects.Instance;
        ml = MainLogic.Instance;
        resetSize = this.GetComponent<RectTransform>().sizeDelta;
    }
    //============================================
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (MainLogic.Instance.state == State.Moving)
            return;

        StartCoroutine(ScaleUp());
    }

    IEnumerator ScaleUp()
    {
        btn.PressColor();
        RectTransform rt = btn.image.GetComponent<RectTransform>();
        Vector2 needSize = rt.sizeDelta * 1.2f;

        while (rt.sizeDelta.x < needSize.x)
        {
            rt.sizeDelta *= 1.07f;
            yield return new WaitForEndOfFrame();
        }

        rt.sizeDelta = needSize;
    }
    //============================================
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (MainLogic.Instance.state == State.Moving)
            return;

        StartCoroutine(ScaleDown());

        //> 광고 팝업
        uiObj.adsUndoPanel.gameObject.SetActive(true);
    }

    IEnumerator ScaleDown()
    {
        btn.ResetColor();
        RectTransform rt = btn.image.GetComponent<RectTransform>();

        while (rt.sizeDelta.x > resetSize.x)
        {
            rt.sizeDelta *= 0.95f;
            yield return new WaitForEndOfFrame();
        }

        rt.sizeDelta = resetSize;
    }
    //============================================
}
