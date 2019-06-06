using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class Maybe
    {
        public static Maybe<T> Some<T>(T value) => new Maybe<T>.Some(value);
        public static Maybe<T> None<T>() => new Maybe<T>.None();
    }

    public abstract class Maybe<T>
    {
        public abstract T ValueUnsafe { get; }
        public abstract bool IsNone { get; }
        public abstract bool IsSome(out T value);

        internal sealed class Some : Maybe<T>
        {
            public override T ValueUnsafe { get; }
            public override bool IsNone => false;
            public override bool IsSome(out T value)
            {
                value = ValueUnsafe;
                return true;
            }

            public Some(T value)
            {
                ValueUnsafe = value;
            }
        }
        internal sealed class None : Maybe<T>
        {
            public override T ValueUnsafe => throw new InvalidOperationException("Get value on None-Object");
            public override bool IsNone => true;
            public override bool IsSome(out T value)
            {
                value = default(T);
                return false;
            }
        }
    }
}
