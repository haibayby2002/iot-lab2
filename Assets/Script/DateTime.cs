using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateTime : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Text txtDateTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        txtDateTime.text = System.DateTime.Now.ToString("F");
    }
}
