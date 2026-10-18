using UnityEngine;

/// <summary>
/// Ustawia kamerę za okrętem i kopiuje jego kierunek patrzenia.
/// </summary>
public class SubmarineFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 1.5f, -7f);

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Odczytujemy tylko składowe już wyliczone przez skrypt sterowania.
        // MyQuaternion sam obraca przesunięcie kamery względem okrętu.
        UnityEngine.Quaternion targetRotation = target.rotation;
        MyQuaternion orientation = new MyQuaternion(
            targetRotation.x,
            targetRotation.y,
            targetRotation.z,
            targetRotation.w).Normalized();

        Vector3 worldOffset = orientation.RotateVector(followOffset);
        transform.position = target.position + worldOffset;

        // Ustawiamy gotowe wartości. Nie wywołujemy Unity Quaternion.LookRotation ani innych metod.
        transform.rotation = new UnityEngine.Quaternion(
            orientation.x,
            orientation.y,
            orientation.z,
            orientation.w);
    }
}
