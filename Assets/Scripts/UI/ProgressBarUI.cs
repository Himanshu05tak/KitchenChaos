using Interface;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress _hasProgress;
    private void Start()
    {
        _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if(_hasProgress==null)
            Debug.LogError($"GameObject{hasProgressGameObject }doesn't have a component that implements IHasProgress!");
        
        _hasProgress.OnProgressChanged += HasProgressGameObjectOnProgressGameObjectChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void HasProgressGameObjectOnProgressGameObjectChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.ProgressNormalized;

        if (e.ProgressNormalized is 0f or 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
