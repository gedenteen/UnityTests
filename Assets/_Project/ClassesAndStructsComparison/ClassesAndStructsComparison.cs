using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ClassSample
{
    public byte Byte;
    public float Float;
    public int Int;
    public long Long;
    public decimal Decimal1;
    public decimal Decimal2;
    public decimal Decimal3;
    public decimal Decimal4;
    public decimal Decimal5;
    public decimal Decimal6;
    public decimal Decimal7;
    public decimal Decimal8;
    public decimal Decimal9;
    public decimal Decimal10;

    public ClassSample(byte b, float f, int i, long l, decimal d)
    {
        Byte = b;
        Float = f;
        Int = i;
        Long = l;
        Decimal1 = d;
        Decimal2 = d;
        Decimal3 = d;
        Decimal4 = d;
        Decimal5 = d;
        Decimal6 = d;
        Decimal7 = d;
        Decimal8 = d;
        Decimal9 = d;
        Decimal10 = d;
    }
}

public struct StructSample
{
    public byte Byte;
    public float Float;
    public int Int;
    public long Long;
    public decimal Decimal1;
    public decimal Decimal2;
    public decimal Decimal3;
    public decimal Decimal4;
    public decimal Decimal5;
    public decimal Decimal6;
    public decimal Decimal7;
    public decimal Decimal8;
    public decimal Decimal9;
    public decimal Decimal10;

    public StructSample(byte b, float f, int i, long l, decimal d)
    {
        Byte = b;
        Float = f;
        Int = i;
        Long = l;
        Decimal1 = d;
        Decimal2 = d;
        Decimal3 = d;
        Decimal4 = d;
        Decimal5 = d;
        Decimal6 = d;
        Decimal7 = d;
        Decimal8 = d;
        Decimal9 = d;
        Decimal10 = d;
    }
}

public class ClassesAndStructsComparison : MonoBehaviour
{
    [SerializeField] private int _instancesCount = 999000;
    [SerializeField] private TMP_Text _textCreationClassesTime;
    [SerializeField] private TMP_Text _textCreationStructsTime;
    [SerializeField] private TMP_Text _textMethodClassesTime;
    [SerializeField] private TMP_Text _textMethodUsualStructsTime;
    [SerializeField] private TMP_Text _textMethodViaRefStructsTime;

    private readonly Stopwatch _stopwatch = new Stopwatch();
    double _sum = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }

    private void Test()
    {
        // ================ TEST 1 ================ //

        _stopwatch.Restart();

        for (int i = 0; i < _instancesCount; i++)
        {
            ClassSample classSample = new ClassSample(8, 9f, 10, -1, -2);
            // _sum += classSample.Byte + classSample.Float + classSample.Int;
        }

        _stopwatch.Stop();
        long classNanoseconds = _stopwatch.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        _textCreationClassesTime.text = $"{classNanoseconds} ns";
        UnityEngine.Debug.Log($"Creation classes: _sum={_sum}, time={classNanoseconds} ns");

        // ================ TEST 2 ================ //

        _stopwatch.Restart();

        for (int i = 0; i < _instancesCount; i++)
        {
            StructSample structSample = new StructSample(8, 9f, 10, -1, -2);
            // _sum += structSample.Byte + structSample.Float + structSample.Int;
        }

        _stopwatch.Stop();
        long structNanoseconds = _stopwatch.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        _textCreationStructsTime.text = $"{structNanoseconds} ns";
        UnityEngine.Debug.Log($"Creation structs: _sum={_sum}, time={structNanoseconds} ns");

        // ================ CREATION OF LISTS FOR NEXT TESTS ================ //

        ClassSample[] arrayClassSamples = new ClassSample[_instancesCount];
        StructSample[] arrayStructSamples = new StructSample[_instancesCount];

        for (int i = 0; i < _instancesCount; i++)
        {
            ClassSample classSample = new ClassSample(8, 9f, 10, -1, -2);
            arrayClassSamples[i] = classSample;
            StructSample structSample = new StructSample(8, 9f, 10, -1, -2);
            arrayStructSamples[i] = structSample;
        }

        // ================ TEST 3 ================ //

        _stopwatch.Restart();

        for (int i = 0; i < _instancesCount; i++)
        {
            DoSomethingWithClass(arrayClassSamples[i]);
        }

        _stopwatch.Stop();
        classNanoseconds = _stopwatch.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        _textMethodClassesTime.text = $"{classNanoseconds} ns";
        UnityEngine.Debug.Log($"Method and classes: _sum={_sum}, time={classNanoseconds} ns");
        _sum = 0;

        // ================ TEST 4 ================ //

        _stopwatch.Restart();

        for (int i = 0; i < _instancesCount; i++)
        {
            DoSomethingWithStruct(arrayStructSamples[i]);
        }

        _stopwatch.Stop();
        structNanoseconds = _stopwatch.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        _textMethodUsualStructsTime.text = $"{structNanoseconds} ns";
        UnityEngine.Debug.Log($"Method usual and structs: _sum={_sum}, time={structNanoseconds} ns");
        _sum = 0;

        // ================ TEST 5 ================ //

        _stopwatch.Restart();

        for (int i = 0; i < _instancesCount; i++)
        {
            DoSomethingWithStructViaRef(ref arrayStructSamples[i]);
        }

        _stopwatch.Stop();
        structNanoseconds = _stopwatch.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        _textMethodViaRefStructsTime.text = $"{structNanoseconds} ns";
        UnityEngine.Debug.Log($"Method via ref and structs: _sum={_sum}, time={structNanoseconds} ns");
        _sum = 0;
    }

    private void DoSomethingWithClass(ClassSample classSample)
    {
        _sum += classSample.Byte + classSample.Float + classSample.Int;
    }

    private void DoSomethingWithStruct(StructSample structSample)
    {
        _sum += structSample.Byte + structSample.Float + structSample.Int;
    }

    private void DoSomethingWithStructViaRef(ref StructSample structSample)
    {
        _sum += structSample.Byte + structSample.Float + structSample.Int;
    }
}