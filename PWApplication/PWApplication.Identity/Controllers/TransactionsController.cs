using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PWApplication.MobileAppService.Data;
using PWApplication.MobileAppService.Models;
using PWApplication.MobileAppService.Models.DataModels;
using PWApplication.MobileAppService.Services;
using static IdentityServer4.IdentityServerConstants;

namespace PWApplication.MobileAppService.Controllers
{
    [Route("api/v1/transactions")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(AppDbContext context,
            IIdentityService identityService,
            ILogger<TransactionsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/v1/transactions/last[?count=10]
        [Route("last")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetLastTransactions([FromQuery]int count = 0)
        {
            if (count > 1000) count = 1000;

            var userid = _identityService.GetUserIdentity();

            var transactionList = await _context.PWTransactions
                .Where(c => c.AgentId == userid)
                .Include(c => c.Сounteragent)
                .OrderByDescending(c => c.OperationDateTime)
                .Take(count).Select(c => new TransactionViewModel(c))
                .ToListAsync();

            return Ok(transactionList);
        }

        // GET api/v1/transactions/range[?skip=100&count=100]
        [Route("range")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByRange([FromQuery]int skip = 0, [FromQuery]int count = 0)
        {
            if (count > 1000) count = 1000;

            var userid = _identityService.GetUserIdentity();

            var transactionList = await _context.PWTransactions
                .Where(c => c.AgentId == userid)
                .Include(c => c.Сounteragent)
                .OrderByDescending(c => c.OperationDateTime)
                .Skip(skip).Take(count)
                .Select(c => new TransactionViewModel(c))
                .ToListAsync();

            return Ok(transactionList);
        }

        // GET api/v1/transactions/bydate[?startDate=10.10.2019&endDate=15.10.2019]
        [Route("bydate")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByDate([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            var userid = _identityService.GetUserIdentity();

            var transactionList = await _context.PWTransactions
                .Where(c => c.AgentId == userid && c.OperationDateTime >= startDate && c.OperationDateTime <= endDate)
                .Include(c => c.Сounteragent)
                .OrderByDescending(c => c.OperationDateTime)
                .Select(c => new TransactionViewModel(c))
                .ToListAsync();

            return Ok(transactionList);
        }

        // GET api/v1/transactions/balance
        [Route("balance")]
        [HttpGet]
        [ProducesResponseType(typeof(Decimal), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<decimal>> GetBalance()
        {
            var userid = _identityService.GetUserIdentity();

            var lastTransaction = await _context.PWTransactions
                .Where(c => c.AgentId == userid)
                .OrderByDescending(c => c.OperationDateTime)
                .FirstOrDefaultAsync();

            if (lastTransaction == null)
            {
                return Ok(0);
            }
            return Ok(Decimal.Round(lastTransaction.AgentBalance, 6));
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TransactionViewModel>> GetTransaction(Guid id)
        {
            var userid = _identityService.GetUserIdentity();

            var transaction = await _context.PWTransactions
                .Where(c => c.AgentId == userid)
                .Include(c => c.Сounteragent)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(new TransactionViewModel(transaction));
        }

        // POST: api/transactions/transfer
        [Route("transfer")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TransactionViewModel>> Transfer(string counteragentId, decimal transactionAmount, string description)
        {
            if (transactionAmount < 0 
                || String.IsNullOrWhiteSpace(counteragentId)
                || Guid.TryParse(counteragentId, out var s) == false)
            {
                return BadRequest();
            }

            string agentId = _identityService.GetUserIdentity();

            var agentOneLastTr = await _context.PWTransactions
                .Where(c => c.AgentId == agentId)
                .OrderBy(c => c.OperationDateTime)
                .FirstOrDefaultAsync();

            var agentTwoLastTr = await _context.PWTransactions
                .Where(c => c.AgentId == counteragentId)
                .OrderBy(c => c.OperationDateTime)
                .FirstOrDefaultAsync();

            var operationTime = DateTime.UtcNow;
            transactionAmount = Decimal.Round(transactionAmount, 2);

            if (agentOneLastTr.AgentBalance < transactionAmount)
            {
                ModelState.AddModelError("transactionAmount", "Agent balance less than transaction amount");
                return BadRequest(ModelState);
            }

            PWTransaction transactionOne = new PWTransaction()
            {
                Id = Guid.NewGuid(),
                AgentId = agentId,
                СounteragentId = counteragentId,
                TransactionAmount = - transactionAmount,
                Description = description ?? "",
                OperationDateTime = operationTime <= agentOneLastTr.OperationDateTime ? agentOneLastTr.OperationDateTime.AddTicks(1) : operationTime,
                AgentBalance = agentOneLastTr.AgentBalance - transactionAmount
            };

            PWTransaction transactionTwo = new PWTransaction()
            {
                Id = Guid.NewGuid(),
                AgentId = counteragentId,
                СounteragentId = agentId,
                TransactionAmount = transactionAmount,
                Description = description ?? "",
                OperationDateTime = operationTime <= agentTwoLastTr.OperationDateTime ? agentTwoLastTr.OperationDateTime.AddTicks(1) : operationTime,
                AgentBalance = agentTwoLastTr.AgentBalance + transactionAmount
            };

            PWOperationPair operationPair = new PWOperationPair()
            {
                TransactionOneId = transactionOne.Id,
                TransactionTwoId = transactionTwo.Id
            };

            _context.PWTransactions.Add(transactionOne);
            _context.PWTransactions.Add(transactionTwo);
            _context.PWOperationPairs.Add(operationPair);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exc)
            {
                _logger.LogWarning($"PostTransaction agentId: {agentId}; counteragentId: {counteragentId}; transactionAmount: {transactionAmount}; Error: {exc.ToString()}." );
                throw;
            }

            return CreatedAtAction("GetTransaction", new { id = transactionOne.Id }, new TransactionViewModel(transactionOne));
        }

    }
}
