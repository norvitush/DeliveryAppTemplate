using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComonTopPanel : TopPanelView
{
    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void SetButtons(TopPanelIco[] icons)
    {
        throw new System.NotImplementedException();
    }

    public override void SetLableValue(string title, bool hasBackArrow)
    {
        throw new System.NotImplementedException();
    }

}
