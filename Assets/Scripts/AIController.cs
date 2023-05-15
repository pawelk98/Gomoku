using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; private set; }
    [SerializeField]
    int easyDepth;
    [SerializeField]
    int mediumDepth;
    [SerializeField]
    int hardDepth;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void MakeMove()
    {
    }
}
