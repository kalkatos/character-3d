using UnityEngine;
using UnityEngine.Events;

namespace Kalkatos.UnityGame.Character3D
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed;
        [SerializeField] private bool rotateToMoveDirection;
        [SerializeField] private float rotationThreshold;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform rotationObject;
        [Header("References")]
        [SerializeField] private ScriptableObject input;
        [SerializeField] private Rigidbody rb;

        private IInput inputSource;
        private Vector3 targetDirection = Vector3.forward;

        private void Reset ()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            CheckInputSource();
        }

        private void OnValidate ()
        {
            CheckInputSource();
        }

        private void Awake ()
        {
            if (input == null)
            {
                //Logger.LogError("Input object must be set.");
            }
            inputSource = (IInput)input;
            if (rotationObject == null)
                rotationObject = transform;
        }

        private void Update ()
        {
            if (rotateToMoveDirection && targetDirection != Vector3.zero && Vector3.Dot(rotationObject.forward, targetDirection) < rotationThreshold)
                rotationObject.forward = Vector3.Lerp(rotationObject.forward, targetDirection, rotationSpeed);
        }

        private void FixedUpdate ()
        {
            Vector3 direction = new Vector3(inputSource.Input.x, 0, inputSource.Input.y);
            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.z) > 1)
                direction.Normalize();
            Vector3 velocity = rb.velocity;
            velocity.x = direction.x * speed;
            velocity.z = direction.z * speed;
            rb.velocity = velocity;
            targetDirection = direction;
        }

        private void CheckInputSource ()
        {
            if (input != null && !(input is IInput))
            {
                //Logger.LogError("Input object must implement IInput interface.");
            }
        }
    }
}