using UnityEngine;

public class PointSight : MonoBehaviour
{
    [SerializeField] private Vector2 MainDirection = Vector2.right;
    private LayerMask groundLayerMask;

    public GameObject sightPoint;
    [Min(0)] public float RayDensity = 1f;
    [Min(0)] public float RayLength = 5f;
    public string TargetTag = "Target";
    public float FOV = 90f;

    [Tooltip("If true, MainDirection will always point towards the cursor.")]
    public bool followCursor = false;

    void Awake()
    {
        groundLayerMask = ~(1 << gameObject.layer);
    }

    private void Update()
    {
        if (!(sightPoint && RayDensity > 0f && RayLength > 0f)) return;

        UpdateMainDirection();
        CastAllRays();
    }

    private bool IsValid()
    {
        return sightPoint && RayDensity > 0f && RayLength > 0f;
    }

    private void UpdateMainDirection()
    {
        if (followCursor)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dirToMouse = (mouseWorldPos - transform.position);
            MainDirection = dirToMouse.normalized;
        }
        else
        {
            Vector2 dirToSight = (sightPoint.transform.position - transform.position).normalized;
            MainDirection = new Vector2(Mathf.Sign(dirToSight.x), 0); // x-direction only
        }
    }

    private void CastAllRays()
    {
        int rayCount = Mathf.CeilToInt(FOV * RayDensity);
        float angleStep = FOV / (rayCount - 1);
        float halfFOV = FOV / 2f;
        float baseAngle = Mathf.Atan2(MainDirection.y, MainDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = -halfFOV + angleStep * i;
            float currentAngle = baseAngle + angleOffset;
            Vector2 dir = AngleToDirection(currentAngle);
            CastRay(dir);
        }
    }

    private void CastRay(Vector2 direction)
    {
        Vector3 origin = sightPoint.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, RayLength, groundLayerMask);

        Color rayColor = Color.green;

        if (hit.collider)
        {
            rayColor = Color.yellow;
            Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.gameObject.name == TargetTag) // Fixed comparison
            {
                rayColor = Color.red;
            }
        }

        Debug.DrawRay(origin, direction * RayLength, rayColor);
    }

    private Vector2 AngleToDirection(float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
