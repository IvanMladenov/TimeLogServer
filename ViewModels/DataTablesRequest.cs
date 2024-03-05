namespace TimeLogs.ViewModels
{
    public class DataTablesRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<DataTablesOrder> order { get; set; }
        public List<DataTablesColumn> columns { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
    }
}
