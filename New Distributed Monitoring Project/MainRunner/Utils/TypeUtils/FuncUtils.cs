using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.AiderTypes;

namespace Utils.TypeUtils
{
    public static class FuncUtils
    {
        public static Func<TInput, Either<TOutput, TOther>> WithEitherReturn<TInput, TOutput, TOther>(
            this Func<TInput, TOutput> @this)
            => input => @this(input);
    }
}
