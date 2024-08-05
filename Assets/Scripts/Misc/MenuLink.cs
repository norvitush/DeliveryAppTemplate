using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public struct MenuLink
{
    public MenuButtonType Type;
    public Button Button;
    public Image Image;
    public TextMeshProUGUI Lable;
}
