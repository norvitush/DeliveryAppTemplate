using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum MenuButtonType
{ 
    Home, Adresses, Catalog, Cart, Bonuses, Favorite, Profile, Info, Chat, Map
}

[Serializable]
public struct MenuLink
{
    public MenuButtonType Type;
    public Button Button;
    public Image Image;
    public TextMeshProUGUI Lable;
}


public class CommonLowerPanel : LowerPanel
{
    [Header("UI links")]
    [SerializeField] private CounterView counterView;
    [SerializeField] private Image cartNotEmptyMark;
    [SerializeField] private List<MenuLink> _allButtonsLinks = new List<MenuLink>();
    [SerializeField] private float _colorChangeTime = 0.2f;

    //setting cash
    public Color _selectedColor;
    public Color _selectedLableColor;
    public Color _unselectedColor;
    private bool _isCartWithCounter;

    private readonly Dictionary<Image, Color> _startValues = new Dictionary<Image, Color>();
    private readonly Dictionary<TextMeshProUGUI, Color> _lablesStartValues = new Dictionary<TextMeshProUGUI, Color>();
    private readonly Dictionary<MenuButtonType, MenuLink[]> _activeMenuButtons = new Dictionary<MenuButtonType, MenuLink[]>();
    private readonly IEnumerable enumNames = Enum.GetValues(typeof(MenuButtonType));
    
    private MenuButtonType _selected;

    private Coroutine _transitingColors;

    private void Awake()
    {
        //cash for setting values        
        if(cartNotEmptyMark != null) cartNotEmptyMark.color = _selectedColor;
     
        //grab to dictionary        
        foreach (MenuButtonType buttonType in enumNames)
        {
            var links = _allButtonsLinks.Where(val => val.Type == buttonType && val.Button != null && val.Button.gameObject.activeSelf);

            if (links.Count() > 0) 
            {
                _activeMenuButtons.Add(buttonType, links.ToArray());
            }
        }

        //SetCartEmptyState();
        RenderEndStateColors();
    }
    private void OnDisable()
    {
        RenderEndStateColors();
    }
    public override MenuButtonType Selected { get => _selected; }
    public override void Show()
    {
        if (gameObject.activeInHierarchy) return;
        gameObject.SetActive(true);

        InnerSelect(false);
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
    }
    public override void SetCounterLableValue(int addedElements = 0)
    {

        print($"SetCounterLableValue: {addedElements}");
        bool isEmpty = addedElements == 0;
        
        cartNotEmptyMark?.gameObject.SetActive(!isEmpty);
      
        if (counterView == null) return;

        counterView.SetCounter(addedElements, visible: _isCartWithCounter && !isEmpty);
    }
    public override void SetSelected(string menuButton)
    {
        if (Enum.TryParse(menuButton, out _selected))
        {
            InnerSelect();
            SetSelected(_selected);
        }
        else
            Debug.LogWarning($"MenuButtonType Parse on Click event WARNING! income({menuButton})");
      
    }
    public override void SetSelected(MenuButtonType button,
                            bool needCallAction = true)
    {
        _selected = button;
        InnerSelect(needCallAction); 
    }

    private void InnerSelect(bool needInvokeReaction = true)
    {
        if(needInvokeReaction) OnSelectMenu?.Invoke(_selected);

        if (_transitingColors != null)
        {
            StopCoroutine(_transitingColors);
        }

        _transitingColors = StartCoroutine(ColorTranzition(_colorChangeTime));
    }
    private void RenderEndStateColors()
    {
        foreach (var uiLinks in _activeMenuButtons)
        {
            Color imgColor = (uiLinks.Key == _selected) ? _selectedColor : _unselectedColor;
            Color lblColor = (uiLinks.Key == _selected) ? _selectedLableColor : _unselectedColor;

            for (int i = 0; i < uiLinks.Value.Length; i++)
            {
                if (uiLinks.Value[i].Image != null) uiLinks.Value[i].Image.color = imgColor;
                if (uiLinks.Value[i].Lable != null) uiLinks.Value[i].Lable.color = lblColor;
            }
        }
    }

    private IEnumerator ColorTranzition(float tranzitionTime = 0.2f)
    {
        float timer = 0f;
        float rate = 0;

        _startValues.Clear();
        _lablesStartValues.Clear();

        foreach (var uiLinks in _activeMenuButtons)
        {

            foreach (var link in uiLinks.Value)
            {
                if (link.Image != null) _startValues.Add(link.Image, link.Image.color);
                if (link.Lable != null) _lablesStartValues.Add(link.Lable, link.Lable.color);
            }
        }


        while (rate < 1)
        {
            rate = timer / tranzitionTime;            
            foreach (var uiLinks in _activeMenuButtons)
            {

                for (int i = 0; i < uiLinks.Value.Length; i++)
                {
                    if (uiLinks.Value[i].Image != null && _startValues.TryGetValue(uiLinks.Value[i].Image, out Color s))
                    { 
                        uiLinks.Value[i].Image.color = (uiLinks.Key == _selected)
                                                        ? Color.Lerp(_startValues[uiLinks.Value[i].Image], _selectedColor, rate)
                                                        : Color.Lerp(_startValues[uiLinks.Value[i].Image], _unselectedColor, rate); 
                    }
                    if (uiLinks.Value[i].Lable != null)
                    {
                        uiLinks.Value[i].Lable.color = (uiLinks.Key == _selected)
                                                       ? Color.Lerp(_lablesStartValues[uiLinks.Value[i].Lable], _selectedLableColor, rate)
                                                       : Color.Lerp(_lablesStartValues[uiLinks.Value[i].Lable], _unselectedColor, rate);
                    }
                }
            }

            yield return null;
            timer += Time.deltaTime;
        }

        RenderEndStateColors();
        _transitingColors = null;
    }

}
