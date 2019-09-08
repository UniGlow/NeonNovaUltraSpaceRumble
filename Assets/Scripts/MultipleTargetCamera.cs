using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MultipleTargetCamera : MonoBehaviour
{

    #region Variable Declarations

    public List<Transform> targets;

    [SerializeField] float smoothTime = 0.5f;
    [SerializeField] float minZoom = 40f;
    [SerializeField] float maxZoom = 10f;
    [SerializeField] float maxDistance = 100f;

    private Vector3 velocity;
    private Camera[] cams;
    private Vector3 offset;
    #endregion



    #region Unity Event Functions
    private void Start()
    {
        cams = transform.GetComponentsInChildren<Camera>();
        offset = transform.position;
	}
	
	private void LateUpdate()
    {
        if (targets.Count == 0) return;

        Move();
        Zoom();
	}

    private void OnDrawGizmosSelected()
    {
        if (targets.Count == 0) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetCenterPoint(), 1f);
    }
    #endregion



    #region Public Functions
    public void SetCameraTargets(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig boss)
    {
        targets.Clear();
        targets.Add(hero1.playerTransform);
        targets.Add(hero2.playerTransform);
        targets.Add(hero3.playerTransform);
        targets.Add(boss.playerTransform);
    }
    #endregion



    #region Private Functions
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        transform.position = Vector3.SmoothDamp(transform.position, centerPoint + offset, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / maxDistance);
        foreach (Camera cam in cams)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        }
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        if (bounds.size.x > (1.778f * bounds.size.z))
        {
            return bounds.size.x;
        }
        else {
            return bounds.size.z * 1.778f;
        }
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 0)
        {
            Debug.LogWarning("No targets set for camera.", this);
            return Vector3.zero;
        }

        else if (targets.Count == 1)
        {
            return targets[0].position;
        }
        else
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            return bounds.center;
        }
    }
    #endregion
}
