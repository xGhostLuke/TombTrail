using UnityEngine;

public class WindowController : MonoBehaviour {
    [SerializeField] protected CanvasGroup cg;

    public void ShowWindow() {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void HideWindow() {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
