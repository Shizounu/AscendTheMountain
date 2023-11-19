using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileVisualsManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visualization")]
    [SerializeField] private float timeTillFullGlow = 1f;
    [SerializeField] private float hoverTime = 0;
    [SerializeField] private HoverState hoverState;

    [Header("References")]
    [SerializeField] private SpriteRenderer baseRenderer; 
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private SpriteRenderer selectedGlowRenderer;

    [Header("Debugs")]
    public bool selectLockout;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selectLockout)
        {
            selectLockout = true;
            hoverState = HoverState.selected;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverState != HoverState.selected)
            hoverState = HoverState.hovered;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverState != HoverState.selected)
            hoverState = HoverState.unselected;
    }

    private void Update()
    {
        switch (hoverState)
        {
            case HoverState.unselected:
                hoverTime = 0;
                glowRenderer.color = new Color(
                    glowRenderer.color.r,
                    glowRenderer.color.g,
                    glowRenderer.color.b,
                    0
                );
                baseRenderer.color = new Color(
                    baseRenderer.color.r,
                    baseRenderer.color.g,
                    baseRenderer.color.b,
                    1
                );
                break;

            case HoverState.hovered:
                hoverTime += Time.deltaTime;
                baseRenderer.color = new Color(
                    baseRenderer.color.r,
                    baseRenderer.color.g,
                    baseRenderer.color.b,
                    Mathf.Lerp(0, 1, 1 - hoverTime / timeTillFullGlow)
                );

                glowRenderer.color = new Color(
                    glowRenderer.color.r,
                    glowRenderer.color.g,
                    glowRenderer.color.b,
                    Mathf.Lerp(0, 1, hoverTime / timeTillFullGlow)
                );
                break;


            case HoverState.selected:
                StartCoroutine(DelayedBlendOut(2.5f));
                hoverState = HoverState.hovered;
                break;
        }
    }


    private IEnumerator DelayedBlendOut(float delay, float blendOutTime = 1f){
        selectedGlowRenderer.color = new Color(
            selectedGlowRenderer.color.r,
            selectedGlowRenderer.color.g,
            selectedGlowRenderer.color.b,
            1
        );

        yield return new WaitForSeconds(delay);

        int resolution = 120;
        for (int i = 0; i < resolution; i++){
            selectedGlowRenderer.color = new Color(
                    selectedGlowRenderer.color.r,
                    selectedGlowRenderer.color.g,
                    selectedGlowRenderer.color.b,
                    Mathf.Lerp(1, 0, (float)i/(float)resolution)
                    );
            yield return new WaitForSeconds(blendOutTime / resolution);
        }
        selectLockout = false;
    }
}
