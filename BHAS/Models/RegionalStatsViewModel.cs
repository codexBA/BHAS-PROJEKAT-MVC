namespace BHAS.Models
{
    public class RegionalStatsViewModel
    {
        public int      RegionID         { get; set; }
        public string   RegionName       { get; set; }
        public string   RegionCode       { get; set; }
        public int Population       { get; set; }
        public int      Year             { get; set; }
        public decimal? GDP              { get; set; }
        public decimal? UnemploymentRate { get; set; }
        public decimal? AverageSalary    { get; set; }
        public decimal? InflationRate    { get; set; }
        public decimal? GDPPerCapita     { get; set; }
    }
}