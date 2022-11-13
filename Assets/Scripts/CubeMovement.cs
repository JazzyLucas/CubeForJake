using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JazzApps
{
    [System.Serializable]
    public enum AXIS
    {
        X,
        Y,
        Z
    }
    
    public class CubeMovement : MonoBehaviour
    {
        //// Externals
        [SerializeField] private GameObject cubeToMove;
        [SerializeField] private Transform startTransform, endTransform;
        [SerializeField] private int rotateIntervals = 10;
        [SerializeField] private int moveIntervals = 100;
        [SerializeField] private AXIS axisToMoveOn = AXIS.X;
        [SerializeField] private AXIS axisToRotateOn = AXIS.Z;
        [SerializeField] private bool reverseMovement = true;
        [SerializeField] private bool reverseRotation = false;
        [SerializeField] private MeshRenderer chromaMesh;
        [SerializeField] private Gradient chromaGradient;
        [SerializeField] private float chromaSpeed;
        
        //// Internals
        // Input
        private Transform currentTransform;
        private float scrollDelta;
        // Local Variables
        private float amountMoved;
        private float amountRotated;
        private float amountToMove;
        private float amountToRotate;
        private float moveInterval;
        private float rotateInterval;
        private float chromaTime = 0;

        /// <summary>
        /// (Can be used to restart)
        /// </summary>
        private void Init()
        {
            Debug.Log("Reset.");
            var startPosition = startTransform.position;
            var endPosition = endTransform.position;
            
            // Reset variables
            amountMoved = 0;
            amountToMove = 0;
            moveInterval = 0;
            amountRotated = 0;
            amountToRotate = 0;
            rotateInterval = 0;
            chromaTime = 0;

            cubeToMove.transform.position = startPosition;
            amountToMove = Vector3.Distance(startPosition, endPosition);
            amountToRotate = 360 * rotateIntervals;
            moveInterval = amountToMove / moveIntervals;
            rotateInterval = amountToRotate / moveIntervals;
        }
        
        private void PollInput()
        {
            scrollDelta += Input.mouseScrollDelta.y;
        }

        private void HandleCubeTransform()
        {
            // Guard Clause
            if (scrollDelta == 0) return;

            float movementDelta = moveInterval * scrollDelta;
            float rotationDelta = rotateInterval * scrollDelta;
            amountMoved += amountMoved >= 0 ? movementDelta : 0;
            amountRotated += amountRotated >= 0 ? rotationDelta : 0;
            
            if (amountMoved >= amountToMove)
            {
                Init();
                return;
            }
            else if (amountRotated >= amountToRotate)
            {
                Init();
                return;
            }

            MoveCubePosition(movementDelta);
            RotateCubeBasedOnPosition(rotationDelta);
            
            scrollDelta = 0;
        }
        
        private void MoveCubePosition(float amount)
        {
            switch (axisToMoveOn)
            {
                case AXIS.X:
                    cubeToMove.transform.position = new Vector3(cubeToMove.transform.position.x + amount, cubeToMove.transform.position.y, cubeToMove.transform.position.z);
                    break;
                case AXIS.Y:
                    cubeToMove.transform.position = new Vector3(cubeToMove.transform.position.x, cubeToMove.transform.position.y  + amount, cubeToMove.transform.position.z);
                    break;
                case AXIS.Z:
                    cubeToMove.transform.position = new Vector3(cubeToMove.transform.position.x, cubeToMove.transform.position.y, cubeToMove.transform.position.z  + amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RotateCubeBasedOnPosition(float amount)
        {
            switch (axisToRotateOn)
            {
                case AXIS.X:
                    cubeToMove.transform.Rotate(reverseRotation ? -amount : amount, 0,0);
                    break;
                case AXIS.Y:
                    cubeToMove.transform.Rotate(0, reverseRotation ? -amount : amount,0);
                    break;
                case AXIS.Z:
                    cubeToMove.transform.Rotate(0, 0,reverseRotation ? -amount : amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChromaEffect()
        {
            chromaTime = Mathf.Repeat(chromaTime += Time.deltaTime * chromaSpeed, 1);
            chromaMesh.material.color = new Color(chromaGradient.Evaluate(chromaTime).r, chromaGradient.Evaluate(chromaTime).g, chromaGradient.Evaluate(chromaTime).b, chromaMesh.material.color.a);
        }
        
        #region Unity Callbacks
        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            PollInput();
            HandleCubeTransform();
            if (chromaMesh != null)
                ChromaEffect();
            
        }
        #endregion
        
        
        
    }
}
