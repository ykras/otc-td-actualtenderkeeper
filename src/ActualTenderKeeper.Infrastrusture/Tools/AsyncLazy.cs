using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Infrastructure.Tools
{
    internal sealed class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) : base(() => Task.FromResult(valueFactory()))
        {
        }

        public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Run(taskFactory))
        {
        }
        
        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}