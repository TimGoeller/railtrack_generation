using System;
using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEditor;

public struct RailCurve
{
    public TrackConnectionPoint Start, End;

    public Vector3 BezierHandle;

    private const int SampleCountMultiplier = 2;

    public RailCurve(TrackConnectionPoint start, TrackConnectionPoint end, Vector3 bezierHandle)
    {
        BezierHandle = bezierHandle;
        Start = start;
        End = end;
    }

    public List<Vector3> CalculateSegmentPoints(float spacing, float segmentWidth, bool startWithSpacing)
    {
        List<SamplingStep> samples = CollectSamples();

        float[] stepsArray = samples.SelectMany(sample => new List<float>() { sample.OffsetOnTrack }).ToArray();

        int maxSegmentsFitting = Convert.ToInt32((samples.Last().OffsetOnTrack - segmentWidth - (startWithSpacing ? spacing : 0)) / (segmentWidth + spacing));
        float adjustedSpacing = (samples.Last().OffsetOnTrack - segmentWidth - (startWithSpacing ? spacing : 0) - (maxSegmentsFitting * segmentWidth)) / maxSegmentsFitting;

        float segmentIterator = startWithSpacing ? spacing : 0;

        List<Vector3> finalPositions = new List<Vector3>();
        for (int i = 0; i <= maxSegmentsFitting; i++)
        {
            finalPositions.Add(GetPointInSteps(stepsArray, samples, segmentIterator));
            segmentIterator += segmentWidth + adjustedSpacing;
        }

        return finalPositions;
    }

    public List<SamplingStep> CollectSamples()
    {
        int samplePointCount = DetermineSamplePointCount();

        double samplingStep = 1f / samplePointCount;

        double currentStep;

        List<SamplingStep> samples = new List<SamplingStep>();

        for (int step = 0; step <= samplePointCount; step++)
        {
            currentStep = step * samplingStep;
            samples.Add(new SamplingStep(EvaluateOnSegment((float)currentStep)));
        }

        float totalSampleLength = 0f;

        int index = 0;
        foreach (SamplingStep step in samples)
        {
            if (index == samples.Count - 1) break;

            Vector3 nextStep = samples[index + 1].SamplePositon;
            samples[index].SetVectorToNextSample(nextStep - step.SamplePositon);
            step.SetOffset(totalSampleLength);

            totalSampleLength += samples[index].VectorToNextSample.magnitude;

            index++;
        }

        samples.Last().SetOffset(totalSampleLength);

        return samples;
    }

    public int DetermineSamplePointCount()
    {
        Vector3 midPoint = EvaluateOnSegment(0.5f);
        int roughLengthEstimation = (int)((midPoint - Start.Point).magnitude + (End.Point - midPoint).magnitude);
        if (roughLengthEstimation < 1) roughLengthEstimation = 1;

        return roughLengthEstimation * SampleCountMultiplier;
    }

    public Vector3 EvaluateOnSegment(float t)
    {
        Vector3 p0 = Vector3.Lerp(Start.Point, BezierHandle, t);
        Vector3 p1 = Vector3.Lerp(Start.Point, End.Point, t);
        return Vector3.Lerp(p0, p1, t);
    }


    public Vector3 GetPointInSteps(float[] steps, List<SamplingStep> samples, float t)
    {
        int found = Array.BinarySearch(steps, t);

        if (found >= 0)
        {
            return samples[found].SamplePositon;
        }
        else
        {
            int higherSampleIndex = (-found) - 1;
            int lowerSampleIndex = higherSampleIndex - 1;
            float higherStepValue = steps[higherSampleIndex];
            float lowerStepValue = steps[lowerSampleIndex];

            float tOffset = t - lowerStepValue;

            float tPercentage = tOffset / (higherStepValue - lowerStepValue);

            return samples[lowerSampleIndex].SamplePositon + (samples[lowerSampleIndex].VectorToNextSample * tPercentage);
        }
    }

    public class SamplingStep
    {
        public Vector3 SamplePositon;
        public Vector3 VectorToNextSample;
        public float OffsetOnTrack;

        public SamplingStep(Vector3 samplePositon)
        {
            SamplePositon = samplePositon;
        }

        public void SetOffset(float offset)
        {
            OffsetOnTrack = offset;
        }

        public void SetVectorToNextSample(Vector3 vectorToNextSample)
        {
            VectorToNextSample = vectorToNextSample;
        }
    }
}