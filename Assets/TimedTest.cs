using Hertzole.CecilAttributes;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace My.Darn.Namespace
{
    public class TimedTest : MonoBehaviour
    {
        [SerializeField]
        private int test = 54;

        [Timed]
        public int TestProperty { get; set; } = 12;

        // Start is called before the first frame update
        [Timed]
        void Start()
        {
            Thread.Sleep(500);
            OtherMethod();

            ProfilerDummy();
            ProfilerTest();
        }

        private void OtherMethod()
        {
            float result = Mathf.Pow(10, 50);
            TestProperty = 10;
            Timed();
        }

        [Timed]
        private void Timed()
        {
            int result = 10 + test;

            Debug.Log("This is a test " + (result + TestProperty + ReturnTest()));
        }

        [Timed]
        private int ReturnTest()
        {
            return test + TestProperty;
        }

        [MarkInProfiler]
        private int ReturnTestProfiler()
        {
            return test + TestProperty;
        }

        private void TimedTemplate()
        {
            int result = 10 + test;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Debug.Log("This is a test " + result);

            watch.Stop();

            Debug.Log($"TimedTemplate took {watch.ElapsedMilliseconds} ticks {watch.ElapsedTicks}.");
        }

        private void ProfilerDummy()
        {
            Profiler.BeginSample("ProfilerDummy");

            Debug.Log("ProiflerDummy");

            Profiler.EndSample();
        }

        [MarkInProfiler]
        private void ProfilerTest()
        {
            Debug.Log("ProfilerTest");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
