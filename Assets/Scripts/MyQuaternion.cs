using System;
using UnityEngine;

/// <summary>
/// Własny kwaternion używany do obliczania obrotu okrętu i kamery.
/// Części x, y i z opisują składową osiową, a w jest częścią rzeczywistą.
/// </summary>
[Serializable]
public struct MyQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public MyQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>Obrót zerowy: wektor po obrocie pozostaje bez zmian.</summary>
    public static MyQuaternion Identity => new MyQuaternion(0f, 0f, 0f, 1f);

    /// <summary>
    /// Buduje kwaternion z osi i kąta. Wektor osi normalizujemy, a kąt dzielimy
    /// przez dwa, ponieważ kwaternion przechowuje sinus i cosinus połowy kąta.
    /// </summary>
    public static MyQuaternion FromAxisAngle(Vector3 axis, float angleDegrees)
    {
        float axisLength = Mathf.Sqrt(axis.x * axis.x + axis.y * axis.y + axis.z * axis.z);
        if (axisLength < 0.000001f)
        {
            return Identity;
        }

        float halfAngleRadians = angleDegrees * Mathf.Deg2Rad * 0.5f;
        float sine = Mathf.Sin(halfAngleRadians);
        float cosine = Mathf.Cos(halfAngleRadians);

        // Wzór: q = (oś * sin(kąt/2), cos(kąt/2)).
        return new MyQuaternion(
            axis.x / axisLength * sine,
            axis.y / axisLength * sine,
            axis.z / axisLength * sine,
            cosine).Normalized();
    }

    /// <summary>
    /// Iloczyn Hamiltona łączy dwa obroty. Kolejność ma znaczenie:
    /// a * b oznacza, że obrót b jest wykonywany jako pierwszy, potem a.
    /// </summary>
    public static MyQuaternion operator *(MyQuaternion a, MyQuaternion b)
    {
        return new MyQuaternion(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
    }

    /// <summary>
    /// Dzieli każdą składową przez długość kwaternionu, aby zachować czysty obrót
    /// bez przypadkowego skalowania. Dla długości bliskiej zeru zwraca Identity.
    /// </summary>
    public MyQuaternion Normalized()
    {
        float length = Mathf.Sqrt(x * x + y * y + z * z + w * w);
        if (length < 0.000001f)
        {
            return Identity;
        }

        float inverseLength = 1f / length;
        return new MyQuaternion(x * inverseLength, y * inverseLength, z * inverseLength, w * inverseLength);
    }

    /// <summary>
    /// Sprzężenie zmienia znak części osiowej, a część rzeczywistą zostawia.
    /// Dla kwaternionu jednostkowego sprzężenie opisuje obrót odwrotny.
    /// </summary>
    public MyQuaternion Conjugate()
    {
        return new MyQuaternion(-x, -y, -z, w);
    }

    /// <summary>
    /// Obraca wektor wzorem q * (v, 0) * sprzężenie(q).
    /// Wektor v zapisujemy jako kwaternion z w = 0; mnożenia wykonuje operator powyżej.
    /// </summary>
    public Vector3 RotateVector(Vector3 vector)
    {
        MyQuaternion unitRotation = Normalized();
        MyQuaternion pureVector = new MyQuaternion(vector.x, vector.y, vector.z, 0f);
        MyQuaternion rotated = unitRotation * pureVector * unitRotation.Conjugate();
        return new Vector3(rotated.x, rotated.y, rotated.z);
    }
}
