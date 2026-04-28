
using UnityEngine;

public class Reticle : MonoBehaviour
{
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        Cursor.visible = false;
    }

    void Update()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float dist))
        {
            Vector3 worldPos = ray.GetPoint(dist);
            transform.position = new Vector3(worldPos.x, 0.05f, worldPos.z);
        }
    }
}