using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace NConfiguration.Combination
{
	/*
		//example of builded code
		public TestAttrClass Combine(ICombiner combiner, TestAttrClass prev, TestAttrClass next)
		{
			if (prev == null) return next;
			if (next == null) return prev;
	
			var result = new TestAttrClass();
			result.F1 = combiner.Combine(combiner, prev.F1, next.F1);
			result.F2 = combiner.Combine(combiner, prev.F2, next.F2);
			return result;
		}
	*/

	internal sealed class ComplexFunctionBuilder
	{
		private Type _targetType;
		private Type _delegateType;

		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pCombiner;
		private ParameterExpression _pPrev;
		private ParameterExpression _pNext;
		private ParameterExpression _vResult;

		private LabelTarget _lbReturn;
		private bool _assingExist = false;

		public ComplexFunctionBuilder(Type targetType)
		{
			_targetType = targetType;
			_delegateType = typeof(Combine<>).MakeGenericType(_targetType);

			_pCombiner = Expression.Parameter(typeof(ICombiner));
			_pPrev = Expression.Parameter(_targetType);
			_pNext = Expression.Parameter(_targetType);
			_vResult = Expression.Variable(_targetType);

			_lbReturn = Expression.Label(_targetType);

			if (!_targetType.IsValueType)
			{
				_bodyList.Add(Expression.IfThen(Expression.Equal(_pNext, Expression.Constant(null)), Expression.Return(_lbReturn, _pPrev)));
				_bodyList.Add(Expression.IfThen(Expression.Equal(_pPrev, Expression.Constant(null)), Expression.Return(_lbReturn, _pNext)));
			}

			var ci = _targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(_ => _.GetParameters().Length == 0);
			Expression resultInstance;

			if (ci == null)
			{
				resultInstance = Expression.Call(typeof(FormatterServices).GetMethod("GetUninitializedObject"), Expression.Constant(_targetType));
				resultInstance = Expression.Convert(resultInstance, _targetType);
			}
			else
				resultInstance = Expression.New(ci);

			_bodyList.Add(Expression.Assign(_vResult, resultInstance));
		}

		private Expression callMemberCombiner(MemberInfo mi, Type targetType, Expression prev, Expression next)
		{
			try
			{
				var combinerAttr = mi.GetCustomAttributes(false).OfType<ICombinerFactory>().SingleOrDefault();
				if (combinerAttr == null)
				{
					var combineMi = typeof(ICombiner).GetMethod("Combine").MakeGenericMethod(targetType);
					return Expression.Call(_pCombiner, combineMi, _pCombiner, prev, next);
				}
				else
				{
					var customCombiner = Expression.Constant(combinerAttr.CreateInstance(targetType));
					var combineMi = typeof(ICombiner<>).MakeGenericType(targetType).GetMethod("Combine");
					return Expression.Call(customCombiner, combineMi, _pCombiner, prev, next);
				}
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create combiner for {0} member", mi.Name), ex);
			}
		}

		public void Add(FieldInfo fi)
		{
			if (fi.IsInitOnly)
				return;

			if(fi.IsPrivate)
			{ // require DataMemberAttribute
				if (fi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (fi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var prevField = Expression.Field(_pPrev, fi);
			var nextField = Expression.Field(_pNext, fi);
			var resultField = Expression.Field(_vResult, fi);
			var right = callMemberCombiner(fi, fi.FieldType, prevField, nextField);

			_bodyList.Add(Expression.Assign(resultField, right));
			_assingExist = true;
		}

		public void Add(PropertyInfo pi)
		{
			if (!pi.CanWrite || !pi.CanRead)
				return;

			if(pi.GetSetMethod().IsPrivate && pi.GetGetMethod().IsPrivate)
			{ // require DataMemberAttribute
				if (pi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (pi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var prevField = Expression.Property(_pPrev, pi);
			var nextField = Expression.Property(_pNext, pi);
			var resultField = Expression.Property(_vResult, pi);

			var right = callMemberCombiner(pi, pi.PropertyType, prevField, nextField);

			_bodyList.Add(Expression.Assign(resultField, right));
			_assingExist = true;
		}

		public object Compile()
		{
			if (!_assingExist)
				return null;

			_bodyList.Add(Expression.Return(_lbReturn, _vResult));
			_bodyList.Add(Expression.Label(_lbReturn, _vResult));

			var lambdaEx = Expression.Lambda(_delegateType, Expression.Block(new []{ _vResult }, _bodyList), _pCombiner, _pPrev, _pNext);
			return lambdaEx.Compile();
		}
	}
}
