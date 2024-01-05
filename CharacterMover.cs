using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Kalkatos.UnityGame.Character3D
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private FloatValueGetter speed;
        [SerializeField] private bool rotateToMoveDirection;
        [SerializeField] private float rotationThreshold;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform rotationObject;
        [Header("References")]
        [SerializeField] private InputObjectType inputObjectType;
#if ODIN_INSPECTOR
        [ShowIf(nameof(inputObjectType), InputObjectType.ScriptableObject)] 
#endif
        [SerializeField] private ScriptableObject inputScriptable;
#if ODIN_INSPECTOR
        [ShowIf(nameof(inputObjectType), InputObjectType.MonoBehaviour)]
#endif
        [SerializeField] private MonoBehaviour inputMono;
        [SerializeField] private Rigidbody rb;

        private bool canMove = true;
        private IInput inputSource;
        private Vector3 targetDirection = Vector3.forward;

        public enum InputObjectType { ScriptableObject, MonoBehaviour }

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
            switch (inputObjectType)
            {
                case InputObjectType.ScriptableObject:
                    if (inputScriptable == null)
                        Logger.LogError("Input object must be set.");
                    inputSource = (IInput)inputScriptable;
                    break;
                case InputObjectType.MonoBehaviour:
                    if (inputMono == null)
                        Logger.LogError("Input object must be set.");
                    inputSource = (IInput)inputMono;
                    break;
            }
            if (rotationObject == null)
                rotationObject = transform;
        }

        private void Update ()
        {
            if (!canMove)
                return;
            if (rotateToMoveDirection && targetDirection != Vector3.zero && Vector3.Dot(rotationObject.forward, targetDirection) < rotationThreshold)
                rotationObject.forward = Vector3.Lerp(rotationObject.forward, targetDirection, rotationSpeed);
        }

        private void FixedUpdate ()
        {
            if (!canMove)
                return;
            Vector3 direction = new Vector3(inputSource.Input.x, 0, inputSource.Input.y);
            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.z) > 1)
                direction.Normalize();
            Vector3 velocity = rb.velocity;
            velocity.x = direction.x * speed.GetValue();
            velocity.z = direction.z * speed.GetValue();
            rb.velocity = velocity;
            targetDirection = direction;
        }

        private void CheckInputSource ()
        {
            switch (inputObjectType)
            {
                case InputObjectType.ScriptableObject:
                    if (inputScriptable != null && !(inputScriptable is IInput))
                        Logger.LogError("Input object must implement IInput interface.");
                    break;
                case InputObjectType.MonoBehaviour:
                    if (inputMono != null && !(inputMono is IInput))
                        Logger.LogError("Input object must implement IInput interface.");
                    break;
            }
        }

        public void SetCanMove (bool b)
        {
            canMove = b;
        }

        public void SetInput (IInput inputSource)
        {
            this.inputSource = inputSource;
        }

        public void SetInput (ScriptableObject inputObj)
        {
            if (!(inputObj is IInput))
            {
                Logger.LogError("Input object must implement IInput interface.");
                return;
            }
            inputObjectType = InputObjectType.ScriptableObject;
            inputSource = (IInput)inputObj;
        }

        public void SetInput (MonoBehaviour mono)
        {
            if (!(mono is IInput))
            {
                Logger.LogError("Input object must implement IInput interface.");
                return;
            }
            inputObjectType = InputObjectType.MonoBehaviour;
            inputSource = (IInput)mono;
        }

        public void SetSpeed (float value)
        {
            speed.Type = FloatValueGetter.ValueType.Simple;
            speed.SimpleValue = value;
        }

        public void SetSpeed (FloatValueGetter valueGetter)
        {
            speed = valueGetter;
        }
    }
}