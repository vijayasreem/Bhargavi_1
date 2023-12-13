using bhargavitest.DTO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bhargavitest
{
    public class CreditTransactionRepository : ICreditTransactionRepository
    {
        private readonly string connectionString;

        public CreditTransactionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<CreditTransactionModel>> GetAllAsync()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("SELECT * FROM CreditTransactions", connection);
                var reader = await command.ExecuteReaderAsync();

                var transactions = new List<CreditTransactionModel>();

                while (await reader.ReadAsync())
                {
                    var transaction = new CreditTransactionModel
                    {
                        Id = reader.GetInt32("Id"),
                        Amount = reader.GetDecimal("Amount"),
                        ErrorMessage = reader.GetString("ErrorMessage"),
                        IsSuccessful = reader.GetBoolean("IsSuccessful")
                    };

                    transactions.Add(transaction);
                }

                return transactions;
            }
        }

        public async Task<CreditTransactionModel> GetByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("SELECT * FROM CreditTransactions WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var transaction = new CreditTransactionModel
                    {
                        Id = reader.GetInt32("Id"),
                        Amount = reader.GetDecimal("Amount"),
                        ErrorMessage = reader.GetString("ErrorMessage"),
                        IsSuccessful = reader.GetBoolean("IsSuccessful")
                    };

                    return transaction;
                }

                return null;
            }
        }

        public async Task AddAsync(CreditTransactionModel transaction)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("INSERT INTO CreditTransactions (Amount, ErrorMessage, IsSuccessful) VALUES (@Amount, @ErrorMessage, @IsSuccessful)", connection);
                command.Parameters.AddWithValue("@Amount", transaction.Amount);
                command.Parameters.AddWithValue("@ErrorMessage", transaction.ErrorMessage);
                command.Parameters.AddWithValue("@IsSuccessful", transaction.IsSuccessful);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(CreditTransactionModel transaction)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("UPDATE CreditTransactions SET Amount = @Amount, ErrorMessage = @ErrorMessage, IsSuccessful = @IsSuccessful WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Amount", transaction.Amount);
                command.Parameters.AddWithValue("@ErrorMessage", transaction.ErrorMessage);
                command.Parameters.AddWithValue("@IsSuccessful", transaction.IsSuccessful);
                command.Parameters.AddWithValue("@Id", transaction.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand("DELETE FROM CreditTransactions WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}