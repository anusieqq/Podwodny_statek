using UnityEditor;
using UnityEngine;

/// <summary>
/// Proste testy uruchamiane ręcznie z menu Unity, bez dodawania obiektu do sceny.
/// </summary>
public static class MyQuaternionTests
{
    [MenuItem("Tools/Submarine/Run MyQuaternion Tests")]
    public static void RunAll()
    {
        int failures = 0;
        Vector3 forward = Vector3.forward;

        MyQuaternion turn90 = MyQuaternion.FromAxisAngle(Vector3.up, 90f);
        CheckVector("Obrót 90 stopni", turn90.RotateVector(forward), Vector3.right, ref failures);

        MyQuaternion turn180 = MyQuaternion.FromAxisAngle(Vector3.up, 180f);
        CheckVector("Obrót 180 stopni", turn180.RotateVector(forward), Vector3.back, ref failures);

        MyQuaternion turn360 = MyQuaternion.FromAxisAngle(Vector3.up, 360f);
        CheckVector("Obrót 360 stopni", turn360.RotateVector(forward), Vector3.forward, ref failures);

        MyQuaternion normalized = new MyQuaternion(0f, 2f, 0f, 2f).Normalized();
        CheckValue("Normalizacja składowej y", normalized.y, 0.7071068f, ref failures);
        CheckValue("Normalizacja składowej w", normalized.w, 0.7071068f, ref failures);

        MyQuaternion twoTurns = (turn90 * turn90).Normalized();
        CheckVector("Połączenie dwóch obrotów po 90 stopni", twoTurns.RotateVector(forward), Vector3.back, ref failures);

        Vector3 undone = turn90.Conjugate().RotateVector(turn90.RotateVector(forward));
        CheckVector("Sprzężenie odwraca obrót", undone, forward, ref failures);

        if (failures == 0)
        {
            Debug.Log("MyQuaternion: wszystkie testy zakończyły się poprawnie.");
        }
        else
        {
            Debug.LogError($"MyQuaternion: liczba nieudanych testów: {failures}.");
        }
    }

    private static void CheckVector(string testName, Vector3 actual, Vector3 expected, ref int failures)
    {
        if ((actual - expected).sqrMagnitude <= 0.0001f)
        {
            Debug.Log($"ZALICZONO: {testName}. Wynik: {actual}");
            return;
        }

        failures++;
        Debug.LogError($"BŁĄD: {testName}. Oczekiwano {expected}, otrzymano {actual}.");
    }

    private static void CheckValue(string testName, float actual, float expected, ref int failures)
    {
        if (Mathf.Abs(actual - expected) <= 0.001f)
        {
            Debug.Log($"ZALICZONO: {testName}. Wynik: {actual}");
            return;
        }

        failures++;
        Debug.LogError($"BŁĄD: {testName}. Oczekiwano {expected}, otrzymano {actual}.");
    }
}
