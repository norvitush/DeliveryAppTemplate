using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerPriceMarkUI : MonoBehaviour, ISelectedMark
{
    [SerializeField] private GameObject _selectedObject;
    [SerializeField] private GameObject _unSelectedObject;
    [SerializeField] private GameObject _newMark;

    [SerializeField] private TextMeshProUGUI _selectedPrice;
    [SerializeField] private TextMeshProUGUI _unselectedPrice;
    
    private IPressResolverByID _pressResolver;
    private int _id;

    public void SetData(IPressResolverByID pressResolver, int id, string text, string title = "", bool isSelected = false, bool isNew = false)
    {
        _pressResolver = pressResolver;
        _id = id;

        _selectedObject.SetActive(isSelected);
        _unSelectedObject.SetActive(isSelected == false);

        _newMark.SetActive(isNew);

        if (double.TryParse(text, out var priceVal))
        {
            _selectedPrice.text = _unselectedPrice.text = StringRenderHelper.GetCurrencyPostfixString(priceVal, 0);
        }
        else
        {
            _selectedPrice.text = _unselectedPrice.text = string.Empty;
        }
        
    }

    public void OnClick()
    {
        _pressResolver?.PressItem(_id);        
    }

    public void SetSelected(bool isSelected)
    {
        _selectedObject.SetActive(isSelected);
        _unSelectedObject.SetActive(isSelected == false);
    }
}

public interface ISelectedMark
{
    void SetData(IPressResolverByID pressResolver, int id, string text, string title = "", bool isSelected = false, bool isNew = false);
    void SetSelected(bool isSelected);
}