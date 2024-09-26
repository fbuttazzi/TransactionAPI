namespace TransactionAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; } // Identificador único
        public string Type { get; set; } // "receita" ou "despesa"
        public string Category { get; set; }
        public decimal BudgetedAmount { get; set; } // Valor Orçado
        public decimal ActualAmount { get; set; }   // Valor Realizado
        public bool Closed { get; set; } // Se a transação está fechada
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

}
