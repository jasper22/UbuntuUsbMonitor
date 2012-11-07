namespace ZeroMQ.Interop
{
    using System;

    internal static class Retry
    {
        public static int IfInterrupted<T1, T2, T3>(Func<T1, T2, T3, int> operation, T1 arg1, T2 arg2, T3 arg3)
        {
            int rc;

            do
            {
                try
                {
                    rc = operation(arg1, arg2, arg3);
                }
                catch (Exception exp_gen)
                {
                    System.Diagnostics.Trace.WriteLine("Exception here");
                    rc = -1;
                }
            }
            while (rc == -1 && LibZmq.zmq_errno() == ErrorCode.EINTR);

            return rc;
        }

        public static int IfInterrupted<T1, T2, T3, T4>(Func<T1, T2, T3, T4, int> operation, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int rc;

            do
            {
                rc = operation(arg1, arg2, arg3, arg4);
            }
            while (rc == -1 && LibZmq.zmq_errno() == ErrorCode.EINTR);

            return rc;
        }
    }
}
