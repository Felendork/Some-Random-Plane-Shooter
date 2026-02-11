using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField]
    private float _speed = 0.5f;

    // This grabs the material, and the Y offset so it can be moved vertically.
    private Material _mat;
    private float _offsetY;

    private void Awake()
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null)
        {
            Debug.LogError("BackgroundScroll needs a Renderer (MeshRenderer) on the same GameObject.");
            enabled = false;
            return;
        }

        // creates an instance once (ok), then we reuse it
        _mat = r.material; 
    }

    private void Update()
    {
        // This has the Y offset moving up at the speed of 0.5 at real time. 
        _offsetY += _speed * Time.deltaTime;
        // And this ensures the material is moving along the axis otherwise you won't see the change. 
        _mat.mainTextureOffset = new Vector2(0f, _offsetY);
    }
}
