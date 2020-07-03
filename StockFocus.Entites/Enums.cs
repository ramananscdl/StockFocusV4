using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFocus.Entites
{
    public enum Exchange
    {
        BSE = 0,
        NSE = 1

    }

    public enum TradeType
    {

        Sell = 0,
        Buy = 1
    }


    public enum InputType
    {
        Textbox = 1,
        NumericTextbox = 2,
        DropDown = 3,
        Calendar = 4
    }

    public enum InputOperation
    {
        AddStock = 1,
        AddTransaction = 2,
        AddProfile = 3,
        AddSegment = 4
    }

    public enum ManageOperation
    {
       Edit,
       Delete,
       Create
    }

    public enum OperationMode
	{
        Single,
        Multiple
	}

    public enum GridSortOrder
    {
        None = 0,
        Ascending = 1,
        Descending = 2

    }


    public enum TransactionColumn
	{
        Date = 0,
        Quantity = 1,
        Amount =2,
        TotalAmount = 3,
        Transaction =4
	}
}
