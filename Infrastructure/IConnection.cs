using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure
{
    public interface IConnection
    {
        DbConnection Db { get; }

        void Open();

        int Execute(string sql, object parameters = null, int? commandTimeout = null);
        Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default);

        IEnumerable<T> Query<T>(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default, CommandType commandType = default);
    }
}
