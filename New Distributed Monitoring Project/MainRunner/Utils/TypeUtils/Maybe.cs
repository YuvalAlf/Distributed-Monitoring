using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils.AiderTypes.TaxiTrips;

namespace Utils.TypeUtils
{
    public static class Maybe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> Some<T>(T value) => new Maybe<T>.Some(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> None<T>() => Maybe<T>.none;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> BindMaybe<T>(this bool @this, Func<Maybe<T>> ifTrue)
        {
            if (@this)
                return ifTrue();
            else
                return Maybe<T>.none;
        }
    }

    public abstract class Maybe<T>
    {
        internal static readonly Maybe<T> none = new None();
        public abstract T ValueUnsafe { get; }
        public abstract bool IsNone { get; }
        public abstract bool IsSome(out T value);
        public abstract void Iter(Action<T> action);
        public abstract Maybe<S> Bind<S>(Func<T, Maybe<S>> bind);
        public abstract Maybe<S> Map<S>(Func<T, S> mapFunc);
        public abstract T ValueOrError(string errorMessage);
        public abstract T ValueOrElse(T @default);

        internal sealed class Some : Maybe<T>
        {
            public override T ValueUnsafe { get; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Some(T value) => ValueUnsafe = value;

            public override bool IsNone => false;
            public override bool IsSome(out T value)
            {
                value = ValueUnsafe;
                return true;
            }
            
            public override void Iter(Action<T> action) => action(ValueUnsafe);

            public override Maybe<S> Bind<S>(Func<T, Maybe<S>> bind) => bind(ValueUnsafe);

            public override Maybe<S> Map<S>(Func<T, S> mapFunc) => new Maybe<S>.Some(mapFunc(ValueUnsafe));

            public override T ValueOrError(string errorMessage) => ValueUnsafe;

            public override T ValueOrElse(T @default) => ValueUnsafe;
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

            public override void Iter(Action<T> action) {}

            public override Maybe<S> Bind<S>(Func<T, Maybe<S>> bind) => Maybe<S>.none;

            public override Maybe<S> Map<S>(Func<T, S> mapFunc) => Maybe<S>.none;

            public override T ValueOrError(string errorMessage) => throw new Exception(errorMessage);

            public override T ValueOrElse(T @default) => @default;
        }
    }
}
