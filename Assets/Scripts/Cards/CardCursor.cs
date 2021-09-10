using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Cards
{
    public class CardCursor : MonoBehaviour
    {
        #region LineRendererFileds
        private Vector3 pointStart;
        private Vector3 pointEnd;
        private Vector3 pointMiddle;
        private GameObject lineRendererObj;
        private LineRenderer lineRenderer;
        public float vertexCount = 12;
        public float point2YPosition = 2;
        private Vector3 pointStartPosition;
        private Vector3 pointEndPosition;
        #endregion

        #region CursorSelectionFileds
        private GameObject cursorObj;
        private Image cursorImage;
        #endregion

        [SerializeField] private Color acceptPositiveColorCursor;
        [SerializeField] private Color acceptNegativeColorCursor;
        private Camera mainCamera;

        public void DisableLine()
        {
            lineRenderer.enabled = false;
            cursorImage.enabled = false;
        }
        
        private void Start()
        {
            mainCamera = Camera.main;
            
            lineRendererObj = GameObject.Find("CursorLineRenderer");
            lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            pointStart = transform.position;
            lineRenderer.transform.position = pointStart;
            
            cursorObj = GameObject.Find("CursorCardSelection");
            cursorImage = cursorObj.GetComponent<Image>();
            cursorImage.enabled = true;
            SetCursorAsInteractable(false); // in the beginning, mark as false
        }

        // Update is called once per frame
        private void Update()
        {
            var curMousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(curMousePos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                pointEnd = raycastHit.point;
            }
            else
            {
                // Return because if there is no raycast we can suck
                return;
            }
            
            DrawCursorLine();
            DrawCursorPoint(curMousePos);

        }

        public void SetCursorAsInteractable(bool status)
        {
            Color color;
            if (status)
            {
                color = acceptPositiveColorCursor;
            }
            else
            {
                color = acceptNegativeColorCursor;
            }
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(255, 0.0f), new GradientAlphaKey(255, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
            cursorImage.color = color;
        }
        
        private void DrawCursorPoint(Vector3 curMousePos)
        {
            cursorObj.transform.position = curMousePos;
        }
        
        private void DrawCursorLine()
        {
            pointMiddle = new Vector3((pointStart.x + pointEnd.x)/2, 
                (pointStart.y + pointEnd.y) / 2,point2YPosition );

            var pointList = new List<Vector3>();
            
            for(float ratio = 0;ratio<=1;ratio+= 1/vertexCount)
            {
                var tangent1 = Vector3.Lerp(pointStart, pointMiddle, ratio);
                var tangent2 = Vector3.Lerp(pointMiddle, pointEnd, ratio);
                var curve = Vector3.Lerp(tangent1, tangent2, ratio);
            
                pointList.Add(curve);
            }
            
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }
    }
}