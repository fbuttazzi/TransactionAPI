using System.Globalization;
using System.Text;
using TransactionAPI.Models;

namespace TransactionAPI.Services
{
    public class TransactionService
    {
        private readonly string filePath = "transactions.txt";

        // Salva uma nova transação no arquivo
        public void SaveTransaction(Transaction transaction)
        {
            transaction.Id = GetNextId();
            var record = FormatTransaction(transaction);
            File.AppendAllText(filePath, record + Environment.NewLine);
        }

        // Atualiza uma transação existente no arquivo
        public bool UpdateTransaction(Transaction transaction)
        {
            var transactions = GetAllTransactions();
            var index = transactions.FindIndex(t => t.Id == transaction.Id);

            if (index == -1)
                return false;

            transactions[index] = transaction;
            WriteAllTransactions(transactions);
            return true;
        }

        // Lê todas as transações do arquivo
        public List<Transaction> GetAllTransactions()
        {
            var transactions = new List<Transaction>();
            if (!File.Exists(filePath)) return transactions;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 8)
                {
                    var transaction = new Transaction
                    {
                        Id = int.Parse(parts[0]),
                        Type = parts[1],
                        Category = parts[2],
                        BudgetedAmount = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        ActualAmount = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                        Closed = bool.Parse(parts[5]),
                        Date = DateTime.Parse(parts[6]),
                        Description = parts[7]
                    };
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }

        // Obtém um novo ID para a transação
        private int GetNextId()
        {
            var transactions = GetAllTransactions();
            return transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;
        }

        // Escreve todas as transações no arquivo (usado na atualização)
        private void WriteAllTransactions(List<Transaction> transactions)
        {
            var lines = transactions.Select(t => FormatTransaction(t)).ToArray();
            File.WriteAllLines(filePath, lines);
        }

        // Formata uma transação para gravação no arquivo
        private string FormatTransaction(Transaction transaction)
        {
            return $"{transaction.Id}|{transaction.Type}|{transaction.Category}|{transaction.BudgetedAmount}|{transaction.ActualAmount}|{transaction.Closed}|{transaction.Date:yyyy-MM-dd}|{transaction.Description}";
        }

        // Obtém resumo mensal totalizado por categoria e descrição
        public object GetMonthlySummary(string month)
        {
            var transactions = GetAllTransactions();
            var selectedMonth = DateTime.ParseExact(month, "MM-yyyy", CultureInfo.InvariantCulture);

            var summary = new
            {
                Month = month,
                TotalRevenue = new
                {
                    Budgeted = transactions.Where(t => t.Type == "receita" && t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year).Sum(t => t.BudgetedAmount),
                    Actual = transactions.Where(t => t.Type == "receita" && t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year).Sum(t => t.ActualAmount)
                },
                TotalExpenses = new
                {
                    Budgeted = transactions.Where(t => t.Type == "despesa" && t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year).Sum(t => t.BudgetedAmount),
                    Actual = transactions.Where(t => t.Type == "despesa" && t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year).Sum(t => t.ActualAmount)
                },
                Categories = transactions
                    .Where(t => t.Date.Month == selectedMonth.Month && t.Date.Year == selectedMonth.Year)
                    .GroupBy(t => new { t.Category, t.Description })  // Agrupa por categoria e descrição
                    .Select(g => new
                    {
                        Category = g.Key.Category,
                        Description = g.Key.Description,
                        Budgeted = g.Sum(t => t.BudgetedAmount),
                        Actual = g.Sum(t => t.ActualAmount),
                        Closed = g.All(t => t.Closed)
                    })
            };

            return summary;
        }
    }
}
