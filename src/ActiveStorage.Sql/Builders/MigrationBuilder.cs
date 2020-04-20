using System.Text;
using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	internal static class MigrationBuilder
	{
		public static string Up(this ISqlDialect d, AccessorMembers members, long sequence, IDataInfoProvider provider)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				AppendCreateIfNotExists(d, members, provider, sb);
			});
		}

		private static void AppendCreateIfNotExists(ISqlDialect d, AccessorMembers members, IDataInfoProvider provider, StringBuilder sb)
		{
			sb.Append($"CREATE TABLE IF NOT EXISTS {d.StartIdentifier}{d.ResolveTableName(members)}{d.EndIdentifier}");
			sb.Append("(");

			var columns = 0;
			foreach (var member in members)
			{
				if (provider.IsIgnored(member))
					continue;

				if (provider.IsSaved(member))
					columns++;
			}

			var count = 0;
			foreach (var member in members)
			{
				if (provider.IsIgnored(member))
					continue;

				if (provider.IsSaved(member))
				{
					sb.Append($"{d.StartIdentifier}{d.ResolveColumnName(member)}{d.EndIdentifier}")
					  .Append(' ')
					  .Append(d.ResolveColumnTypeName(member))
					  .Append(d.ResolveColumnLimit(member))
					  .Append(' ')
					  .Append(d.ResolveColumnNullability(member));

					if (++count < columns)
						sb.Append(", ");
				}
			}

			sb.AppendLine(");");
		}
	}
}
