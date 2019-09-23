using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PWApplication.Identity.Models.DataModels;

namespace PWApplication.Identity.Data
{
    public class ApplicationDbContextSeed 
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(AppDbContext context, IHostingEnvironment env,
            ILogger<ApplicationDbContextSeed> logger, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            var users = GetDefaultUsers();

            try
            {
                if (!context.Users.Any())
                {
                    context.Users.AddRange(users);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(AppDbContext));

                    await SeedAsync(context, env, logger, retryForAvaiability);
                }
            }

        }

        
        private IList<ApplicationUser> GetDefaultUsers()
        {
            ApplicationUser defUser =
            new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "John Wick",
                UserName = "demouser@microsoft.com",
                Email = "demouser@microsoft.com",
                EmailConfirmed = false,
                PhoneNumber = "",
                PhoneNumberConfirmed = false,
                NormalizedEmail = "DEMOUSER@MICROSOFT.COM",
                NormalizedUserName = "DEMOUSER@MICROSOFT.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                LockoutEnabled = false,
                AccessFailedCount = 0,
                TwoFactorEnabled = false
            };

            defUser.PasswordHash = _passwordHasher.HashPassword(defUser, "Pass@word1");

            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                defUser
            };

            ApplicationUser systemUser =
                new ApplicationUser()
                {
                    Id = "00000000-0000-0000-0000-000000000000",
                    FullName = "System",
                    UserName = "system@microsoft.com",
                    Email = "system@microsoft.com",
                    EmailConfirmed = false,
                    PhoneNumber = "",
                    PhoneNumberConfirmed = false,
                    NormalizedEmail = "system@MICROSOFT.COM",
                    NormalizedUserName = "system@MICROSOFT.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false
                };

            systemUser.PasswordHash = _passwordHasher.HashPassword(systemUser, "Pass@word1");

            users.Add(systemUser);


            for (int i = 0; i < 30; i++)
            {
                systemUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = $"Ivan Ivanov{i}",
                    UserName = $"demouser{i}@microsoft.com",
                    Email = $"demouser{i}@microsoft.com",
                    EmailConfirmed = false,
                    PhoneNumber = "",
                    PhoneNumberConfirmed = false,
                    NormalizedEmail = $"DEMOUSER{i}@MICROSOFT.COM",
                    NormalizedUserName = $"DEMOUSER{i}@MICROSOFT.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false
                };

                systemUser.PasswordHash = _passwordHasher.HashPassword(systemUser, "Pass@word1");
                users.Add(systemUser);
            }

            return users;
        }

        private void AddTransactions(IList<ApplicationUser> users, AppDbContext context)
        {
            var demoUser = context.Users.FirstOrDefault(user => user.UserName.Contains("demouser@microsoft.com"));
            var systemUser = context.Users.FirstOrDefault(user => user.UserName.Contains("system@microsoft.com"));

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
                        AgentBalance = 100000 - i*50 - 50,
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
