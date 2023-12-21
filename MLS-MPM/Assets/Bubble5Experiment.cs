using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble5Experiment : MonoBehaviour
{
    GameObject bubble5;

    private void MoveObject()
    {
        bubble5.GetComponent<Transform>().Translate(0, -0.1f, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        bubble5 = GameObject.Find("Bubble5");
        Invoke("MoveObject", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        bubble5 = GameObject.Find("Bubble5");
        Material material = bubble5.GetComponent<Material>();
        Vector3 position = bubble5.GetComponent<Transform>().position;
        Vector4 arr = new(position.x, position.y, position.z, 1.0f);
        List<Vector4> list = new List<Vector4>
        {
            arr
        };
        material.SetVectorArray("bonusSphereCenter", list);
        material.SetFloat("bonusSphereRadius", bubble5.GetComponent<Transform>().transform.lossyScale.x * 0.5f);
    }
}
