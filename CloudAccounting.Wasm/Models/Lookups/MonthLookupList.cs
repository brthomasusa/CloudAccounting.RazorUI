namespace CloudAccounting.Wasm.Models.Lookups
{
    public static class MonthLookupList
    {
        public static readonly List<MonthLookup> Months =
        [
            new MonthLookup(0, "------"),
            new MonthLookup(1, "January"),
            new MonthLookup(2, "February"),
            new MonthLookup(3, "March"),
            new MonthLookup(4, "April"),
            new MonthLookup(5, "May"),
            new MonthLookup(6, "June"),
            new MonthLookup(7, "July"),
            new MonthLookup(8, "August"),
            new MonthLookup(9, "September"),
            new MonthLookup(10, "October"),
            new MonthLookup(11, "November"),
            new MonthLookup(12, "December")
        ];
    }
}
