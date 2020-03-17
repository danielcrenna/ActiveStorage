// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TypeKitchen;

namespace ActiveStorage.Azure.TableStorage
{
	public class FastTableEntity : ITableEntity
	{
		private readonly object _entity;
		private readonly ILogger _logger;

		public FastTableEntity(object entity, string partitionKey, string rowKey, ILogger logger = null)
		{
			_entity = entity;
			PartitionKey = partitionKey;
			RowKey = rowKey;
			_logger = logger;
		}

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string ETag { get; set; }

		public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
		{
			var accessor = WriteAccessor.Create(_entity, AccessorMemberTypes.Properties, AccessorMemberScope.Public,
				out var members);

			foreach (var member in members)
			{
				if (ShouldSkipProperty(member, operationContext))
					continue;

				if (!properties.ContainsKey(member.Name))
				{
					_logger.LogInformation(FormatLine(operationContext,
						"Omitting property '{0}' from de-serialization because there is no corresponding entry in the dictionary provided.",
						member.Name));
					continue;
				}

				var property = properties[member.Name];

				if (property.PropertyAsObject == default)
					accessor[_entity, member.Name] = default;
				else
				{
					switch (property.PropertyType)
					{
						case EdmType.String:
							if (member.Type == typeof(double) || member.Type == typeof(double?))
								accessor[_entity, member.Name] = property.DoubleValue;
							else if (member.Type != typeof(string))
								continue;
							accessor[_entity, member.Name] = property.StringValue;
							continue;
						case EdmType.Binary:
							if (member.Type == typeof(byte[]))
								accessor[_entity, member.Name] = property.BinaryValue;
							continue;
						case EdmType.Boolean:
							if (member.Type == typeof(bool) || member.Type == typeof(bool?))
								accessor[_entity, member.Name] = property.BooleanValue;
							continue;
						case EdmType.DateTime:
							if (member.Type == typeof(DateTime))
							{
								accessor[_entity, member.Name] =
									property.DateTimeOffsetValue.GetValueOrDefault().UtcDateTime;
								continue;
							}

							if (member.Type == typeof(DateTime?))
							{
								accessor[_entity, member.Name] = property.DateTimeOffsetValue?.UtcDateTime;
								continue;
							}

							if (member.Type == typeof(DateTimeOffset))
							{
								accessor[_entity, member.Name] = property.DateTimeOffsetValue.GetValueOrDefault();
								continue;
							}

							if (member.Type == typeof(DateTimeOffset?))
								accessor[_entity, member.Name] = property.DateTimeOffsetValue;
							continue;
						case EdmType.Double:
							if (member.Type == typeof(double) || member.Type == typeof(double?))
								accessor[_entity, member.Name] = property.DoubleValue;
							continue;
						case EdmType.Guid:
							if (member.Type == typeof(Guid) || member.Type == typeof(Guid?))
								accessor[_entity, member.Name] = property.GuidValue;
							continue;
						case EdmType.Int32:
							if (member.Type == typeof(int) || member.Type == typeof(int?))
								accessor[_entity, member.Name] = property.Int32Value;
							continue;
						case EdmType.Int64:
							if (member.Type == typeof(long) || member.Type == typeof(long?))
								accessor[_entity, member.Name] = property.Int64Value;
							continue;
						default:
							continue;
					}
				}
			}
		}

		public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
		{
			var map = new Dictionary<string, EntityProperty>();
			var accessor = ReadAccessor.Create(_entity, AccessorMemberTypes.Properties, AccessorMemberScope.Public,
				out var members);
			foreach (var member in members)
			{
				if (ShouldSkipProperty(member, operationContext))
					continue;
				if (!accessor.TryGetValue(_entity, member.Name, out var value))
					continue;

				var entityValue = EntityProperty.CreateEntityPropertyFromObject(value);
				if (entityValue != null)
					map.Add(member.Name, entityValue);
			}

			return map;
		}

		private bool ShouldSkipProperty(AccessorMember member, OperationContext operationContext)
		{
			var name = member.Name;

			if (name == nameof(ITableEntity.PartitionKey) ||
			    name == nameof(ITableEntity.RowKey) ||
			    name == nameof(ITableEntity.Timestamp) ||
			    name == nameof(ITableEntity.ETag))
			{
				return true;
			}

			if (!member.CanRead || !member.CanWrite)
			{
				_logger?.LogInformation(
					"Omitting property '{0}' from serialization/de-serialization because the property's getter/setter are not public.",
					member.Name);

				return true;
			}

			if (member.HasAttribute<IgnorePropertyAttribute>())
			{
				_logger?.LogInformation(
					FormatLine(operationContext,
						$"Omitting property '{0}' from serialization/de-serialization because {nameof(IgnorePropertyAttribute)} has been set on that property.",
						member.Name));

				return true;
			}

			return false;
		}

		private static string FormatLine(
			OperationContext operationContext,
			string format,
			params object[] args)
		{
			var culture = CultureInfo.InvariantCulture;
			return string.Format(culture, "{0}: {1}",
				operationContext == null ? "*" as object : operationContext.ClientRequestID,
				args == null
					? format
					: string.Format(culture, format, args).Replace('\n', '.') as object);
		}
	}
}