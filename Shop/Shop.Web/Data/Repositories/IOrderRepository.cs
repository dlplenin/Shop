using Shop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Data.Repositories
{
	public interface IOrderRepository : IGenericRepository<Order>
	{
		Task<IQueryable<Order>> GetOrdersAsync(string userName);
	}

}
