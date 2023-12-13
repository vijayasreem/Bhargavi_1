using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace bhargavitest
{
    public class CreditTransactionRepository : ICreditTransactionService
    {
        private readonly string connectionString;

        public CreditTransactionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> CreateTransaction(CreditTransactionModel transaction)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Check daily credit limit
                    decimal dailyCreditLimit = 50000;
                    decimal totalDailyCredit = await GetTotalDailyCredit(connection);
                    if (transaction.CreditAmount > dailyCreditLimit - totalDailyCredit)
                    {
                        throw new Exception("Your daily credit limit is exceeded");
                    }

                    // Check transaction count
                    int maxTransactionCount = 3;
                    int currentTransactionCount = await GetTransactionCountForToday(connection);
                    if (currentTransactionCount >= maxTransactionCount)
                    {
                        throw new Exception("Transaction limit exceeded. You cannot perform more than 3 transactions in a day");
                    }

                    // Create transaction
                    string query = "INSERT INTO CreditTransactions (CreditAmount) VALUES (@CreditAmount)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CreditAmount", transaction.CreditAmount);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Update daily credit and transaction count
                    await UpdateDailyCreditAndTransactionCount(connection, transaction.CreditAmount);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<CreditTransactionModel> GetTransaction(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM CreditTransactions WHERE Id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new CreditTransactionModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    CreditAmount = Convert.ToDecimal(reader["CreditAmount"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<List<CreditTransactionModel>> GetAllTransactions()
        {
            var transactions = new List<CreditTransactionModel>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM CreditTransactions";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                transactions.Add(new CreditTransactionModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    CreditAmount = Convert.ToDecimal(reader["CreditAmount"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return transactions;
        }

        public async Task<bool> UpdateTransaction(CreditTransactionModel transaction)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE CreditTransactions SET CreditAmount = @CreditAmount WHERE Id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CreditAmount", transaction.CreditAmount);
                        command.Parameters.AddWithValue("@Id", transaction.Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteTransaction(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "DELETE FROM CreditTransactions WHERE Id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<decimal> GetTotalDailyCredit(MySqlConnection connection)
        {
            decimal totalDailyCredit = 0;

            string query = "SELECT SUM(CreditAmount) FROM CreditTransactions WHERE DATE(DateCreated) = CURDATE()";
            using (var command = new MySqlCommand(query, connection))
            {
                object result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    totalDailyCredit = Convert.ToDecimal(result);
                }
            }

            return totalDailyCredit;
        }

        private async Task<int> GetTransactionCountForToday(MySqlConnection connection)
        {
            int transactionCount = 0;

            string query = "SELECT COUNT(*) FROM CreditTransactions WHERE DATE(DateCreated) = CURDATE()";
            using (var command = new MySqlCommand(query, connection))
            {
                object result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    transactionCount = Convert.ToInt32(result);
                }
            }

            return transactionCount;
        }

        private async Task UpdateDailyCreditAndTransactionCount(MySqlConnection connection, decimal creditAmount)
        {
            string query = "UPDATE DailyStats SET TotalCredit = TotalCredit + @CreditAmount, TransactionCount = TransactionCount + 1 WHERE Date = CURDATE()";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CreditAmount", creditAmount);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}