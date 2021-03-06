using UnityEngine;

[RequireComponent(typeof(TEXDraw))]
[AddComponentMenu("TEXDraw/TEXLink UI", 4)]
public class TEXLink : TEXLinkBase
{
    protected override int SamplePointerStatus(int linkIdx)
    {
        Vector2 o;
        if(linkIdx >= m_DrawingContext.linkBoxRect.Count)
            return 0;
        for (int i = 0; i < input_PressPos.Count; i++) {
            var screenPos = input_PressPos[i];

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle
                    ((RectTransform)transform, screenPos, triggerCamera, out o)) {
                if (m_DrawingContext.linkBoxRect[linkIdx].Contains(o))
                    return 2;
            }
        }

        if (Input.mousePresent && RectTransformUtility.ScreenPointToLocalPointInRectangle
                    ((RectTransform)transform, input_HoverPos, triggerCamera, out o)) {
                if (m_DrawingContext.linkBoxRect[linkIdx].Contains(o))
                    return 1;
        }

        return 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        target = GetComponent<TEXDraw>();
        var tex = (TEXDraw)target;
        triggerCamera = tex.canvas.worldCamera;
    }
}

