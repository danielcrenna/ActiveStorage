using TypeKitchen;

namespace ActiveStorage.Sql.Builders
{
	internal static class MigrationBuilder
	{
		public static string Up(this ISqlDialect d, AccessorMembers members, long sequence, IDataInfoProvider provider)
		{
			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				sb.AppendLine($"[Migration({sequence})]");
				sb.AppendLine($"public sealed class Migration_{sequence} : AutoReversingMigration");
				sb.AppendLine($"{{");

				sb.AppendLine(1, "public override void Up()");
				sb.AppendLine(1, "{");
				
				sb.AppendLine(2, $"Table.Create(\"{d.ResolveTableName(members)}\")");
				foreach (var member in members)
				{
					if (provider.IsIgnored(member))
						continue;

					if (provider.IsSaved(member))
					{
						sb.Append(3, $".WithColumn(\"{d.ResolveColumnName(member)}\")");
						sb.Append($".As{d.ResolveColumnTypeName(member)}()");
						sb.AppendLine();
					}
				}
				sb.AppendLine(3, ";");
				sb.AppendLine(1, "}");

				sb.AppendLine("}");
			});
		}
	}
}
