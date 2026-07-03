namespace Infrastructure.BaseModels
{
    public class SpecificExpensesBaseModel
    {
        public int Id { get; set; }
        public int SpecificId {  get; set; }
        public DateTime ExtractDate {  get; set; }
        public string Description {  get; set; }
        public decimal EntryValue {  get; set; }
        public int ExpenseId {  get; set; }
        public string ExpenseName {  get; set; }
        public decimal ExpenseValue {  get; set; }
        public DateTime ExpenseDate { get; set; }
        public bool Variable {  get; set; }
        public decimal Balance {  get; set; }
    }
}
