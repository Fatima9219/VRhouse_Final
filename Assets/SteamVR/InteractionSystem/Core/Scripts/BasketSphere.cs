using UnityEngine;

public class BasketSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Substring(0, 2).Equals("ZP") || other.name.Substring(0, 2).Equals("Pr")) {   // paranting only ZP-Items with ProduktBarcode
            Vector3 scale = other.transform.localScale;
            Vector3 scaleOfChild = other.transform.GetChild(0).localScale;

            other.transform.parent = transform;

            other.transform.localScale = scale;
            other.transform.GetChild(0).transform.localScale = scaleOfChild;
        }
    }
}