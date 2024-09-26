using Microsoft.AspNetCore.Mvc;
using TransactionAPI.Services;
using TransactionAPI.Models;

namespace TransactionAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionsController()
        {
            _transactionService = new TransactionService();
        }

        // POST /transactions - Adiciona uma nova transação
        [HttpPost]
        public IActionResult PostTransaction([FromBody] Transaction transaction)
        {
            _transactionService.SaveTransaction(transaction);
            return Ok(new { Message = "Transação adicionada com sucesso!" });
        }

        // PUT /transactions/{id} - Atualiza uma transação existente
        [HttpPut("{id}")]
        public IActionResult PutTransaction(int id, [FromBody] Transaction transaction)
        {
            transaction.Id = id;
            var result = _transactionService.UpdateTransaction(transaction);
            if (!result) return NotFound(new { Message = "Transação não encontrada" });

            return Ok(new { Message = "Transação atualizada com sucesso!" });
        }

        // GET /summary?month=2024-09 - Resumo mensal por mês
        [HttpGet("summary")]
        public IActionResult GetMonthlySummary([FromQuery] string month)
        {
            var summary = _transactionService.GetMonthlySummary(month);
            return Ok(summary);
        }
    }

}
