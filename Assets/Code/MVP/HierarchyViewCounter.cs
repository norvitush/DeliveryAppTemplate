
using System;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyViewCounter
{
    private struct HierarchyObject
    {
        public IView view;
        public WindowPresenter presenter;
        public Action closeCallback;
    }
    
    private GameObject _fastMessage;
    private readonly LinkedList<HierarchyObject> _hierarchy= new();

    public HierarchyViewCounter(GameObject fastMessage)
    {
        _fastMessage = fastMessage;
        _fastMessage.SetActive(false);
    }
    
    public void AddOpened(IView view, WindowPresenter presenter, Action onClose = null)
    {

        HierarchyObject @object = new() { view = view, presenter = presenter, closeCallback = () => { SafeClose(presenter, view); onClose?.Invoke(); } };

        _hierarchy.AddLast(@object);
    }
    
    public void CloseAny(IView view)
    {        
        if(_hierarchy.Last.Value.view == view)
        {
            GoBack();
            return;
        }

        var currentNode = _hierarchy.First;

        while ((currentNode != null) && (currentNode.Value.view == null || currentNode.Value.view != view))
            currentNode = currentNode.Next;
        

        if (currentNode != null)
        {
            //finded

            var node = currentNode;
            LinkedListNode<HierarchyObject> next = null;

            while (node != null)
            {
                next = node.Next;
                SafeClose(node.Value.presenter, node.Value.view);
                _hierarchy.Remove(node); 
                node = next;
            }
        }
    }
    
    public void ForceDrop()
    {
        _hierarchy.Clear();
    }
    
    public void OnPressBack()
    {

        if (_hierarchy.Count == 0)
        {
            if (!_fastMessage.activeSelf)
            {
                _fastMessage.SetActive(true);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"Aplication QUIT!");
#else
                Application.Quit();
#endif
            }
        }
        else
        {
            GoBack();
        }
    }
    
    private void SafeClose(WindowPresenter presenter, IView view)
    {
        if(presenter != null)
        {
            presenter.SetActive(false);
            return;
        }
        if (view.IsActive) view.SetActive(false);
    }
    
    private void GoBack()
    {
        Debug.Log($"GO BACK { _hierarchy.Last.Value.closeCallback.Method.Name}");
        _hierarchy.Last.Value.closeCallback();
        _hierarchy.RemoveLast();
    }
}