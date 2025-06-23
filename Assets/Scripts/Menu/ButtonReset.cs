using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonReset : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Color normalColor;
    public bool debug;
    public Animator anim;
    public Color highlightedColor;

    private void ResetColor()
    {
        text.color = normalColor;
    }
    private void Update()
    {
        if (debug) Debug.Log("HighlightedColor: " + ColorUtility.ToHtmlStringRGB(text.color));
    }

    public void Highlight()
    {
        text.color = highlightedColor;
    }
}
