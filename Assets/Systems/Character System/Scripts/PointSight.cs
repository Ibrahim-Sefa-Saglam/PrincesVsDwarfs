using System.Collections.Generic;
using UnityEngine;

public class PointSight : MonoBehaviour
{
    [SerializeField] private Vector2 MainDirection = Vector2.right;
    private LayerMask groundLayerMask;
    
    [System.Serializable]
    public struct RayInfo
    {
        public float rayLength;
        public float rayAngle;
        public Vector2 hitPosition;
        public GameObject hitObject;

        public RayInfo(float length, float angle, Vector2 position, GameObject obj)
        {
            rayLength = length;
            rayAngle = angle;
            hitPosition = position;
            hitObject = obj;
        }
    }

    
    public GameObject sightPoint;
    [Min(0)] public float RayDensity = 1f;
    [Min(0)] public float RayLength = 5f;
    public string TargetTag = "Target";
    public float FOV = 90f;

    [Tooltip("If true, MainDirection will always point towards the cursor.")]
    public bool followCursor = false;
    public bool debugMode = false;

    // Store ray information
    public List<RayInfo> rayInformation = new List<RayInfo>();

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

        rayInformation.Clear(); // Clear previous ray information

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

        if (debugMode)
        {
            Color rayColor = Color.green;
        
            if (hit.collider)
            {
                rayColor = Color.yellow;
                if (hit.collider.gameObject.name == TargetTag) // Fixed comparison
                {
                    rayColor = Color.red;
                }
            }
            Debug.DrawRay(origin, direction * RayLength, rayColor);
        }

        // Store the ray information
        rayInformation.Add(new RayInfo(
            RayLength, 
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, // Convert direction to angle
            hit.point, 
            hit.collider ? hit.collider.gameObject : null)
        );
    }

    private Vector2 AngleToDirection(float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    // Public method to get the ray information
    public List<RayInfo> GetRayInformation()
    {
        return rayInformation;
    }
}

