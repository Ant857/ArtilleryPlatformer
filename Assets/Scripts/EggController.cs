using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggController : MonoBehaviour
{

    public float power;
    public Rigidbody2D rb;

    public Vector2 minPower;
    public Vector2 maxPower;

    Camera cam;
    Vector2 finalForce;
    Vector2 currentForce;
    Vector3 startPoint;
    Vector3 endPoint;

    private bool isMouseDown = false;
    private float collisionCheckRadius = 0.01f;
    public float simulateForDuration;//simulate for 5 secs in the future
    public float simulationStep;//Will add a point every 0.1 secs.
    public LineRenderer lineRenderer;
    private List<Vector2> lineRendererPoints = new List<Vector2>();
    private int layerMask = 1 << 0; //Layer 0


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            startPoint.z = 15;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            endPoint.z = 15;

            finalForce = new Vector2(Mathf.Clamp(startPoint.x - endPoint.x, minPower.x, maxPower.x), Mathf.Clamp(startPoint.y - endPoint.y, minPower.y, maxPower.y));
            rb.velocity = finalForce * power;
        }

        currentForce = new Vector2(Mathf.Clamp(startPoint.x - cam.ScreenToWorldPoint(Input.mousePosition).x, minPower.x, maxPower.x), Mathf.Clamp(startPoint.y - cam.ScreenToWorldPoint(Input.mousePosition).y, minPower.y, maxPower.y));
    }

    private void LateUpdate()
    {
        if (isMouseDown == true)
        {
            GetComponent<LineRenderer>().enabled = true; 
            SimulateArc();
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }
    }
    private void SimulateArc()
    {
        lineRendererPoints.Clear();
        int steps = (int)(simulateForDuration / simulationStep);//number of points on line -- currently 50
        Vector2 calculatedPosition;
        Vector2 directionVector = currentForce;//Vector of force being applied to egg
        Vector2 launchPosition = rb.position;//Position where egg is launched from
        float launchSpeed = (power * 1.41f);//Example speed per secs.

        for (int i = 0; i < steps; i++)
        {
            calculatedPosition = launchPosition + (directionVector * (launchSpeed * i * simulationStep));
            //Calculate gravity
            calculatedPosition.y += Physics2D.gravity.y * (i * simulationStep) * (i * simulationStep);
            lineRendererPoints.Add(calculatedPosition);
            if (CheckForCollision(calculatedPosition))//if you hit something
            {
                break;//stop adding positions
            }
        }
        lineRenderer.positionCount = lineRendererPoints.Count;
        for (int i = 0; i < lineRendererPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, lineRendererPoints[i]); //Assign all the positions to the line renderer.
        }
    }
    private bool CheckForCollision(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, collisionCheckRadius, layerMask);
        if (hits.Length > 0)
        {
            //We hit something 
            //check if its a wall or seomthing
            //if its a valid hit then return true
            return true;
        }
        return false;
    }
}
