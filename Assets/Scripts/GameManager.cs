using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cam;

    public static GameManager inst;

    private void Awake()
    {
        if(inst != null)
        {
            Destroy(this);
        }
        else
        {
            inst = this;
        }
    }

    public void SetCameraActive(bool isActive)
    {
        if(cam != null)
        {
            cam.SetActive(isActive);
        }
    }
}