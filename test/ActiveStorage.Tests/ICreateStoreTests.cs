using System.Threading.Tasks;

namespace ActiveStorage.Tests
{
	public interface ICreateStoreTests
	{
		Task<bool> Empty_database_has_no_objects();
		Task<bool> Can_create_object_in_store();
		Task<bool> Cannot_create_same_object_twice();
	}
}
