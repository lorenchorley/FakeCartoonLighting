using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour {

    public float k;
    public Vector3 InitialOffset;

    Rigidbody2D rb;
    Vector3 InitialPosition;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        InitialPosition = transform.position;
        transform.position += InitialOffset;

    }

    void Update() {
        Vector3 HereToThere = InitialPosition - transform.position;
        float mag = k * HereToThere.magnitude;
        rb.AddForce(HereToThere.normalized * mag);
	}

}
