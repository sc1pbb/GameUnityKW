using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] private Text popUpText_;
    [SerializeField] private Button button1_;
    [SerializeField] private Button button2_;
    [SerializeField] private Text button1_Text_;
    [SerializeField] private Text button2_Text_;

    public void ActivatePopUp (string popupText, PopUpType popUpType, string button1Text, string button2Text, System.Action button1Delegate, System.Action button2Delegate)
    {
        popUpText_.text = popupText;

        if (string.IsNullOrEmpty (button1Text) == true)
        {
            button1_.gameObject.SetActive (false);
        } else
        {
            button1_.gameObject.SetActive (true);
            button1_Text_.text = button1Text;

            if (button1Delegate != null)
                AddListenerToPopUpButton (button1_, button1Delegate);
        }

        if (string.IsNullOrEmpty (button2Text) == true)
        {
            button2_.gameObject.SetActive (false);
        }
        else
        {
            button2_.gameObject.SetActive (true);
            button2_Text_.text = button2Text;

            if (button2Delegate != null)
                AddListenerToPopUpButton (button2_, button2Delegate);
        }

        //TODO: ADJUST BUTTONS POSITIONS BASED ON TYPE

        //Play animation
        this.gameObject.SetActive (true);
        this.transform.DOKill ();
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale (Vector3.one, 1f).SetEase (Ease.OutBounce);
    }

    void AddListenerToPopUpButton (Button button, System.Action onClickDeletage)
    {
        button.onClick.RemoveAllListeners ();
       
        button.onClick.AddListener (() =>
        { onClickDeletage (); });
    }

    public void UIButton_ClosePopUp ()
    {
        this.transform.DOScale (Vector3.zero, 1f).SetEase (Ease.OutBounce).OnComplete ( () => { gameObject.SetActive (false); });
    }
}

public enum PopUpType
{
    ONE_BUTTON,
    TWO_BUTTONS
}
