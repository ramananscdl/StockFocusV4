using StockFocus.Entites;

namespace StockFocus.UI
{
	public interface IsfControls
	{
		TransactionColumn Field { get; set; }
		int TransactionId { get; set; }
	}
}