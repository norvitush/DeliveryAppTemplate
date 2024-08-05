using System;

public interface IOverlayAnimator
{
    public void AnimateTranzition(Action callbackOnHide);
    public event Action OnCompleteTranzit;
}
