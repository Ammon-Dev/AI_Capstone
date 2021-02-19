using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    private RectTransform arrow;//the arrow tile holder

    [SerializeField]
    private TextMeshProUGUI f, g, h, p;//the costs and pos to be displayed

    public RectTransform MyArrow { get => arrow; set => arrow = value; }//the arrow tile holder
    public TextMeshProUGUI F { get => f; set => f = value; }//f mesh text
    public TextMeshProUGUI G { get => g; set => g = value; }//g mesh text
    public TextMeshProUGUI H { get => h; set => h = value; }//h mesh text
    public TextMeshProUGUI P { get => p; set => p = value; }//pos mesh text
}
