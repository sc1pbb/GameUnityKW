using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopUIElement : MonoBehaviour
{
    public Button shopButtonUI_; //if this item is bought than set it so the user can use it
    public RectTransform priceHolderUI_;
    public Text shopPriceTextUI_;
    public Image lockedImageUI_;    
    public Image usedElementHighlight_;
    

    /// <summary>
    /// Update based on bought/used
    /// </summary>
    /// <param name="bought"></param>
    /// <param name="used"></param>
    /// <param name="buttonCallback"></param>
    public void UpdateShopUIElement (bool bought, bool used, Sprite sprite, int price, int index, System.Action <int> buttonCallback = null)
    {
        shopPriceTextUI_.text = price.ToString ();
        shopButtonUI_.GetComponent<Image> ().sprite = sprite;
        lockedImageUI_.gameObject.SetActive (false);

        if (bought == false && used == false)
        {
            lockedImageUI_.gameObject.SetActive (true);
        }

        if (bought == true)
        {
            shopPriceTextUI_.gameObject.SetActive (false);
            priceHolderUI_.gameObject.SetActive (false);
        }

        if (used == true)
            usedElementHighlight_.gameObject.SetActive (true);
        else
            usedElementHighlight_.gameObject.SetActive (false);

        if (buttonCallback != null)
            AddListenerToButton (shopButtonUI_, buttonCallback, index);
        else
            shopButtonUI_.onClick.RemoveAllListeners ();
        
    }

    void AddListenerToButton (Button button, System.Action<int> onClickDeletage, int index)
    {
        button.onClick.RemoveAllListeners ();
        button.onClick.AddListener (() =>
        { onClickDeletage (index); });
    }
}
