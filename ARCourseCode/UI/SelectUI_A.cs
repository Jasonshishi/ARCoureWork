using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUI_A : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aPos = new Vector3(Mathf.Round(Module_Reflection.instant.selectionBoxA.transform.position.x), Mathf.Round(Module_Reflection.instant.selectionBoxA.transform.position.y), Mathf.Round(Module_Reflection.instant.selectionBoxA.transform.position.z));
        Vector3 bPos = new Vector3(Mathf.Round(Module_Reflection.instant.selectionBoxB.transform.position.x), Mathf.Round(Module_Reflection.instant.selectionBoxB.transform.position.y), Mathf.Round(Module_Reflection.instant.selectionBoxB.transform.position.z));

        if (aPos != this.transform.position && bPos != transform.position)
        {
            
            Destroy(this.gameObject);
            Module_Reflection.instant.selectANum = 0;
        }

    }
}
