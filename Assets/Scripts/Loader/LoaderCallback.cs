using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool _isFirstUpdate;
    void Update()
    {
        if (_isFirstUpdate) return;
        _isFirstUpdate = true;
        Loader.Loader.LoaderCallback();
    }
}
