using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class Jobs1 : MonoBehaviour
{
    [SerializeField] private bool useJobs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useJobs)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);

            for (int i = 0; i < 10; i++)
            {
                MyJob1 job = new MyJob1();
                JobHandle handle = job.Schedule();
                // handle.Complete(); // wait for job is complete
                handles.Add(handle);
            }

            JobHandle.CompleteAll(handles.ToArray(Allocator.Temp));
            handles.Dispose(); // release memory
        }
        else
        {
            for (int j = 0; j < 10; j++)
            {
                float value = 0f;
                for (int i = 0; i < 10000; i++)
                {
                    value = math.sqrt(i);
                }
            }
        }
    }
}

[BurstCompile]
public struct MyJob1 : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 10000; i++)
        {
            value = math.sqrt(i);
        }
    }
}
