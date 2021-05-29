using System;

public class NBench
{
    /// <summary>
    /// Action represents a reference to a void parameterless Method.
    /// </summary>
    public static void Bench(Action handler, Action handler2)
    {
        Console.WriteLine("########## BENCHMARKING: {0}", handler.Method.Name);
        Perform(handler, handler2, 1000, 10);
    }


    private static void Perform(Action handler, Action handler2, int time, int iters)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Result res = new Result();
        long maxThroughput = 0;
        for (int i = 0; i < iters; i++)
        {
            Console.Write("---> Iteration {0,2}: ", i);
            if (handler2 == null)
            {
                res = CallWhile(handler, time);
            }
            else
            {
                res = CallWhile2(handler, handler2, time);
            }
            long curr = res.OpsPerMs;
            Console.WriteLine("{0} ops/ms", curr);
            if (curr > maxThroughput) maxThroughput = curr;
            GC.Collect();
        }
        Console.WriteLine("============ BEST ===> {0 } ops/ms", maxThroughput);
    }

    private static Result CallWhile(Action handler, int time)
    {
        const int MAX = 32;
        int start = Environment.TickCount; // Gets the number of milliseconds elapsed since the system started.
        int end = start + time;
        int curr = start;
        Result res = new Result();
        do
        {
            handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
            handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
            handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
            handler(); handler(); handler(); handler(); handler(); handler(); handler(); handler();
            curr = Environment.TickCount;
            res.ops += MAX; // res.ops accumulate the number of calls to handler

        } while (curr < end);
        res.durInMs = curr - start;
        return res;
    }
    private static Result CallWhile2(Action handler, Action handler2, int time)
    {
        const int MAX = 32;
        int start = Environment.TickCount; // Gets the number of milliseconds elapsed since the system started.
        int end = start + time;
        int curr = start;
        int curr2 = 0;
        int start2;
        int end2;
        Result res = new Result();
        do
        {
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler(); start2 = Environment.TickCount; handler2(); end2 = Environment.TickCount; curr2 += (end2 - start2);
            handler();
            curr = Environment.TickCount;
            
            res.ops += MAX; // res.ops accumulate the number of calls to handler

        } while (curr < end);
        
        res.durInMs = (curr-curr2) - start;
        return res;
    }



    struct Result
    {
        public long ops;
        public int durInMs;

        public long OpsPerMs
        {
            get
            {
                return ops / durInMs;
            }
        }

        public long OpsPerSec
        {
            get
            {
                return (ops * 1000) / durInMs;
            }
        }
    }
}