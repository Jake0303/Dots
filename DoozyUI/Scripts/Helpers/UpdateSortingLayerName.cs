using UnityEngine;
using System.Collections;
using DoozyUI;

public class UpdateSortingLayerName : MonoBehaviour
{
    public string newLayerName = "UI";

    public void UpdateCanvases()
    {
          DoozyUI.UIManager.UpdateCanvases(gameObject, newLayerName);
    }

    public void UpdateRenderers()
    {
        DoozyUI.UIManager.UpdateRenderers(gameObject, newLayerName);
    }

}
