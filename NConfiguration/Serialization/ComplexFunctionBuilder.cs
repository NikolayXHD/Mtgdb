using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NConfiguration.Serialization
{
	internal sealed class ComplexFunctionBuilder
	{
		private Type _targetType;
		private bool _supportInitialize;
		private ParameterExpression _pDeserializer = Expression.Parameter(typeof(IDeserializer));
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;

		public ComplexFunctionBuilder(Type targetType)
		{
			_targetType = targetType;
			_supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(_targetType);
			_pResult = Expression.Parameter(_targetType);

			setConstructor();
			if (_supportInitialize)
				CallBeginInit();
		}

		private void setConstructor()
		{
			if (_targetType.IsValueType)
				return;
			
			var ci = _targetType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, new Type[] { }, null);

			Expression resultInstance;

			if (ci == null)
			{
				resultInstance = Expression.Call(typeof(FormatterServices).GetMethod("GetUninitializedObject"), Expression.Constant(_targetType));
				resultInstance = Expression.Convert(resultInstance, _targetType);
			}
			else
				resultInstance = Expression.New(ci);

			_bodyList.Add(Expression.Assign(_pResult, resultInstance));
		}

		public object Compile()
		{
			if (_supportInitialize)
				CallEndInit();

			_bodyList.Add(Expression.Label(Expression.Label(_targetType), _pResult));

			var delegateType = typeof(Deserialize<>).MakeGenericType(_targetType);

			return Expression.Lambda(delegateType, Expression.Block(new[] { _pResult }, _bodyList), _pDeserializer, _pCfgNode).Compile();
		}

		private void CallBeginInit()
		{
			var callBeginInit = Expression.Call(_pResult, typeof(ISupportInitialize).GetMethod("BeginInit"));
			_bodyList.Add(callBeginInit);
		}

		private void CallEndInit()
		{
			var callEndInit = Expression.Call(_pResult, typeof(ISupportInitialize).GetMethod("EndInit"));
			_bodyList.Add(callEndInit);
		}

		private Expression makeFieldReader(FieldFunctionInfo ffi)
		{
			if (ffi.DeserializerFactory != null)
			{
				var mi = BuildUtils.CustomFieldMi.MakeGenericMethod(ffi.ResultType);
				var customDeserializer = Expression.Constant(ffi.DeserializerFactory.CreateInstance(ffi.ResultType));
				return Expression.Call(null, mi, _pDeserializer, customDeserializer, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(ffi.Required));
			}

			if (!SimpleTypes.Converter.IsPrimitive(ffi.ResultType))
			{
				if (ffi.ResultType.IsArray)
				{
					var itemType = ffi.ResultType.GetElementType();
					var mi = BuildUtils.ToArrayMi.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode);
				}

				if (BuildUtils.IsCollection(ffi.ResultType))
				{
					var itemType = ffi.ResultType.GetGenericArguments()[0];
					var mi = BuildUtils.ToListMi.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode);
				}
			}

			{
				var mi = BuildUtils.ReadFieldMi.MakeGenericMethod(ffi.ResultType);
				return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(ffi.Required));
			}
		}

		internal void Add(FieldInfo fi)
		{
			if (fi.IsInitOnly)
				return;

			if (fi.IsPrivate)
			{ // require DataMemberAttribute
				if (fi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (fi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var right = makeFieldReader(new FieldFunctionInfo(fi));
			if (right == null)
				return;
			var left = Expression.Field(_pResult, fi);
			_bodyList.Add(Expression.Assign(left, right));
		}

		internal void Add(PropertyInfo pi)
		{
			if (!pi.CanWrite || !pi.CanRead)
				return;

			if (pi.GetSetMethod().IsPrivate && pi.GetGetMethod().IsPrivate)
			{ // require DataMemberAttribute
				if (pi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (pi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var right = makeFieldReader(new FieldFunctionInfo(pi));
			if (right == null)
				return;
			var left = Expression.Property(_pResult, pi);
			_bodyList.Add(Expression.Assign(left, right));
		}
	}
}
