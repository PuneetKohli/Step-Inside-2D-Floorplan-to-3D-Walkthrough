using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.SetWidth(0.2f, 0.2f);
        _lineRenderer.enabled = true;
        addTapGesture();
    }

    private Vector3 _initialPosition;
    private Vector3 _currentPosition;

    public int _vertextCount = 1;
    private bool isTapped = false;

    void addTapGesture()
    {
        TKTapRecognizer tapRecognizer = new TKTapRecognizer();

        tapRecognizer.gestureRecognizedEvent += (r) =>
        {
            //Debug.Log("tap recognizer fired: " + r);
            //print("Location of tap" + r.startTouchLocation());
            _initialPosition = GetCurrentMousePosition(r.startTouchLocation()).GetValueOrDefault();
            //print("vertex count is " + _vertextCount);
            _lineRenderer.SetPosition(_vertextCount - 1, _initialPosition);  //1 -> initial pos
            _lineRenderer.SetVertexCount(++_vertextCount); //count = 2
            isTapped = !isTapped;
        };
        TouchKit.addGestureRecognizer(tapRecognizer);
    }

    public void Update()
    {
        if (isTapped)
        {
            _currentPosition = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
            _lineRenderer.SetVertexCount(_vertextCount + 1);
            _lineRenderer.SetPosition(_vertextCount, _currentPosition);

        }
        /*else if (Input.GetMouseButtonUp(0))
        {
            var releasePosition = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
            var direction = releasePosition - _initialPosition;
            Debug.Log("Process direction " + direction);
        }*/
    }

    private Vector3? GetCurrentMousePosition(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);

        }

        return null;
    }

}