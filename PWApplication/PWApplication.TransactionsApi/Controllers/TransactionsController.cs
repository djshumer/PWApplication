using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PWApplication.TransactionApi.Extensions;
using PWApplication.TransactionApi.Infrastructure.Data;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;
using PWApplication.TransactionApi.Infrastructure.Exceptions;
using PWApplication.TransactionApi.Infrastructure.Services;
using PWApplication.TransactionApi.Models;

namespace PWApplication.TransactionApi.Controllers
{
    [Route("api/v1/transactions")]
    [Authorize]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ILogger<TransactionsController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/v1/transactions/last[?count=10]
        [Route("last")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetLastTransactions([FromQuery]int count = 0)
        {
            if (count < 1)
                return BadRequest();
            if (count > 1000) count = 1000;

            var userId = _identityService.GetUserIdentity();

            var transactionList = await _unitOfWork.TransactionRepository.GetLastTransactionsWithIncludeAsync(userId, count);

            return Ok(transactionList.ToTransactionViewModels());
        }

        // GET api/v1/transactions/range[?skip=100&count=100]
        [Route("range")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByRange([FromQuery]int skip = 0, [FromQuery]int count = 0)
        {
            if (count > 1000) count = 1000;

            var userId = _identityService.GetUserIdentity();

            var transactionList = await _unitOfWork.TransactionRepository.GetTransactionsByRangeAsync(userId, skip, count);

            return Ok(transactionList.ToTransactionViewModels());
        }

        // GET api/v1/transactions/bydate[?startDate=10.10.2019&endDate=15.10.2019]
        [Route("bydate")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByDate([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            var userId = _identityService.GetUserIdentity();

            var transactionList = await _unitOfWork.TransactionRepository.GetTransactionsByDateAsync(userId, startDate, endDate);
              
            return Ok(transactionList.ToTransactionViewModels());
        }

        // GET api/v1/transactions/balance
        [Route("balance")]
        [HttpGet]
        [ProducesResponseType(typeof(Decimal), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<decimal>> GetBalance()
        {
            var userId = _identityService.GetUserIdentity();

            var lastTransaction = await _unitOfWork.TransactionRepository.GetLastTransaction(userId);

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

            var transaction = await _unitOfWork.TransactionRepository.GetTransactionWithInclude(userid, id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(new TransactionViewModel(transaction));
        }

      
        [Route("transfer")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TransactionViewModel>> Transfer(TransactionModel model)
        {
            string agentId = _identityService.GetUserIdentity();

            if (model.TransactionAmount < 0 
                || String.IsNullOrWhiteSpace(model.CounteragentId)
                || Guid.TryParse(model.CounteragentId, out var s) == false
                || agentId == model.CounteragentId)
            {
                return BadRequest();
            }

            var agentOneLastTr = await _unitOfWork.TransactionRepository.GetLastTransaction(agentId);

            var agentTwoLastTr = await _unitOfWork.TransactionRepository.GetLastTransaction(model.CounteragentId);

            var operationTime = DateTime.UtcNow;
            model.TransactionAmount = Decimal.Round(model.TransactionAmount, 2);

            if (agentOneLastTr.AgentBalance < model.TransactionAmount)
            {
                ModelState.AddModelError("transactionAmount", "Agent balance less than transaction amount");
                return BadRequest(ModelState);
            }

            PWTransaction transactionOne = new PWTransaction()
            {
                Id = Guid.NewGuid(),
                AgentId = agentId,
                СounteragentId = model.CounteragentId,
                TransactionAmount = -model.TransactionAmount,
                Description = model.Description ?? "",
                OperationDateTime = operationTime <= agentOneLastTr.OperationDateTime ? agentOneLastTr.OperationDateTime.AddTicks(1) : operationTime,
                AgentBalance = agentOneLastTr.AgentBalance - model.TransactionAmount
            };

            PWTransaction transactionTwo = new PWTransaction()
            {
                Id = Guid.NewGuid(),
                AgentId = model.CounteragentId,
                СounteragentId = agentId,
                TransactionAmount = model.TransactionAmount,
                Description = model.Description ?? "",
                OperationDateTime = operationTime <= agentTwoLastTr.OperationDateTime ? agentTwoLastTr.OperationDateTime.AddTicks(1) : operationTime,
                AgentBalance = agentTwoLastTr.AgentBalance + model.TransactionAmount
            };

            PWOperationPair operationPair = new PWOperationPair()
            {
                TransactionOneId = transactionOne.Id,
                TransactionTwoId = transactionTwo.Id
            };

            _unitOfWork.TransactionRepository.Create(transactionOne);
            _unitOfWork.TransactionRepository.Create(transactionTwo);
            _unitOfWork.OperationPairsRepository.Create(operationPair);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                transactionOne = await _unitOfWork.TransactionRepository.GetTransactionWithInclude(agentId, transactionOne.Id);
            }
            catch (DbException exc)
            {
                _logger.LogWarning($"PostTransaction agentId: {agentId}; counteragentId: {model.CounteragentId}; transactionAmount: {model.TransactionAmount}; Error: {exc.ToString()}.");
                throw;
            }

            return CreatedAtAction("GetTransaction", new { id = transactionOne.Id }, new TransactionViewModel(transactionOne));
        }

    }
}
