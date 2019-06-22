using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure
{
    public class MySqlDbConnection: IConnection
    {
        private volatile bool _disposed;
        private Lazy<MySqlConnection> _connection;
        private readonly ILogger _logger;

        public MySqlDbConnection(IConnectionSettings settings, ILoggerFactory loggerFactory)
            : this(settings.ConnectionString, loggerFactory)
        {
        }

        public MySqlDbConnection(string connectionString, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MySqlDbConnection>();
            _connection = new Lazy<MySqlConnection>(
                () => new MySqlConnection(connectionString),
                isThreadSafe: false);
        }

        public MySqlDbConnection(MySqlConnection connection, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MySqlDbConnection>();
            _connection = new Lazy<MySqlConnection>(() => connection, isThreadSafe: true);
        }

        public DbConnection Db
        {
            get => !_disposed
                ? _connection.Value
                : throw new ObjectDisposedException(nameof(MySqlDbConnection));
        }

        public void Open()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MySqlDbConnection));
            }

            if (Db.State == ConnectionState.Open)
            {
                return;
            }

            Db.Open();

            if (_logger != null && _logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Opened connection to {0}", Db.ConnectionString);
            }
        }

        public int Execute(string sql, object parameters = null, int? commandTimeout = null)
        {
            return Db.Execute(sql, parameters, commandTimeout: commandTimeout);
        }

        public Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return Db.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            return Db.Query<T>(sql, parameters);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default, CommandType commandType = default)
        {
            return Db.QueryAsync<T>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken, commandType: commandType));
        }

    }
}
