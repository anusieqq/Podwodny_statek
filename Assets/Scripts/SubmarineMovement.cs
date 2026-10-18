using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Steruje okrętem wzdłuż jego lokalnych osi: pochyleniem, kursem i przechyleniem.
/// </summary>
public class SubmarineMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pitchSpeed = 60f;
    [SerializeField] private float yawSpeed = 90f;
    [SerializeField] private float rollSpeed = 90f;

    private MyQuaternion orientation;

    private void Awake()
    {
        // Pobieramy początkowe składowe z Transform. Nie używamy ich do obliczania obrotu.
        UnityEngine.Quaternion startingRotation = transform.rotation;
        orientation = new MyQuaternion(
            startingRotation.x,
            startingRotation.y,
            startingRotation.z,
            startingRotation.w).Normalized();

        ApplyOrientation();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        float pitchDirection = 0f;
        if (keyboard.wKey.isPressed)
        {
            pitchDirection += 1f; // W unosi dziób, czyli obraca wokół lokalnej osi X w stronę ujemną.
        }
        if (keyboard.sKey.isPressed)
        {
            pitchDirection -= 1f; // S opuszcza dziób.
        }

        float yawDirection = 0f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            yawDirection += 1f; // D i strzałka w prawo zmieniają kurs w prawo.
        }
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            yawDirection -= 1f; // A i strzałka w lewo zmieniają kurs w lewo.
        }

        float rollDirection = 0f;
        if (keyboard.qKey.isPressed)
        {
            rollDirection += 1f; // Q przechyla okręt w lewo.
        }
        if (keyboard.eKey.isPressed)
        {
            rollDirection -= 1f; // E przechyla okręt w prawo.
        }

        float pitchAngle = -pitchDirection * pitchSpeed * Time.deltaTime;
        float yawAngle = yawDirection * yawSpeed * Time.deltaTime;
        float rollAngle = rollDirection * rollSpeed * Time.deltaTime;

        // Każdy kwaternion opisuje mały obrót wokół lokalnej osi okrętu.
        MyQuaternion pitchStep = MyQuaternion.FromAxisAngle(Vector3.right, pitchAngle);
        MyQuaternion yawStep = MyQuaternion.FromAxisAngle(Vector3.up, yawAngle);
        MyQuaternion rollStep = MyQuaternion.FromAxisAngle(Vector3.forward, rollAngle);

        // Mnożenie po prawej stronie dodaje obroty w układzie lokalnym okrętu.
        // Ponieważ qA * qB obraca najpierw przez qB, kolejność działania to roll, yaw, pitch.
        orientation = (orientation * pitchStep * yawStep * rollStep).Normalized();
        ApplyOrientation();

        float thrustDirection = 0f;
        if (keyboard.spaceKey.isPressed)
        {
            thrustDirection += 1f;
        }
        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
        {
            thrustDirection -= 1f;
        }

        // Obracamy lokalny kierunek +Z własnym kwaternionem, aby płynąć dziobem okrętu.
        Vector3 forwardDirection = orientation.RotateVector(Vector3.forward);
        Vector3 displacement = forwardDirection * thrustDirection * moveSpeed * Time.deltaTime;
        transform.position += displacement;
    }

    private void ApplyOrientation()
    {
        // Przekazujemy do Unity gotowe składowe. Matematyka obrotu została wykonana przez MyQuaternion.
        transform.rotation = new UnityEngine.Quaternion(
            orientation.x,
            orientation.y,
            orientation.z,
            orientation.w);
    }
}
