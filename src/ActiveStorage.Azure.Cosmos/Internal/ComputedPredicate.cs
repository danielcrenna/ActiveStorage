// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;

namespace ActiveStorage.Azure.Cosmos.Internal
{
	internal static class ComputedPredicate<T>
	{
		public static Expression<Func<T, bool>> AsExpression(string memberName, ExpressionOperator @operator,
			object value)
		{
			var parameter = Expression.Parameter(typeof(T), memberName);
			var memberExpression = Expression.PropertyOrField(parameter, memberName);

			var expression = @operator switch
			{
				ExpressionOperator.Equal => Expression.Equal(memberExpression, Expression.Constant(value)),
				ExpressionOperator.NotEqual => Expression.NotEqual(memberExpression, Expression.Constant(value)),
				_ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null)
			};

			return Expression.Lambda<Func<T, bool>>(expression, parameter);
		}
	}
}