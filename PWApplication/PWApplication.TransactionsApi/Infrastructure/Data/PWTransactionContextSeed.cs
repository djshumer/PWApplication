using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Transaction.Api.Infrastructure.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Transaction.Api.Infrastructure.Data
{
    public class PWTransactionContextSeed
    {
        public async Task SeedAsync(PWTranscationContext context, IHostingEnvironment env, ILogger<PWTransactionContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(PWTransactionContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                if (!context.PWTransactions.Any())
                {
                    AddTransactions(context);

                    await context.SaveChangesAsync();
                }
            });
        }


        private Policy CreatePolicy(ILogger<PWTransactionContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }

        private void AddTransactions(PWTranscationContext context)
        {
            var users = context.Users.Where(c => c.UserName.Contains("demo") || c.UserName.Contains("system")).ToList();
            var demoUser = users.FirstOrDefault(user => user.UserName.Contains("demouser@microsoft.com"));
            var systemUser = users.FirstOrDefault(user => user.UserName.Contains("system@microsoft.com"));

            var demotransaction = new PWTransaction()
            {
                Id = Guid.NewGuid(),
                AgentId = demoUser.Id,
                СounteragentId = systemUser.Id,
                OperationDateTime = DateTime.UtcNow.AddDays(-30),
                AgentBalance = 100000,
                TransactionAmount = 100000,
                Description = "Bonus"
            };

            context.PWTransactions.Add(demotransaction);

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] != systemUser && users[i] != demoUser)
                {
                    PWTransaction transaction = new PWTransaction()
                    {
                        Id = Guid.NewGuid(),
                        AgentId = users[i].Id,
                        СounteragentId = systemUser.Id,
                        OperationDateTime = DateTime.UtcNow.AddDays(-30).AddTicks(i),
                        AgentBalance = 500,
                        TransactionAmount = 500,
                        Description = "Bonus"
                    };
                    context.PWTransactions.Add(transaction);
                }
            }

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] != systemUser)
                {
                    DateTime dateOper = DateTime.UtcNow.AddDays(-25).AddTicks(i);
                    PWTransaction transactionOne = new PWTransaction()
                    {
                        Id = Guid.NewGuid(),
                        AgentId = demoUser.Id,
                        СounteragentId = users[i].Id,
                        OperationDateTime = dateOper,
                        AgentBalance = 100000 - i * 50 - 50,
                        TransactionAmount = -50,
                        Description = ""
                    };

                    context.PWTransactions.Add(transactionOne);

                    PWTransaction transactionTwo = new PWTransaction()
                    {
                        Id = Guid.NewGuid(),
                        AgentId = users[i].Id,
                        СounteragentId = demoUser.Id,
                        OperationDateTime = dateOper,
                        AgentBalance = 550,
                        TransactionAmount = 50,
                        Description = ""
                    };

                    context.PWTransactions.Add(transactionTwo);

                    PWOperationPair operationPair = new PWOperationPair()
                    {
                        TransactionOneId = transactionOne.Id,
                        TransactionTwoId = transactionTwo.Id
                    };

                    context.PWOperationPairs.Add(operationPair);

                }
            }

        }
    }
}
