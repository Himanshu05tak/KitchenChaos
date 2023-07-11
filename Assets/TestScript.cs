using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float time;
    public float maxTime;
    
    void Update()
    {
        time += Time.deltaTime;
        Debug.Log($"Antes: {time}");
        if (!(time > maxTime)) return;
        Debug.Log($"Time is less than: {time}");   
    }
}
