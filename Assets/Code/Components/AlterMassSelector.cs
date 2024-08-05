using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlterMassSelector : MonoBehaviour
{    
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;

    public RectTransform contentRect;
    public RectTransform selectorRect;
    public GameObject massPrefab;

    int currentSelected;

    public float speed = 10;

    private readonly Dictionary<int,(RectTransform rect, TextMeshProUGUI text)> _elements = new Dictionary<int, (RectTransform, TextMeshProUGUI)>();

    private bool inited = false;

    private void OnEnable()
    {
        if(inited)
        {
            StartCoroutine(SlideToCurrent());
        }
    }
    private void OnDisable()
    {
        inited = false;
    }
    public void Init(List<string> massList, Action<int> callback)
    {

        currentSelected = 0;

        foreach (var element in _elements)
        {
            Destroy(element.Value.rect.gameObject);
        }

        Button massButton;
        _elements.Clear();

        for (int i = 0; i < massList.Count; i++)
        {
            massButton = Instantiate(massPrefab, contentRect).GetComponent<Button>();
            var lable = massButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _elements.Add(i, ((RectTransform)massButton.transform, lable));

            lable.text = string.IsNullOrEmpty(massList[i]) ? "empty" : massList[i];
            int index = i;
            massButton.onClick.AddListener(() => {
                currentSelected = index;
                callback.Invoke(index);
                print($"index: {index}");
                StopAllCoroutines();
                StartCoroutine(SlideToCurrent());
            });

        }

        inited = true;
        if(gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(SlideToCurrent());
        }
    }

    private IEnumerator SlideToCurrent()
    {
        
        if(_elements.TryGetValue(currentSelected, out (RectTransform rect, TextMeshProUGUI text) target))
        {

            yield return null;
            yield return null;

            var targetPos = target.rect.anchoredPosition;

            Vector2 currentSize =  selectorRect.sizeDelta;
            float targetSizeX = target.rect.sizeDelta.x;
            if (currentSelected == 0 || currentSelected == _elements.Count - 1) { targetSizeX -= 5; }

            while (Vector2.Distance(selectorRect.anchoredPosition, targetPos) > 0.02f || currentSize.x < (targetSizeX - 0.01f))
            {
                selectorRect.anchoredPosition = Vector2.Lerp(selectorRect.anchoredPosition, targetPos, Time.deltaTime * speed);
                currentSize.x = Mathf.Lerp(currentSize.x, targetSizeX, Time.deltaTime * speed);
                selectorRect.sizeDelta = currentSize;

                foreach (var pair in _elements)
                {
                    if (pair.Key == currentSelected)
                        pair.Value.text.color = Color.Lerp(pair.Value.text.color, _selectedColor, speed * Time.deltaTime);
                    else
                        pair.Value.text.color = Color.Lerp(pair.Value.text.color, _unselectedColor, speed * Time.deltaTime);
                }
                
                yield return null;
            }

            selectorRect.anchoredPosition = targetPos;
            foreach (var pair in _elements)
            {
                if (pair.Key == currentSelected)
                    pair.Value.text.color = _selectedColor;
                else
                    pair.Value.text.color = _unselectedColor;
            }
        }

        yield break;
    }

}
