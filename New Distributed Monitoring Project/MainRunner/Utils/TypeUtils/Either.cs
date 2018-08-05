using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public abstract class Either<A, B>
    {
        public static Either<A, B> Create(A a) => new Choice1(a);
        public static Either<A, B> Create(B b) => new Choice2(b);
        public bool IsChoice1 => this is Choice1;
        public bool IsChoice2 => this is Choice2;
        public abstract A GetChoice1 { get; }
        public abstract B GetChoice2 { get; }

        public static implicit operator Either<A, B>(A a) => Create(a);
        public static implicit operator Either<A, B>(B b) => Create(b);

        private sealed class Choice1 : Either<A, B>
        {
            private A a { get; }

            public Choice1(A a)
            {
                this.a = a;
            }

            public override A GetChoice1 => a;
            public override B GetChoice2 => throw new TypeAccessException();
        }
        private sealed class Choice2 : Either<A, B>
        {
            private B b { get; }

            public Choice2(B b)
            {
                this.b = b;
            }
            public override A GetChoice1 => throw new TypeAccessException();
            public override B GetChoice2 => b;
        }
    }
}
