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


        Material material = bubble5.GetComponent<Renderer>().material;
        Vector3 position = bubble5.GetComponent<Transform>().position;
        Debug.Log("POS: " + position);
        material.SetVector("SphereCenter", new Vector4(position.x, position.y, position.z, 1));
        material.SetFloat("bonusSphereRadius", bubble5.GetComponent<Transform>().transform.lossyScale.x * 0.5f);
        // It...sort of discards it? When running, that is. 
    }

    // Update is called once per frame
    void Update()
    {
        bubble5 = GameObject.Find("Bubble5");
        Material material = bubble5.GetComponent<Renderer>().material;
        Vector3 position = bubble5.GetComponent<Transform>().position;
        Debug.Log("POS: " + position);
        Debug.Log(new Vector4(position.x, position.y, position.z, 1));
        material.SetVector("SphereCenter", new Vector4(position.x, position.y, position.z, 1));
        material.SetFloat("bonusSphereRadius", bubble5.GetComponent<Transform>().transform.lossyScale.x * 0.5f);
    }
}
