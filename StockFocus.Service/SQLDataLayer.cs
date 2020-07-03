using StockFocus.Entites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Service
{
	public class SQLDataLayer : IDataLayer
	{
		  string _connectionString;//= System.Configuration.ConfigurationManager.AppSettings["SqlConnectionString"];

		public string ConnectionString { get => _connectionString; set => _connectionString = value; }

		public int AddPortfolio(Portfolio portfolio)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("InsertPortfolio", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(portfolio.PortfolioName, cmd, "@PortfolioName");
					AddParameters(portfolio.IsDefault, cmd, "@IsDefault");
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
			return result;
		}

		public int AddSegment(Segment segment)
		{

			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("InsertSegment", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(segment.SegmentName, cmd, "@SegmentName");

					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result;
		}

		public int AddStock(Stock stock)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("InsertStock", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(stock.Code.Code, cmd, "@Code");
					AddParameters(stock.StockName, cmd, "@StockName");
					AddParameters(stock.SegmentId, cmd, "@SegmentId");
					AddParameters(stock.StockExchange.ToString(), cmd, "@Exchange");
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result;
		}

		private static void AddParameters(object value, SqlCommand cmd, string paramName)
		{
			SqlParameter param = new SqlParameter(paramName, value);
			cmd.Parameters.Add(param);
		}

		public int AddTransaction(Transaction transaction)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("InsertTransaction", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(transaction.Amount, cmd, "@Amount");
					AddParameters(transaction.PortfolioId, cmd, "@PortfolioId");
					AddParameters(transaction.Quantity, cmd, "@Quantity");
					AddParameters(transaction.StockId, cmd, "@StockId");
					AddParameters(transaction.TransactionDate, cmd, "@TransactionDate");
					AddParameters((int)transaction.TransactionType, cmd, "@TransactionType");
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result;
		}

		public bool EditPortfolio(Portfolio portfolio)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("EditPortfolio", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					AddParameters(portfolio.PortfolioId, cmd, "@PortfolioId");
					AddParameters(portfolio.PortfolioName, cmd, "@PortfolioName");
					AddParameters(portfolio.IsDefault, cmd, "@IsDefault");
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result > 0;
		}

		public bool EditSegment(Segment segment)
		{
			throw new NotImplementedException();
		}

		public bool EditStock(Stock stock)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("EditStock", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					AddParameters(stock.StockId, cmd, "@StockId");
					AddParameters(stock.StockName, cmd, "@StockName");
					AddParameters(stock.SegmentId, cmd, "@SegmentId");
					AddParameters(stock.Code.Code, cmd, "@Code");
					AddParameters(stock.StockExchange, cmd, "@Exchange");
					
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result > 0;
		}

		public bool EditTransaction(Transaction transaction)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("EditTransaction", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(transaction.Amount, cmd, "@Amount");
					AddParameters(transaction.PortfolioId, cmd, "@PortfolioId");
					AddParameters(transaction.Quantity, cmd, "@Quantity");
					AddParameters(transaction.StockId, cmd, "@StockId");
					AddParameters(transaction.TransactionDate, cmd, "@TransactionDate");
					AddParameters(transaction.TransactionId, cmd, "@Transactionid");
					AddParameters((int)transaction.TransactionType, cmd, "@TransactionType");
					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());



				}
			}
			return result > 0;
		}

		public List<Segment> GetAllSegments()
		{
			List<Segment> segments = new List<Entites.Segment>();
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetAllSegments", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					con.Open();
					var reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						Segment s = new Segment();
						s.SegmentId = (int)reader["SegmentId"];
						s.SegmentName = reader["SegmentName"].ToString();

						segments.Add(s);
					}

				}
			}
			return segments;
		}

		public List<Transaction> GetAllTransactionsByStockId(int stockId, int portfolioId)
		{
			List<Transaction> transaction = new List<Transaction>();

			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetTransactionByStock", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					AddParameters(portfolioId, cmd, "@PortfolioId");
					AddParameters(stockId, cmd, "@stockId");
					con.Open();
					var reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						Transaction t = new Transaction();
						t.TransactionId = (int)reader["TransactionId"];
						t.StockId = stockId;
						t.TransactionDate = (DateTime)reader["TransactionDate"];
						t.Quantity = (int)reader["Quantity"];
						t.PortfolioId = portfolioId;
						t.Code = reader["Code"].ToString();
						t.Amount = reader["Amount"].ToDecimal();
						t.TransactionType = (TradeType)Convert.ToInt16(reader["TransactionType"]);
						transaction.Add(t);

					}

				}
			}

			return transaction;
		}

		public List<Portfolio> GetPortfolios()
		{
			List<Portfolio> portflios = new List<Portfolio>();
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("GetPortfolios", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					con.Open();
					var reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						Portfolio p = new Portfolio();
						p.PortfolioId = (int)reader["PortfolioId"];
						p.PortfolioName = reader["PortfolioName"].ToString();
						p.IsDefault = (bool)reader["IsDefault"];
						portflios.Add(p);

					}
				}

			}

			return portflios;
		}

		public List<Stock> GetStocksByPortfolioId(int portfolioId)
		{
			List<Stock> stocks = new List<Stock>();

			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetStocksByPortfolioId", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					SqlParameter param = new SqlParameter("@PortfolioId", portfolioId);
					cmd.Parameters.Add(param);
					con.Open();
					var reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						Stock s = new Stock();
						s.AdjustedPrice = reader["AdjustedPrice"].ToDecimal();
						s.NSECode = reader["Code"].ToString();
						s.StockName = reader["StockName"].ToString();
						s.SegmentId = (int)reader["SegmentId"];
						s.Quantity = (int)reader["TotalQuantity"];
						s.StockExchange = reader["Exchange"].ToString().Equals("BSE") ? Exchange.BSE : Exchange.NSE;
						s.PurchasePrice = reader["PurchasePrice"].ToDecimal();
						s.StockId = (int)reader["StockId"];
						stocks.Add(s);
					}

				}
			}

			return stocks;
		}

		public List<Transaction> GetTransactionsByPortfolio(DateTime StartDate, DateTime EndDate, int PortfolioId)
		{
			throw new NotImplementedException();
		}

		public List<Stock> GetStockList()
		{
			List<Stock> stocks = new List<Stock>();
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetStockList", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					con.Open();
					var reader = cmd.ExecuteReader();

					while (reader.Read())
					{
						Stock s = new Stock();

						s.StockName = reader["StockName"].ToString();
						s.StockId = (int)reader["StockId"];
						s.StockExchange = (Exchange)Enum.Parse(typeof(Exchange), reader["Exchange"].ToString());
						s.SegmentId = (int)reader["SegmentId"];
						if (s.StockExchange == Exchange.BSE)
							s.BSECode = reader["Code"].ToString();
						else
							s.NSECode = reader["Code"].ToString();
						stocks.Add(s);
					}

				}
			}
			return stocks;
		}

		public decimal GetNetRealizedProfitByProfileId(int portfolioId)
		{
			decimal result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetNetRealizedProfitByProfileId", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					SqlParameter param = new SqlParameter("@PortfolioId", portfolioId);
					cmd.Parameters.Add(param);
					con.Open();
					result = cmd.ExecuteScalar().ToDecimal();



				}
			}

			return result;
		}

		public bool DeletePortfolio(int portfolioId)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("DeletePortfolio", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					AddParameters(portfolioId, cmd, "@PortfolioId");

					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());

				}
			}
			return result > 0;
		}

		public bool DeleteStock(int stockId)
		{
			throw new NotImplementedException();
		}

		public bool DeleteTransaction(int transactionId)
		{
			int result = 0;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("DeleteTransaction", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					AddParameters(transactionId, cmd, "@TransactionId");

					con.Open();
					result = Convert.ToInt32(cmd.ExecuteScalar());

				}
			}
			return result > 0;
		}

		public bool DeleteSegment(int segmentId)
		{
			throw new NotImplementedException();
		}

		public DateTime GetLastUpdatedDate()
		{
			DateTime result = DateTime.Today;
			using (SqlConnection con = new SqlConnection(ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("GetLastUpdatedDate", con))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;

					con.Open();
				 
					result = Convert.ToDateTime(cmd.ExecuteScalar());
  			 

				}
			}
			return result;
		}
	}
}
