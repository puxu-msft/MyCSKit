using System;
using System.Linq;
using System.Linq.Expressions;

namespace My
{
    public static class New<T>
    {
        private readonly static Type TypeOfT = typeof(T);

        public static T Create()
        {
            return Build.Instantiate();
        }
        public static T Create<Arg1>(Arg1 arg1)
        {
            return Build<Arg1>.Instantiate(arg1);
        }
        public static T Create<Arg1, Arg2>(Arg1 arg1, Arg2 arg2)
        {
            return Build<Arg1, Arg2>.Instantiate(arg1, arg2);
        }

        public static Func<T> Construct()
        {
            return Build.Constructor;
        }
        public static Func<Arg1, T> Construct<Arg1>()
        {
            return Build<Arg1>.Constructor;
        }
        public static Func<Arg1, Arg2, T> Construct<Arg1, Arg2>()
        {
            return Build<Arg1, Arg2>.Constructor;
        }

        private static class Helper
        {
            public static Tuple<NewExpression, ParameterExpression[]> CreateExpressions(Type[] argsTypes)
            {
                var constructorInfo = TypeOfT.GetConstructor(argsTypes);
                var constructorParameters = argsTypes.Select(p => Expression.Parameter(p)).ToArray();
                var expression = Expression.New(constructorInfo, constructorParameters);
                return new Tuple<NewExpression, ParameterExpression[]>(expression, constructorParameters);
            }
        }

        private static class Build
        {
            public static readonly Func<T> Constructor = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
            public static T Instantiate() => Constructor.Invoke();
        }

        private static class Build<Arg1>
        {
            public static readonly Func<Arg1, T> Constructor = CreateConstructor();
            public static T Instantiate(Arg1 arg) => Constructor.Invoke(arg);
            private static Func<Arg1, T> CreateConstructor()
            {
                var expressionAndParam = Helper.CreateExpressions(new Type[] { typeof(Arg1) });
                return Expression.Lambda<Func<Arg1, T>>(expressionAndParam.Item1, expressionAndParam.Item2).Compile();
            }
        }

        private static class Build<Arg1, Arg2>
        {
            public static Func<Arg1, Arg2, T> Constructor = CreateConstructor();
            public static T Instantiate(Arg1 arg1, Arg2 arg2) => Constructor.Invoke(arg1, arg2);
            private static Func<Arg1, Arg2, T> CreateConstructor()
            {
                var expressionAndParam = Helper.CreateExpressions(new Type[] { typeof(Arg1), typeof(Arg2) });
                return Expression.Lambda<Func<Arg1, Arg2, T>>(expressionAndParam.Item1, expressionAndParam.Item2).Compile();
            }
        }
    }
}
