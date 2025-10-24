using UnityEngine;

public class WindowController : MonoBehaviour {
    [SerializeField] protected CanvasGroup cg;

    private bool active;

    public void ShowWindow() {
        SetActive(false);
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

    public bool GetActive() {
        return active;
    }

    public void SetActive(bool b) {
        active = b;
    }

}
