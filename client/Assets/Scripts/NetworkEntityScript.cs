using UnityEngine;
using System.Collections;

public class NetworkEntityScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ping() {
        reset();
        transform.localScale = new Vector3(4,4,4);
        Invoke("reset", 1);
    }

    void reset() {
        transform.localScale = new Vector3(10,10,10);
    }
}
