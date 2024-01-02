using UnityEngine;

namespace Kalkatos.UnityGame.Character3D
{
    [CreateAssetMenu(fileName = "AxisInput", menuName = "Character 3D/Axis Input")]
    public class AxisInput : ScriptableObject, IInput
    {
        public Vector2 Input
        {
            get => new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
        }
    }
}